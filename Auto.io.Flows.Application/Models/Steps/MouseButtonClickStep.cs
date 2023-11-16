using Auto.io.Flows.Application.Models.Parameters;
using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class MouseButtonClickStep : IStep
{
    private readonly IMouseService _mouseService;

    public MouseButtonClickStep(IMouseService mouseService)
    {
        _mouseService = mouseService;
    }

    public string Identifier => "MouseButtonClick";
    public string Description => "Presses down on the provided mouse button.";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new MouseButtonParameter
        {
            DisplayName = "Mouse Button",
        }
    };

    public Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        string? mouseButton = parameters.FirstOrDefault()?.ToString();

        if (mouseButton == IMouseService.MOUSE_BUTTONS_LEFT)
        {
            _mouseService.PressLeftMouseButton();
            _mouseService.ReleaseLeftMouseButton();
        }
        else if (mouseButton == IMouseService.MOUSE_BUTTONS_RIGHT)
        {
            _mouseService.PressRightMouseButton();
            _mouseService.ReleaseRightMouseButton();
        }

        return Task.CompletedTask;
    }
}