using Auto.io.Flows.Application.Models.Parameters;

namespace Auto.io.Flows.Application.Models.Steps;
public class DelayStep : IStep
{
    public string Identifier => "Delay";
    public string Description => "Waits the provided number of Milliseconds.";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new IntegerParameter
        {
            DisplayName = "Milliseconds",
        }
    };

    public async Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        string? parameterString = parameters.FirstOrDefault()?.ToString();

        if (int.TryParse(parameterString, out int milliSeconds))
        {
            await Task.Delay(milliSeconds);
        }
    }
}
