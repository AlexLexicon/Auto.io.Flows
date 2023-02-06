using Auto.io.Flows.Application.Models.Parameters;

namespace Auto.io.Flows.Application.Models.Steps;
public class ScreenshotStep : IStep
{
    public string Identifier => "ScreenshotV1";
    public string Description => "Creates a screenshot image from the provided rect region.";

    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new IntegerParameter
        {
            DisplayName = "X1",
        },
        new IntegerParameter
        {
            DisplayName = "Y1",
        },
        new IntegerParameter
        {
            DisplayName = "X2",
        },
        new IntegerParameter
        {
            DisplayName = "Y2",
        },
    };

    public Task ExecuteAsync(IEnumerable<object?> parameters)
    {
        return Task.CompletedTask;
    }
}
