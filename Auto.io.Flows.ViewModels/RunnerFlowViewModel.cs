using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels.Models;
using Auto.io.Flows.ViewModels.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicom.Configuration.Settings;
using Lexicom.Mvvm;
using Lexicom.Wpf.Amenities.Dialogs;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace Auto.io.Flows.ViewModels;
public partial class RunnerFlowViewModel : ObservableObject, IDisposable
{
    private static readonly char[] DIGITS = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

    private const string STEP_DELAY_EIGHT_SECONDS = "1/8 Seconds";
    private const string STEP_DELAY_FORTH_SECONDS = "1/4 Seconds";
    private const string STEP_DELAY_HALF_SCEONDS = "1/2 Seconds";
    private const string STEP_DELAY_1_SECONDS = "1 Seconds";
    private const string STEP_DELAY_5_SECONDS = "5 Seconds";
    private const string STEP_DELAY_10_SECONDS = "10 Seconds";
    private const string STEP_DELAY_15_SECONDS = "15 Seconds";
    private const string STEP_DELAY_30_SECONDS = "30 Seconds";

    private readonly Guid _keyboardHandlerId;

    private readonly Flow _flow;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IKeyboardService _keyboardService;
    private readonly IParameterService _parameterService;
    private readonly IWindowsDialog _windowsDialog;
    private readonly IOptionsMonitor<FileDirectoryOptions> _fileDirectoryOptions;
    private readonly ISettingsWriter _settingsWriter;
    private readonly IFlowService _flowService;

    public RunnerFlowViewModel(
        Flow flow,
        IViewModelFactory dataContextFactory,
        IKeyboardService keyboardService,
        IParameterService parameterService,
        IWindowsDialog windowsDialog,
        IOptionsMonitor<FileDirectoryOptions> fileDirectoryOptions,
        ISettingsWriter settingsWriter,
        IFlowService flowService)
    {
        _keyboardHandlerId = Guid.NewGuid();

        _flow = flow;
        _viewModelFactory = dataContextFactory;
        _keyboardService = keyboardService;
        _parameterService = parameterService;
        _windowsDialog = windowsDialog;
        _fileDirectoryOptions = fileDirectoryOptions;
        _settingsWriter = settingsWriter;
        _flowService = flowService;

        RunnerStepViewModels = new ObservableCollection<RunnerStepViewModel>();
        if (_flow.Steps is not null)
        {
            foreach (FlowStep step in _flow.Steps)
            {
                var runnerStepViewModel = _viewModelFactory.Create<RunnerStepViewModel, FlowStep>(step);

                RunnerStepViewModels.Add(runnerStepViewModel);
            }
        }
        IterationsText = "1";
        IsIterationsTextValid = true;
        ToggleHotKeys = IKeysService.KEYS;
        SelectedToggleHotKey = ToggleHotKeys.First();
        StopHotKeySelected();
        StepDelays = new List<string>
        {
            STEP_DELAY_EIGHT_SECONDS,
            STEP_DELAY_FORTH_SECONDS,
            STEP_DELAY_HALF_SCEONDS,
            STEP_DELAY_1_SECONDS,
            STEP_DELAY_5_SECONDS,
            STEP_DELAY_10_SECONDS,
            STEP_DELAY_15_SECONDS,
            STEP_DELAY_30_SECONDS,
        };
        SelectedStepDelay = StepDelays.First();
        UpdateIterationsTextEnabled();
        Update();
        PauseText = "Pause";
    }

    [ObservableProperty]
    private bool _isRunnable;

    [ObservableProperty]
    private ObservableCollection<RunnerStepViewModel> _runnerStepViewModels = null!;

    private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            UpdateIterationsTextEnabled();
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private bool _isIterationsTextValid;

    private string? _iterationsText;
    public string? IterationsText
    {
        get => _iterationsText;
        set
        {
            _iterationsText = value;
            OnIterationsTextChanged();
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private IReadOnlyList<string> _toggleHotKeys = null!;

    [ObservableProperty]
    private string? _selectedToggleHotKey;

    private bool _isInfinite;
    public bool IsInfinite
    {
        get => _isInfinite;
        set
        {
            _isInfinite = value;
            UpdateIterationsTextEnabled();
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private bool _isIterationsTextEnabled;

    [ObservableProperty]
    private bool _isToggleHotKeysEnabled;

    [ObservableProperty]
    private bool _isInfiniteEnabled;

    [ObservableProperty]
    private bool _isStopEnabled;

    [ObservableProperty]
    private string? _currentIterationText;

    [ObservableProperty]
    private bool _isStopping;

    [ObservableProperty]
    private List<string> _stepDelays = null!;

    [ObservableProperty]
    private string _selectedStepDelay = null!;

    [ObservableProperty]
    private bool _isPauseEnabled;

    private bool IsToggleHotKeyPressed { get; set; }

    private void UpdateIterationsTextEnabled()
    {
        Update();
    }

    private bool _isDisposed;
    public void Dispose()
    {
        _keyboardService.UnRegisterKeyReleased(_keyboardHandlerId);
        _isDisposed = true;
    }

    [RelayCommand]
    private void LoadAppend()
    {
        Options.FileDirectoryOptions fileConfiguration = _fileDirectoryOptions.CurrentValue;

        string? filePath = _windowsDialog.OpenFile(new OpenFileSettings
        {
            InitialDirectory = fileConfiguration.SaveFileDirectory,
        });

        if (filePath is not null)
        {
            var fileInfo = new FileInfo(filePath);

            fileConfiguration.SaveFileDirectory = fileInfo.DirectoryName;

            _settingsWriter.SaveAndBind(fileConfiguration);

            Flow flow = _flowService.LoadFlow(filePath);
            if (flow.Steps is not null)
            {
                foreach (FlowStep step in flow.Steps)
                {
                    var runnerStepViewModel = _viewModelFactory.Create<RunnerStepViewModel, FlowStep>(step);

                    RunnerStepViewModels.Add(runnerStepViewModel);
                }
            }
        }
    }

    [RelayCommand]
    private void StopHotKeySelected()
    {
        if (!_isDisposed)
        {
            if (SelectedToggleHotKey is not null)
            {
                _keyboardService.RegisterKeyReleased(_keyboardHandlerId, SelectedToggleHotKey, async () =>
                {
                    bool isPaused = Runner is not null && Runner.IsPaused;

                    if (IsRunning)
                    {
                        IsToggleHotKeyPressed = true;
                        await Pause();
                    }
                    else if (!IsRunning && !IsStopping && !isPaused)
                    {
                        await RunAsync();
                    }
                });
            }
        }
    }

    [ObservableProperty]
    private string? _pauseText;

    [RelayCommand]
    private async Task Pause()
    {
        if (Runner is not null)
        {
            await Runner.TogglePause();
        }
        Update();
    }

    [RelayCommand]
    private async Task Stop()
    {
        IsStopping = true;
        if (Runner is not null)
        {
            await Runner.Stop();
        }
        Update();
    }

    private Runner? Runner { get; set; }

    [RelayCommand]
    private async Task RunAsync()
    {
        if (IsIterationsTextValid && !IsRunning)
        {
            IsRunning = true;
            Update();

            IsToggleHotKeyPressed = false;
            IsStopping = false;

            if (!int.TryParse(IterationsText, out int maxIterations))
            {
                maxIterations = 0;
            }

            int stepDelayMilliseconds = SelectedStepDelay switch
            {
                STEP_DELAY_EIGHT_SECONDS => 125,
                STEP_DELAY_FORTH_SECONDS => 250,
                STEP_DELAY_HALF_SCEONDS => 500,
                STEP_DELAY_1_SECONDS => 1000,
                STEP_DELAY_5_SECONDS => 5000,
                STEP_DELAY_10_SECONDS => 10000,
                STEP_DELAY_15_SECONDS => 15000,
                STEP_DELAY_30_SECONDS => 30000,
                _ => 0,
            };

            Runner = new Runner(RunnerStepViewModels, maxIterations, stepDelayMilliseconds, async runner =>
            {
                _parameterService.SetVariable(IParameterService.KEY_ITERATION, runner.Iteration.ToString());
                CurrentIterationText = $"{runner.Iteration + 1}/{maxIterations}";
                if (runner.IsStopping)
                {
                    await Stop();
                }
            }, IsInfinite, () =>
            {
                IsToggleHotKeyPressed = false;
                IsStopping = false;
                IsRunning = false;
                Update();
            });

            await Runner.SetupAsync();

            await Runner.StartAsync();
        }

        Update();
    }

    private void Update()
    {
        IsRunnable = !IsRunning && IsIterationsTextValid && RunnerStepViewModels.Any();
        IsIterationsTextEnabled = !IsRunning && !IsInfinite;
        IsToggleHotKeysEnabled = !IsRunning;
        IsInfiniteEnabled = !IsRunning;
        IsStopEnabled = IsRunning && !IsStopping;
        IsPauseEnabled = IsRunning && !IsStopping;
        if (Runner is not null)
        {
            PauseText = Runner.IsPaused ? "UnPause" : "Pause";
        }
    }

    private void OnIterationsTextChanged()
    {
        IsIterationsTextValid = IsInfinite || IterationsText is not null && IterationsText.All(DIGITS.Contains);
        Update();
    }
}
