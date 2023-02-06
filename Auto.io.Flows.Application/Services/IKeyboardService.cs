namespace Auto.io.Flows.Application.Services;
public interface IKeyboardService
{
    void PressKey(string key);
    void ReleaseKey(string key);
    void KeyPressed(Guid id, string key, Action? handler);
    void KeyReleased(Guid id, string key, Action? handler);
}
