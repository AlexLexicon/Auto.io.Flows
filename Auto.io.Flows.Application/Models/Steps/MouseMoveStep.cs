﻿using Auto.io.Flows.Application.Models.Parameters;
using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class MouseMoveStep : IStep
{
    private readonly IMouseService _mouseService;

    public MouseMoveStep(IMouseService mouseService)
    {
        _mouseService = mouseService;
    }

    public string Identifier => "MouseMove";
    public string Description => "Moves the mouse cursor to the provied X and Y coordinate on the screen.";

    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new IntegerParameter
        {
            DisplayName = "X",
        },
        new IntegerParameter
        {
            DisplayName = "Y",
        },
    };

    public Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        string? xString = parameters.ElementAtOrDefault(0)?.ToString();
        string? yString = parameters.ElementAtOrDefault(1)?.ToString();

        if (int.TryParse(xString, out int x) && int.TryParse(yString, out int y))
        {
            _mouseService.Move(x, y);
        }

        return Task.CompletedTask;
    }
}
