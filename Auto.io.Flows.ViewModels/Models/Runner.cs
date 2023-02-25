using Auto.io.Flows.Application.Models;
using System.Collections.ObjectModel;

namespace Auto.io.Flows.ViewModels.Models;
public class Runner : IRunner
{
    private const int STEPDELAYMILLISECONDSMINIMUM = 50;

    private readonly ObservableCollection<RunnerStepViewModel> _runnerStepViewModels;
    private readonly int _maxIterations;
    private readonly int _stepDelayMilliseconds;
    private readonly Func<Runner, Task> _callBack;
    private readonly bool _isInfinite;
    private readonly Action _completed;

    public Runner(
        ObservableCollection<RunnerStepViewModel> runnerStepViewModels,
        int maxIterations,
        int stepDelayMilliseconds,
        Func<Runner, Task> callback,
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
            if (CurrentViewModel is not null)
            {
                CurrentViewModel.State = RunnerStepViewModel.STATE_SUCCEEDED;
            }
            await NextAsync();
        }
    }

    public async Task PauseAsync()
    {
        await TogglePause();
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
    private RunnerStepViewModel? CurrentViewModel { get; set; }

    private bool _isNexting = false;
    private async Task NextAsync()
    {
        if (!_isNexting)
        {
            _isNexting = true;

            await _callBack.Invoke(this);

            CurrentViewModel = _runnerStepViewModels[Index];

            CurrentViewModel.State = RunnerStepViewModel.STATE_WAITING;
            int delay = IsSkipping ? STEPDELAYMILLISECONDSMINIMUM : _stepDelayMilliseconds;
            await Task.Delay(delay);

            CurrentViewModel.BringIntoViewCommand?.Execute(null);

            if (!IsSkipping)
            {
                bool success = await CurrentViewModel.RunAsync(this);

                if (!success)
                {
                    IsStopping = true;
                }
            }
            else
            {
                CurrentViewModel.State = RunnerStepViewModel.STATE_SKIPPED;
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
