using Auto.io.Flows.Application.Models.Parameters;
using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class SetClipboardStep : IStep
{
    private readonly IParameterService _parameterService;
    private readonly IKeyboardService _keyboardService;

    public SetClipboardStep(IParameterService parameterService, IKeyboardService keyboardService)
    {
        _parameterService = parameterService;
        _keyboardService = keyboardService;
    }

    public string Identifier => "SetClipboardV1";
    public string Description => "Sets the clipboard text to a provided value.";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new TextParameter
        {
            DisplayName = "Value",
        }
    };

    public Task ExecuteAsync(IEnumerable<object?> parameters)
    {
        string? parameter = parameters.ElementAtOrDefault(0)?.ToString();

        if (parameter is not null)
        {
            parameter = _parameterService.ReplaceVariables(parameter);

            _keyboardService.SetClipboard(parameter);
        }

        return Task.CompletedTask;
    }
}
