using Auto.io.Flows.Application.Services;
using Auto.io.Flows.Views.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto.io.Flows.Views.Services;
public class WindowsKeyboardService : IKeyboardService
{
    private readonly WindowsHookService _windowsHookService;
    private readonly Dictionary<Guid, KeyEventHandler> _keyPressedHandlers;
    private readonly Dictionary<Guid, KeyEventHandler> _keyReleasedHandlers;

    public WindowsKeyboardService(WindowsHookService windowsHookService)
    {
        _windowsHookService = windowsHookService;
        _windowsHookService.KeyboardPressed += _windowsHookService_KeyboardPressed;

        _keyPressedHandlers = new Dictionary<Guid, KeyEventHandler>();
        _keyReleasedHandlers = new Dictionary<Guid, KeyEventHandler>();
    }

    private async void _windowsHookService_KeyboardPressed(WindowsHookService.EventArgs e)
    {
        Keys key = e.KeyboardData.Key;

        List<KeyEventHandler>? handlers = null;
        if (e.KeyboardState == WindowsHookService.KeyboardState.KeyDown)
        {
            handlers = _keyPressedHandlers
                .Where(kvp => kvp.Value.Key == key)
                .Select(kvp => kvp.Value)
                .ToList();
        }
        else if (e.KeyboardState == WindowsHookService.KeyboardState.KeyUp)
        {
            handlers = _keyReleasedHandlers
                .Where(kvp => kvp.Value.Key == key)
                .Select(kvp => kvp.Value)
                .ToList();
        }

        if (handlers is not null)
        {
            foreach (KeyEventHandler handler in handlers)
            {
                await handler.ActionHandler.Invoke();
            }
        }
    }

    public void PressKey(string key)
    {

    }

    public void ReleaseKey(string key)
    {

    }

    public void UnRegisterKeyPressed(Guid id)
    {
        _keyPressedHandlers.Remove(id);
    }
    public void RegisterKeyPressed(Guid id, string key, Action handler)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handler);

        RegisterKeyPressed(id, key, () =>
        {
            handler.Invoke();
            return Task.CompletedTask;
        });
    }
    public void RegisterKeyPressed(Guid id, string key, Func<Task> handlerAsync)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handlerAsync);

        Keys? formsKey = key.ToWindowsFormsKey();

        if (formsKey is null)
        {
            throw new Exception($"Failed to get the windows forms key for the key '{key}'.");
        }

        if (_keyPressedHandlers.TryGetValue(id, out KeyEventHandler? keyEventHandler))
        {
            keyEventHandler.Key = formsKey.Value;
            keyEventHandler.ActionHandler = handlerAsync;
        }
        else
        {
            _keyPressedHandlers.Add(id, new KeyEventHandler
            {
                Key = formsKey.Value,
                ActionHandler = handlerAsync,
            });
        }
    }

    public void UnRegisterKeyReleased(Guid id)
    {
        _keyReleasedHandlers.Remove(id);
    }
    public void RegisterKeyReleased(Guid id, string key, Action handler)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handler);

        RegisterKeyReleased(id, key, () =>
        {
            handler.Invoke();
            return Task.CompletedTask;
        });
    }
    public void RegisterKeyReleased(Guid id, string key, Func<Task> handlerAsync)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handlerAsync);

        Keys? formsKey = key.ToWindowsFormsKey();

        if (formsKey is null)
        {
            throw new Exception($"Failed to get the windows forms key for the key '{key}'.");
        }

        if (_keyReleasedHandlers.TryGetValue(id, out KeyEventHandler? keyEventHandler))
        {
            keyEventHandler.Key = formsKey.Value;
            keyEventHandler.ActionHandler = handlerAsync;
        }
        else
        {
            _keyReleasedHandlers.Add(id, new KeyEventHandler
            {
                Key = formsKey.Value,
                ActionHandler = handlerAsync,
            });
        }
    }

    private class KeyEventHandler
    {
        public required Keys Key { get; set; }
        public required Func<Task> ActionHandler { get; set; }
    }
}
