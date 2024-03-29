﻿using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class CtrlVShortcutStep : IStep
{
    private readonly IKeyboardService _keyboardService;

    public CtrlVShortcutStep(IKeyboardService keyboardService)
    {
        _keyboardService = keyboardService;
    }

    public string Identifier => "CtrlVShortcut";
    public string Description => "Simulates a Ctrl+V keyboard shortcut";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>();

    public Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        _keyboardService.CtrlVShortcut();

        return Task.CompletedTask;
    }
}
