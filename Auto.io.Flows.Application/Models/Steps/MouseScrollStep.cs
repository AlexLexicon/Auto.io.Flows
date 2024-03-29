﻿using Auto.io.Flows.Application.Models.Parameters;
using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class MouseScrollStep : IStep
{
    private readonly IMouseService _mouseService;

    public MouseScrollStep(IMouseService mouseService)
    {
        _mouseService = mouseService;
    }

    public string Identifier => "MouseScroll";
    public string Description => "Scrolls the mouse wheel";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>
    {
        new IntegerParameter
        {
            DisplayName = "Amount",
        }
    };

    public Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        string? amountString = parameters.FirstOrDefault()?.ToString();

        if (int.TryParse(amountString, out int amount))
        {
            _mouseService.ScrollMouseButton(amount);
        }

        return Task.CompletedTask;
    }
}
