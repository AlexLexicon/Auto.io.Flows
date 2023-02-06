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

    public void KeyPressed(Guid id, string key, Action? handler)
    {
        if (handler is null)
        {
            KeyPressed(id, key, handlerAsync: null);
        }
        else
        {
            KeyPressed(id, key, () =>
            {
                handler.Invoke();
                return Task.CompletedTask;
            });
        }
    }
    public void KeyPressed(Guid id, string key, Func<Task>? handlerAsync)
    {
        Keys? formsKey = key.ToWindowsFormsKey();

        if (formsKey is null)
        {
            throw new Exception($"Failed to get the windows forms key for the key '{key}'.");
        }

        if (_keyPressedHandlers.TryGetValue(id, out KeyEventHandler? keyEventHandler))
        {
            if (handlerAsync is null)
            {
                if (keyEventHandler.Key == formsKey.Value)
                {
                    _keyPressedHandlers.Remove(id);
                }
                else
                {
                    keyEventHandler.Key = formsKey.Value;
                }
            }
            else
            {
                keyEventHandler.Key = formsKey.Value;
                keyEventHandler.ActionHandler = handlerAsync;
            }
        }
        else if (handlerAsync is not null)
        {
            _keyPressedHandlers.Add(id, new KeyEventHandler
            {
                Key = formsKey.Value,
                ActionHandler = handlerAsync,
            });
        }
    }

    public void KeyReleased(Guid id, string key, Action? handler)
    {
        if (handler is null)
        {
            KeyReleased(id, key, handlerAsync: null);
        }
        else
        {
            KeyReleased(id, key, () =>
            {
                handler.Invoke();
                return Task.CompletedTask;
            });
        }
    }
    public void KeyReleased(Guid id, string key, Func<Task>? handlerAsync)
    {
        Keys? formsKey = key.ToWindowsFormsKey();

        if (formsKey is null)
        {
            throw new Exception($"Failed to get the windows forms key for the key '{key}'.");
        }

        if (_keyReleasedHandlers.TryGetValue(id, out KeyEventHandler? keyEventHandler))
        {
            if (handlerAsync is null)
            {
                if (keyEventHandler.Key == formsKey.Value)
                {
                    _keyReleasedHandlers.Remove(id);
                }
                else
                {
                    keyEventHandler.Key = formsKey.Value;
                }
            }
            else
            {
                keyEventHandler.Key = formsKey.Value;
                keyEventHandler.ActionHandler = handlerAsync;
            }
        }
        else if (handlerAsync is not null)
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
