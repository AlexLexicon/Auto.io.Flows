using Auto.io.Flows.Application.Models;
using Auto.io.Flows.ViewModels.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;

namespace Auto.io.Flows.ViewModels;
public partial class BuilderParameterComboBoxViewModel : BuilderParameterViewModel
{
    public BuilderParameterComboBoxViewModel(IParameter parameter) : base(parameter)
    {
        if (_parameter.Argument is not IEnumerable arguments)
        {
            throw new Exception($"{nameof(IParameter)}.{nameof(_parameter.Argument)} was not '{nameof(IEnumerable)}'.");
        }

        var values = new List<string>();
        foreach (object? argument in arguments)
        {
            string? argumentString = argument?.ToString();

            if (argumentString is not null)
            {
                values.Add(argumentString);
            }
        }

        Values = values;
        SelectedValue = (_parameter.InitalValue ?? values.FirstOrDefault())?.ToString();
    }

    [ObservableProperty]
    private IEnumerable<string>? _values;

    private bool _isSelectedValueChanging = false;
    private string? _selectedValue;
    public string? SelectedValue
    {
        get => _selectedValue;
        set
        {
            _selectedValue = value;

            if (!_isSelectedValueChanging)
            {
                _isSelectedValueChanging = true;

                OnSelectedValueChanged();
                OnPropertyChanged();

                _isSelectedValueChanging = false;
            }
        }
    }

    public override FlowParameter? GetFlowParameter()
    {
        return new FlowParameter
        {
            Identifier = _parameter.Identifier,
            Value = SelectedValue,
        };
    }

    public override void LoadFlowParameter(FlowParameter flowParameter)
    {
        SelectedValue = flowParameter.Value?.ToString();
    }

    private void OnSelectedValueChanged()
    {
        Update(SelectedValue);
    }
}
