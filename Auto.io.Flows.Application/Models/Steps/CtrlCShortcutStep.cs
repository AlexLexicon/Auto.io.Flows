﻿using Auto.io.Flows.Application.Services;

namespace Auto.io.Flows.Application.Models.Steps;
public class CtrlCShortcutStep : IStep
{
    private readonly IKeyboardService _keyboardService;

    public CtrlCShortcutStep(IKeyboardService keyboardService)
    {
        _keyboardService = keyboardService;
    }

    public string Identifier => "CtrlCShortcut";
    public string Description => "Simulates a Ctrl+C keyboard shortcut";
    public IReadOnlyList<IParameter> Parameters => new List<IParameter>();

    public Task ExecuteAsync(IRunner runner, IEnumerable<object?> parameters)
    {
        _keyboardService.CtrlCShortcut();

        return Task.CompletedTask;
    }
}
