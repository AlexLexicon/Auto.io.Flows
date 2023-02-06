using Auto.io.Flows.Application.Services;
using System;
using System.Runtime.InteropServices;

namespace Auto.io.Flows.Views.Services;
public class WindowsMouseService : IMouseService
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, IntPtr dwExtraInfo);
    //Mouse actions
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;
    private const int MOUSEEVENTF_WHEEL = 0x0800;

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

        mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
    }
    public void ReleaseLeftMouseButton()
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
    }

    public void PressRightMouseButton()
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
    }
    public void ReleaseRightMouseButton()
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
    }

    public void ScrollMouseButton(int amount)
    {
        (int x, int y) = GetPosition();

        mouse_event(MOUSEEVENTF_WHEEL, x, y, amount, 0);
    }
}
