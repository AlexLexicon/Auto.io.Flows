using Auto.io.Flows.Application;
using Auto.io.Flows.Application.Models;
using Auto.io.Flows.Application.Services;
using Auto.io.Flows.ViewModels.Abstractions;
using Auto.io.Flows.ViewModels.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auto.io.Flows.ViewModels;
public partial class BuilderStepViewModel : ObservableObject
{
    public event Action? Updated;
    public event Action? Delete;
    public event Action? MoveUp;
    public event Action? MoveDown;

    private readonly IStep _step;
    private readonly IBuilderParameterFactory _builderParameterFactory;
    private readonly IFlowService _flowService;

    public BuilderStepViewModel(
        IStep step,
        IBuilderParameterFactory dataContextFactory,
        IFlowService flowService)
    {
        _step = step;
        _builderParameterFactory = dataContextFactory;
        _flowService = flowService;

        IsValid = true;
        Name = _step.Identifier;
        Description = _step.Description;
        BuilderParameterViewModels = new List<BuilderParameterViewModel>();
        foreach (IParameter parameter in _step.Parameters)
        {
            BuilderParameterViewModel builderParameterViewModel = _builderParameterFactory.CreateBuilderParameter(parameter);

            builderParameterViewModel.Updated += Update;

            BuilderParameterViewModels.Add(builderParameterViewModel);
        }
    }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private bool _isValid;

    [ObservableProperty]
    private List<BuilderParameterViewModel> _builderParameterViewModels = null!;

    public FlowStep GetFlowStep()
    {
        var parameters = new List<FlowParameter?>();
        foreach (BuilderParameterViewModel builderParameterViewModel in BuilderParameterViewModels)
        {
            parameters.Add(builderParameterViewModel.GetFlowParameter());
        }

        return _flowService.CreateStep(_step, parameters);
    }

    private void Update()
    {
        IsValid = BuilderParameterViewModels.All(p => p.IsValid);

        Updated?.Invoke();
    }

    public void LoadFlowParameters(IEnumerable<FlowParameter?> flowParameters)
    {
        List<FlowParameter?> flowParametersList = flowParameters.ToList();
        for (int index = 0; index < flowParametersList.Count; index++)
        {
            FlowParameter? flowParameter = flowParametersList[index];
            if (flowParameter is not null)
            {
                BuilderParameterViewModel builderParameterViewModel = BuilderParameterViewModels[index];

                builderParameterViewModel.LoadFlowParameter(flowParameter);
            }
        }
    }

    [RelayCommand]
    private void OnMoveUp()
    {
        MoveUp?.Invoke();
    }

    [RelayCommand]
    private void OnMoveDown()
    {
        MoveDown?.Invoke();
    }

    [RelayCommand]
    private void OnDelete()
    {
        Delete?.Invoke();
    }
}
