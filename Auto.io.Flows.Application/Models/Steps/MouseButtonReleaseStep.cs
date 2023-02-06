using Auto.io.Flows.Application.Models.Parameters;
using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class MouseButtonReleaseStep : IStep
{
    private IMouseService _mouseService;

    public MouseButtonReleaseStep(IMouseService mouseService)
    {
        _mouseService = mouseService;
    }

    public string Identifier => "MouseButtonRelease";
    public string Description => "Releases the provided mouse button.";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new MouseButtonParameter
        {
            DisplayName = "Mouse Button",
        }
    };

    public Task ExecuteAsync(IEnumerable<object?> parameters)
    {
        string? mouseButton = parameters.FirstOrDefault()?.ToString();

        if (mouseButton == IMouseService.MOUSE_BUTTONS_LEFT)
        {
            _mouseService.ReleaseLeftMouseButton();
        }

        return Task.CompletedTask;
    }
}
