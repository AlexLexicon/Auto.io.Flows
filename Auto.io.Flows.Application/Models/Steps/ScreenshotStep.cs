using Auto.io.Flows.Application.Models.Parameters;
using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class ScreenshotStep : IStep
{
    private readonly IParameterService _parameterService;
    private readonly IScreenService _screenService;

    public ScreenshotStep(IScreenService screenService)
    {
        _screenService = screenService;
    }

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
        new TextParameter
        {
            DisplayName = "FileNamePath",
        }
    };

    public Task ExecuteAsync(IEnumerable<object?> parameters)
    {
        string? x1String = parameters.ElementAtOrDefault(0)?.ToString();
        string? y1String = parameters.ElementAtOrDefault(1)?.ToString();
        string? x2String = parameters.ElementAtOrDefault(2)?.ToString();
        string? y2String = parameters.ElementAtOrDefault(3)?.ToString();
        object? fileNamePathObject = parameters.ElementAtOrDefault(4);

        if (int.TryParse(x1String, out int x1) &&
            int.TryParse(y1String, out int y1) &&
            int.TryParse(x2String, out int x2) &&
            int.TryParse(y2String, out int y2) &&
            fileNamePathObject is string fileNamePath &&
            !string.IsNullOrWhiteSpace(fileNamePath))
        {
            fileNamePath = _parameterService.ReplaceVariables(fileNamePath);

            _screenService.Screenshot(x1, y1, x2, y2, fileNamePath);
        }

        return Task.CompletedTask;
    }
}
