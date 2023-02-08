using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels.Configurations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicon.Common.Wpf.DependencyInjection.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Services;
using Lexicon.Common.Wpf.DependencyInjection.Amenities.Abstractions.Settings;
using Lexicon.Common.Wpf.DependencyInjection.Mvvm.Abstractions.Factories;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
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
    private readonly IDataContextFactory _dataContextFactory;
    private readonly IKeyboardService _keyboardService;
    private readonly IParameterService _parameterService;
    private readonly IWindowsDialogService _windowsDialogService;
    private readonly IOptionsMonitor<FileConfiguration> _fileOptions;
    private readonly ISettingsService _settingsService;
    private readonly IFlowService _flowService;

    public RunnerFlowViewModel(
        Flow flow,
        IDataContextFactory dataContextFactory,
        IKeyboardService keyboardService,
        IParameterService parameterService,
        IWindowsDialogService windowsDialogService,
        IOptionsMonitor<FileConfiguration> fileOptions,
        ISettingsService settingsService,
        IFlowService flowService)
    {
        _keyboardHandlerId = Guid.NewGuid();

        _flow = flow;
        _dataContextFactory = dataContextFactory;
        _keyboardService = keyboardService;
        _parameterService = parameterService;
        _windowsDialogService = windowsDialogService;
        _fileOptions = fileOptions;
        _settingsService = settingsService;
        _flowService = flowService;

        RunnerStepViewModels = new ObservableCollection<RunnerStepViewModel>();
        if (_flow.Steps is not null)
        {
            foreach (FlowStep step in _flow.Steps)
            {
                var runnerStepViewModel = _dataContextFactory.Create<RunnerStepViewModel, FlowStep>(step);

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
        FileConfiguration fileConfiguration = _fileOptions.CurrentValue;

        string? filePath = _windowsDialogService.OpenFile(new OpenFileSettings
        {
            InitialDirectory = fileConfiguration.SaveFileDirectory,
        });

        if (filePath is not null)
        {
            var fileInfo = new FileInfo(filePath);

            fileConfiguration.SaveFileDirectory = fileInfo.DirectoryName;

            _settingsService.BindAndSave(fileConfiguration);

            Flow flow = _flowService.LoadFlow(filePath);
            if (flow.Steps is not null)
            {
                foreach (FlowStep step in flow.Steps)
                {
                    var runnerStepViewModel = _dataContextFactory.Create<RunnerStepViewModel, FlowStep>(step);

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

    private TheRunner? Runner { get; set; }
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

            Runner = new TheRunner(RunnerStepViewModels, maxIterations, stepDelayMilliseconds, async runner =>
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

    private class TheRunner
    {
        private const int STEPDELAYMILLISECONDSMINIMUM = 50;

        private readonly ObservableCollection<RunnerStepViewModel> _runnerStepViewModels;
        private readonly int _maxIterations;
        private readonly int _stepDelayMilliseconds;
        private readonly Func<TheRunner, Task> _callBack;
        private readonly bool _isInfinite;
        private readonly Action _completed;

        public TheRunner(
            ObservableCollection<RunnerStepViewModel> runnerStepViewModels,
            int maxIterations,
            int stepDelayMilliseconds,
            Func<TheRunner, Task> callback,
            bool isInfinite,
            Action completed)
        {
            _runnerStepViewModels = runnerStepViewModels;
            _maxIterations = maxIterations;
            _stepDelayMilliseconds = stepDelayMilliseconds;
            _callBack = callback;
            _isInfinite = isInfinite;
            _completed = completed;
        }

        public Task SetupAsync()
        {
            foreach (RunnerStepViewModel stepViewModel in _runnerStepViewModels)
            {
                stepViewModel.State = RunnerStepViewModel.STATE_NOTSTARTED;
            }

            return Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            await NextAsync();
        }

        public async Task TogglePause()
        {
            IsPaused = !IsPaused;
            if (!IsPaused)
            {
                await NextAsync();
            }
        }

        public async Task Stop()
        {
            if (!IsStopping)
            {
                IsStopping = true;
                if (IsPaused)
                {
                    IsPaused = false;
                    await NextAsync();
                }
            }
        }

        public bool IsStopping { get; private set; }

        public int Index { get; private set; }
        public bool IsSkipping { get; private set; }
        public bool IsComplete { get; private set; }
        public int Iteration { get; private set; }

        public bool IsPaused { get; private set; }

        private bool _isNexting = false;
        private async Task NextAsync()
        {
            if (!_isNexting)
            {
                _isNexting = true;

                await _callBack.Invoke(this);

                var runnerStepViewModel = _runnerStepViewModels[Index];

                runnerStepViewModel.State = RunnerStepViewModel.STATE_WAITING;
                int delay = IsSkipping ? STEPDELAYMILLISECONDSMINIMUM : _stepDelayMilliseconds;
                await Task.Delay(delay);

                runnerStepViewModel.BringIntoViewCommand?.Execute(null);

                if (!IsSkipping)
                {
                    bool success = await runnerStepViewModel.RunAsync();

                    if (!success)
                    {
                        IsStopping = true;
                    }
                }
                else
                {
                    runnerStepViewModel.State = RunnerStepViewModel.STATE_SKIPPED;
                }

                Index++;

                if (IsStopping)
                {
                    IsSkipping = true;
                }

                if (Index >= _runnerStepViewModels.Count)
                {
                    if (!IsSkipping && (_isInfinite || Iteration < _maxIterations))
                    {
                        Index = 0;
                        Iteration++;
                    }

                    if (Iteration >= _maxIterations && !_isInfinite)
                    {
                        IsSkipping = true;
                    }

                    if (IsSkipping)
                    {
                        IsComplete = true;
                    }
                    else
                    {
                        foreach (RunnerStepViewModel stepViewModel in _runnerStepViewModels)
                        {
                            stepViewModel.State = RunnerStepViewModel.STATE_NOTSTARTED;
                        }
                    }
                }

                _isNexting = false;
            }

            if (!IsComplete && !IsPaused)
            {
                await NextAsync();
            }
            else if (IsComplete)
            {
                _completed.Invoke();
            }
        }
    }
}
