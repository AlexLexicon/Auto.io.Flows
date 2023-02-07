using Auto.io.Flows.Application.Models.Parameters;
using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class SendTextStep : IStep
{
    private readonly IKeyboardService _keyboardService;
    private readonly IParameterService _parameterService;

    public SendTextStep(
        IKeyboardService keyboardService, 
        IParameterService parameterService)
    {
        _keyboardService = keyboardService;
        _parameterService = parameterService;
    }

    public string Identifier => "SendTextV1";
    public string Description => "Sends the text as keyboard key strokes.";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new TextParameter
        {
            DisplayName = "Text",
        }
    };

    public Task ExecuteAsync(IEnumerable<object?> parameters)
    {
        string? parameter = parameters.ElementAtOrDefault(0)?.ToString();

        if (parameter is not null)
        {
            parameter = _parameterService.ReplaceVariables(parameter);

            _keyboardService.SendText(parameter);
        }

        return Task.CompletedTask;
    }
}
