using Auto.io.Flows.Application.Services;
using System.Windows.Input;

namespace Auto.io.Flows.Views.Extensions;
public static class KeyExtensions
{
    public static string ToWindowsInputKeyString(this Key key)
    {
        return key switch
        {
            Key.Escape => IKeysService.KEYS_ESCAPE,
            Key.F1 => IKeysService.KEYS_F1,
            Key.F2 => IKeysService.KEYS_F2,
            Key.F3 => IKeysService.KEYS_F3,
            Key.F4 => IKeysService.KEYS_F4,
            Key.F5 => IKeysService.KEYS_F5,
            Key.F6 => IKeysService.KEYS_F6,
            Key.F7 => IKeysService.KEYS_F7,
            Key.F8 => IKeysService.KEYS_F8,
            Key.F9 => IKeysService.KEYS_F9,
            Key.F10 => IKeysService.KEYS_F10,
            Key.F11 => IKeysService.KEYS_F11,
            Key.F12 => IKeysService.KEYS_F12,

            Key.OemTilde => IKeysService.KEYS_TILDE,
            Key.D1 => IKeysService.KEYS_1,
            Key.D2 => IKeysService.KEYS_2,
            Key.D3 => IKeysService.KEYS_3,
            Key.D4 => IKeysService.KEYS_4,
            Key.D5 => IKeysService.KEYS_5,
            Key.D6 => IKeysService.KEYS_6,
            Key.D7 => IKeysService.KEYS_7,
            Key.D8 => IKeysService.KEYS_8,
            Key.D9 => IKeysService.KEYS_9,
            Key.D0 => IKeysService.KEYS_0,
            Key.Back => IKeysService.KEYS_BACKSPACE,

            Key.Tab => IKeysService.KEYS_TAB,
            Key.Q => IKeysService.KEYS_Q,
            Key.W => IKeysService.KEYS_W,
            Key.E => IKeysService.KEYS_E,
            Key.R => IKeysService.KEYS_R,
            Key.T => IKeysService.KEYS_T,
            Key.Y => IKeysService.KEYS_Y,
            Key.U => IKeysService.KEYS_U,
            Key.I => IKeysService.KEYS_I,
            Key.O => IKeysService.KEYS_O,
            Key.P => IKeysService.KEYS_P,
            Key.OemOpenBrackets => IKeysService.KEYS_BRACKET_OPEN,
            Key.OemCloseBrackets => IKeysService.KEYS_BRACKET_CLOSE,
            Key.OemBackslash => IKeysService.KEYS_SLASH_BACK,

            Key.CapsLock => IKeysService.KEYS_CAPSLOCK,
            Key.A => IKeysService.KEYS_A,
            Key.S => IKeysService.KEYS_S,
            Key.D => IKeysService.KEYS_D,
            Key.F => IKeysService.KEYS_F,
            Key.G => IKeysService.KEYS_G,
            Key.H => IKeysService.KEYS_H,
            Key.J => IKeysService.KEYS_J,
            Key.K => IKeysService.KEYS_K,
            Key.L => IKeysService.KEYS_L,
            Key.OemSemicolon => IKeysService.KEYS_SEMI_COLON,
            Key.OemQuotes => IKeysService.KEYS_QOUTE,
            Key.Enter => IKeysService.KEYS_ENTER,

            Key.LeftShift => IKeysService.KEYS_SHIFT_LEFT,
            Key.Z => IKeysService.KEYS_Z,
            Key.X => IKeysService.KEYS_X,
            Key.C => IKeysService.KEYS_C,
            Key.V => IKeysService.KEYS_V,
            Key.B => IKeysService.KEYS_B,
            Key.N => IKeysService.KEYS_N,
            Key.M => IKeysService.KEYS_M,
            Key.OemComma => IKeysService.KEYS_COMMA,
            Key.OemPeriod => IKeysService.KEYS_PERIOD,
            Key.RightShift => IKeysService.KEYS_SHIFT_RIGHT,

            Key.LeftCtrl => IKeysService.KEYS_CONTROL_LEFT,
            Key.LeftAlt => IKeysService.KEYS_ALT_LEFT,
            Key.Space => IKeysService.KEYS_SPACE,
            Key.RightAlt => IKeysService.KEYS_ALT_RIGHT,
            Key.RightCtrl => IKeysService.KEYS_CONTROL_RIGHT,

            Key.Up => IKeysService.KEYS_UP,
            Key.Left => IKeysService.KEYS_LEFT,
            Key.Down => IKeysService.KEYS_DOWN,
            Key.Right => IKeysService.KEYS_RIGHT,

            _ => string.Empty,
        };
    }
}
