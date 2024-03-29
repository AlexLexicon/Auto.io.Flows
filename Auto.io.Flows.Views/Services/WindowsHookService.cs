﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Auto.io.Flows.Views.Services;
//based on https://stackoverflow.com/questions/604410/global-keyboard-capture-in-c-sharp-application
public class WindowsHookService : IDisposable
{
    delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    public const int LlkhfAltdown = KfAltdown >> 8;
    public const int WH_KEYBOARD_LL = 13;
    private const int KfAltdown = 0x2000;

    public enum KeyboardState
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        SysKeyDown = 0x0104,
        SysKeyUp = 0x0105
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LowLevelKeyboardInputEvent
    {
        /// <summary>
        /// A virtual-key code. The code must be a value in the range 1 to 254.
        /// </summary>
        public int VirtualCode;

        // EDT: added a conversion from VirtualCode to Keys.
        /// <summary>
        /// The VirtualCode converted to typeof(Keys) for higher usability.
        /// </summary>
        public Keys Key { get { return (Keys)VirtualCode; } }

        /// <summary>
        /// A hardware scan code for the key. 
        /// </summary>
        public int HardwareScanCode;

        /// <summary>
        /// The extended-key flag, event-injected Flags, context code, and transition-state flag. This member is specified as follows. An application can use the following values to test the keystroke Flags. Testing LLKHF_INJECTED (bit 4) will tell you whether the event was injected. If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
        /// </summary>
        public int Flags;

        /// <summary>
        /// The time stamp stamp for this message, equivalent to what GetMessageTime would return for this message.
        /// </summary>
        public int TimeStamp;

        /// <summary>
        /// Additional information associated with the message. 
        /// </summary>
        public IntPtr AdditionalInformation;
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool FreeLibrary(IntPtr hModule);
    /// <summary>
    /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
    /// You would install a hook procedure to monitor the system for certain types of events. These events are
    /// associated either with a specific thread or with all threads in the same desktop as the calling thread.
    /// </summary>
    /// <param name="idHook">hook type</param>
    /// <param name="lpfn">hook procedure</param>
    /// <param name="hMod">handle to application instance</param>
    /// <param name="dwThreadId">thread identifier</param>
    /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
    [DllImport("USER32", SetLastError = true)]
    static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);
    /// <summary>
    /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
    /// </summary>
    /// <param name="hhk">handle to hook procedure</param>
    /// <returns>If the function succeeds, the return value is true.</returns>
    [DllImport("USER32", SetLastError = true)]
    public static extern bool UnhookWindowsHookEx(IntPtr hHook);

    /// <summary>
    /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
    /// A hook procedure can call this function either before or after processing the hook information.
    /// </summary>
    /// <param name="hHook">handle to current hook</param>
    /// <param name="code">hook code passed to hook procedure</param>
    /// <param name="wParam">value passed to hook procedure</param>
    /// <param name="lParam">value passed to hook procedure</param>
    /// <returns>If the function succeeds, the return value is true.</returns>
    [DllImport("USER32", SetLastError = true)]
    static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

    // EDT: Replaced VkSnapshot(int) with RegisteredKeys(Keys[])
    public static Keys[]? RegisteredKeys { get; set; }

    public event Action<EventArgs>? KeyboardPressed;

    private IntPtr _windowsHookHandle;
    private IntPtr _user32LibraryHandle;
    private HookProc _hookProc;

    public WindowsHookService()
    {
        //Added an optional parameter (registeredKeys) that accepts keys to restict the logging mechanism.
        //Keys that should trigger logging. Pass null for full logging.
        RegisteredKeys = null;

        _windowsHookHandle = IntPtr.Zero;
        _user32LibraryHandle = IntPtr.Zero;

        // we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.
        _hookProc = LowLevelKeyboardProc;

        _user32LibraryHandle = LoadLibrary("User32");
        if (_user32LibraryHandle == IntPtr.Zero)
        {
            int errorCode = Marshal.GetLastWin32Error();

            throw new Win32Exception(errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
        }

        _windowsHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);
        if (_windowsHookHandle == IntPtr.Zero)
        {
            int errorCode = Marshal.GetLastWin32Error();

            throw new Win32Exception(errorCode, $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
        }
    }

    public IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        bool fEatKeyStroke = false;

        var wparamTyped = wParam.ToInt32();
        if (Enum.IsDefined(typeof(KeyboardState), wparamTyped))
        {
            object? lowLevelKeyboardInputEventObject = Marshal.PtrToStructure(lParam, typeof(LowLevelKeyboardInputEvent));

            if (lowLevelKeyboardInputEventObject is not null)
            {
                LowLevelKeyboardInputEvent lowLevelKeyboardInputEvent = (LowLevelKeyboardInputEvent)lowLevelKeyboardInputEventObject;

                var eventArguments = new EventArgs
                {
                    KeyboardData = lowLevelKeyboardInputEvent,
                    KeyboardState = (KeyboardState)wparamTyped,
                };

                // EDT: Removed the comparison-logic from the usage-area so the user does not need to mess around with it.
                // Either the incoming key has to be part of RegisteredKeys (see constructor on top) or RegisterdKeys
                // has to be null for the event to get fired.
                var key = (Keys)lowLevelKeyboardInputEvent.VirtualCode;
                if (RegisteredKeys is null || RegisteredKeys.Contains(key))
                {
                    KeyboardPressed?.Invoke(eventArguments);

                    fEatKeyStroke = eventArguments.Handled;
                }
            }
        }

        return fEatKeyStroke ? 1 : CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~WindowsHookService()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // because we can unhook only in the same thread, not in garbage collector thread
            if (_windowsHookHandle != IntPtr.Zero)
            {
                if (!UnhookWindowsHookEx(_windowsHookHandle))
                {
                    int errorCode = Marshal.GetLastWin32Error();

                    throw new Win32Exception(errorCode, $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                }
                _windowsHookHandle = IntPtr.Zero;

                // ReSharper disable once DelegateSubtraction
                _hookProc -= LowLevelKeyboardProc;
            }
        }

        if (_user32LibraryHandle != IntPtr.Zero)
        {
            if (!FreeLibrary(_user32LibraryHandle)) // reduces reference to library by 1.
            {
                int errorCode = Marshal.GetLastWin32Error();

                throw new Win32Exception(errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }
            _user32LibraryHandle = IntPtr.Zero;
        }
    }

    public class EventArgs : HandledEventArgs
    {
        public required LowLevelKeyboardInputEvent KeyboardData { get; init; }
        public required KeyboardState KeyboardState { get; init; }
    }
}
