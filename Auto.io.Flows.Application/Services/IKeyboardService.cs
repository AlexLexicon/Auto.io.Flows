﻿namespace Auto.io.Flows.Application.Services;
public interface IKeyboardService
{
    void PressKey(string key);
    void ReleaseKey(string key);
    void SendText(string text);
    void CtrlCShortcut();
    void CtrlVShortcut();
    void SetClipboard(string value);
    void RegisterKeyPressed(Guid id, string key, Action handler);
    void RegisterKeyPressed(Guid id, string key, Func<Task> handlerAsync);
    void RegisterKeyReleased(Guid id, string key, Action handler);
    void RegisterKeyReleased(Guid id, string key, Func<Task> handlerAsync);
    void UnRegisterKeyPressed(Guid id);
    void UnRegisterKeyReleased(Guid id);
}
