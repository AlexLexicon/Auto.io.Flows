﻿using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Lexicom.Mvvm;
using System.Windows.Input;

namespace Auto.io.Flows.ViewModels;
public partial class RunnerStepViewModel : ObservableObject
{
    public const string STATE_NOTSTARTED = "notstarted";
    public const string STATE_WAITING = "waiting";
    public const string STATE_RUNNING = "running";
    public const string STATE_SKIPPED = "skipped";
    public const string STATE_CANCELLED = "cancelled";
    public const string STATE_SUCCEEDED = "succeeded";
    public const string STATE_FAILED = "failed";

    private readonly FlowStep _flowStep;
    private readonly IStepService _stepService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IStep _step;
    private readonly IReadOnlyList<object?> _stepParameters;

    public RunnerStepViewModel(
        FlowStep flowStep,
        IStepService stepService,
        IViewModelFactory viewModelFactory)
    {
        _flowStep = flowStep;
        _stepService = stepService;
        _viewModelFactory = viewModelFactory;

        string identifier = _flowStep.Identifer;
        _step = _stepService.GetStepByIdentifier(identifier);
        Name = _step.Identifier;
        Description = _step.Description;
        RunnerParameterViewModels = new List<RunnerParameterViewModel>();
        var stepParameters = new List<object?>();
        List<FlowParameter?> flowParameters = flowStep.Parameters.ToList();
        for (int index = 0; index < _step.Parameters.Count; index++)
        {
            FlowParameter? flowParameter = flowParameters[index];
            if (flowParameter is not null)
            {
                stepParameters.Add(flowParameter.Value);
                IParameter parameter = _step.Parameters[index];
                var runnerParameterViewModel = _viewModelFactory.Create<RunnerParameterViewModel, IParameter, FlowParameter>(parameter, flowParameter);
                RunnerParameterViewModels.Add(runnerParameterViewModel);
            }
        }
        _stepParameters = stepParameters;
        State = STATE_NOTSTARTED;
        IsEnabled = true;
    }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _description;

    private string? _state;
    public string? State
    {
        get => _state;
        set
        {
            if (IsEnabled || value == STATE_SKIPPED)
            {
                _state = value;
                OnPropertyChanged();
            }
        }
    }

    [ObservableProperty]
    private List<RunnerParameterViewModel> _runnerParameterViewModels = null!;

    public ICommand? BringIntoViewCommand { get; set; }

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            OnPropertyChanged();
            State = IsEnabled ? STATE_NOTSTARTED : STATE_SKIPPED;
        }
    }

    public async Task<bool> RunAsync(IRunner runner)
    {
        State = STATE_RUNNING;

        try
        {
            if (!IsEnabled)
            {
                State = STATE_SKIPPED;
            }
            else
            {
                await _step.ExecuteAsync(runner, _stepParameters);

                State = runner.IsPaused ? STATE_WAITING : STATE_SUCCEEDED;
            }
            return true;
        }
        catch
        {
            State = STATE_FAILED;

            return false;
        }
    }
}
