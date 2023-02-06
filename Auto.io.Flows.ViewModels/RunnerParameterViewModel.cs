using Auto.io.Flows.Application.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Auto.io.Flows.ViewModels;
public partial class RunnerParameterViewModel : ObservableObject
{
    public RunnerParameterViewModel(IParameter parameter, FlowParameter flowParameter)
    {
        Name = parameter.DisplayName;
        Value = flowParameter.Value?.ToString();
    }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _value;
}
