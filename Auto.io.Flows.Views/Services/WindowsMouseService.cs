using Auto.io.Flows.Application.Services;
using System.Runtime.InteropServices;

namespace Auto.io.Flows.Views.Services;
public class WindowsMouseService : IMouseService
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
    //Mouse actions
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    public (int x, int y) GetPosition()
    {
        return (System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
    }

    public void Move(int x, int y)
    {
        System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);
    }

    public void PressLeftMouseButton()
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, 0);
    }
    public void ReleaseLeftMouseButton()
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
    }

    public void PressRightMouseButton()
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)x, (uint)y, 0, 0);
    }
    public void ReleaseRightMouseButton()
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_RIGHTUP, (uint)x, (uint)y, 0, 0);
    }
}
