namespace Auto.io.Flows.Application.Services;
public interface IMouseService
{
    public const string MOUSE_BUTTONS_LEFT = "Left";
    public const string MOUSE_BUTTONS_RIGHT = "Right";
    public readonly static IReadOnlyList<string> MOUSE_BUTTONS = new List<string>
    {
        MOUSE_BUTTONS_LEFT,
        MOUSE_BUTTONS_RIGHT,
    };

    (int x, int y) GetPosition();
    void Move(int x, int y);
    void PressLeftMouseButton();
    void ReleaseLeftMouseButton();
    void PressRightMouseButton();
    void ReleaseRightMouseButton();
}
