using Auto.io.Flows.Application.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Auto.io.Flows.ViewModels.Abstractions;
public abstract partial class BuilderParameterViewModel : ObservableObject
{
    public event Action? Updated;

    protected readonly IParameter _parameter;

    public BuilderParameterViewModel(IParameter parameter)
    {
        _parameter = parameter;

        Name = _parameter.DisplayName;
    }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private bool _isValid;

    public abstract FlowParameter? GetFlowParameter();

    public abstract void LoadFlowParameter(FlowParameter flowParameter);

    protected virtual void Update(object? value) 
    {
        IsValid = _parameter.Validate(value);

        Updated?.Invoke();
    }
}
