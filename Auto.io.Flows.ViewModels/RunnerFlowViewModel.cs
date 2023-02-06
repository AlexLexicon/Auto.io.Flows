using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lexicon.Common.Wpf.DependencyInjection.Mvvm.Abstractions.Factories;

namespace Auto.io.Flows.ViewModels;
public partial class RunnerFlowViewModel : ObservableObject
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

    public RunnerFlowViewModel(
        Flow flow,
        IDataContextFactory dataContextFactory,
        IKeyboardService keyboardService,
        IParameterService parameterService)
    {
        _keyboardHandlerId = Guid.NewGuid();

        _flow = flow;
        _dataContextFactory = dataContextFactory;
        _keyboardService = keyboardService;
        _parameterService = parameterService;

        RunnerStepViewModels = new List<RunnerStepViewModel>();
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
    }

    [ObservableProperty]
    private bool _isRunnable;

    [ObservableProperty]
    private List<RunnerStepViewModel> _runnerStepViewModels = null!;

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

    private bool IsToggleHotKeyPressed { get; set; }

    private void UpdateIterationsTextEnabled()
    {
        Update();
    }

    [RelayCommand]
    private void StopHotKeySelected()
    {
        if (SelectedToggleHotKey is not null)
        {
            _keyboardService.KeyReleased(_keyboardHandlerId, SelectedToggleHotKey, async () =>
            {
                if (IsRunning && !IsStopping)
                {
                    IsToggleHotKeyPressed = true;
                    IsStopping = true;
                    Update();
                }
                else if (!IsRunning)
                {
                    await RunAsync();
                }
            });
        }
    }

    [RelayCommand]
    private void Stop()
    {
        IsStopping = true;
        Update();
    }

    [RelayCommand]
    private async Task RunAsync()
    {
        if (IsIterationsTextValid)
        {
            IsRunning = true;
            Update();

            IsToggleHotKeyPressed = false;
            IsStopping = false;

            if (!int.TryParse(IterationsText, out int maxIterations))
            {
                maxIterations = 0;
            }

            foreach (RunnerStepViewModel stepViewModel in RunnerStepViewModels)
            {
                stepViewModel.State = RunnerStepViewModel.STATE_NOTSTARTED;
            }

            int stepDelayMillisecondsMinimum = 125;
            int stepDelayMilliseconds = SelectedStepDelay switch
            {
                STEP_DELAY_FORTH_SECONDS => 250,
                STEP_DELAY_HALF_SCEONDS => 500,
                STEP_DELAY_1_SECONDS => 1000,
                STEP_DELAY_5_SECONDS => 5000,
                STEP_DELAY_10_SECONDS => 10000,
                STEP_DELAY_15_SECONDS => 15000,
                STEP_DELAY_30_SECONDS => 30000,
                _ => stepDelayMillisecondsMinimum,
            };

            int iteration = 0;
            _parameterService.SetVariable(IParameterService.KEY_ITERATION, iteration.ToString());
            int index = 0;
            bool isSkipping = false;
            bool isComplete = false;
            while (!isComplete)
            {
                CurrentIterationText = $"{iteration + 1}/{maxIterations}";

                var runnerStepViewModel = RunnerStepViewModels[index];

                runnerStepViewModel.State = RunnerStepViewModel.STATE_WAITING;
                int delay = isSkipping ? stepDelayMillisecondsMinimum : stepDelayMilliseconds;
                await Task.Delay(delay);

                if (!isSkipping)
                {
                    
                    bool success = await runnerStepViewModel.RunAsync();

                    if (!success)
                    {
                        Stop();
                    }
                }
                else
                {
                    runnerStepViewModel.State = RunnerStepViewModel.STATE_SKIPPED;
                }

                index++;

                if (IsToggleHotKeyPressed || IsStopping)
                {
                    isSkipping = true;
                }

                if (index >= RunnerStepViewModels.Count)
                {
                    if (!isSkipping && (IsInfinite || iteration < maxIterations))
                    {
                        index = 0;
                        iteration++;
                        _parameterService.SetVariable(IParameterService.KEY_ITERATION, iteration.ToString());
                    }

                    if (iteration >= maxIterations && !IsInfinite)
                    {
                        isSkipping = true;
                    }

                    if (isSkipping)
                    {
                        isComplete = true;
                    }
                    else
                    {
                        foreach (RunnerStepViewModel stepViewModel in RunnerStepViewModels)
                        {
                            stepViewModel.State = RunnerStepViewModel.STATE_NOTSTARTED;
                        }
                    }
                }
            }

            IsToggleHotKeyPressed = false;
            IsStopping = false;
            IsRunning = false;
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
    }

    private void OnIterationsTextChanged()
    {
        IsIterationsTextValid = IsInfinite || IterationsText is not null && IterationsText.All(DIGITS.Contains);
        Update();
    }
}
