using Auto.io.Flows.Application.Models;
using Auto.io.Flows.ViewModels.Abstractions;

namespace Auto.io.Flows.ViewModels;
public partial class BuilderParameterTextBoxViewModel : BuilderParameterViewModel
{
    public BuilderParameterTextBoxViewModel(IParameter parameter) : base(parameter)
    {
        Value = _parameter.InitalValue?.ToString();
    }

    private bool _isValueChanging = false;
    private string? _value;
    public string? Value
    {
        get => _value;
        set
        {
            _value = value;

            if (!_isValueChanging)
            {
                _isValueChanging = true;

                OnTextValueChanged();
                OnPropertyChanged();

                _isValueChanging = false;
            }
        }
    }

    public override FlowParameter? GetFlowParameter()
    {
        return new FlowParameter
        {
            Identifier = _parameter.Identifier,
            Value = Value,
        };
    }

    public override void LoadFlowParameter(FlowParameter flowParameter)
    {
        Value = flowParameter.Value?.ToString();
    }

    private void OnTextValueChanged()
    {
        Update(Value);
    }
}
