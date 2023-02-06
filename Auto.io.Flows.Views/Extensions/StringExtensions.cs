using Auto.io.Flows.Application.Services;
using System.Windows.Forms;

namespace Auto.io.Flows.Views.Extensions;
public static class StringExtensions
{
    public static Keys? ToWindowsFormsKey(this string? key)
    {
        return key switch
        {
            IKeysService.KEYS_ESCAPE => Keys.Escape,
            IKeysService.KEYS_F1 => Keys.F1,
            IKeysService.KEYS_F2 => Keys.F2,
            IKeysService.KEYS_F3 => Keys.F3,
            IKeysService.KEYS_F4 => Keys.F4,
            IKeysService.KEYS_F5 => Keys.F5,
            IKeysService.KEYS_F6 => Keys.F6,
            IKeysService.KEYS_F7 => Keys.F7,
            IKeysService.KEYS_F8 => Keys.F8,
            IKeysService.KEYS_F9 => Keys.F9,
            IKeysService.KEYS_F10 => Keys.F10,
            IKeysService.KEYS_F11 => Keys.F11,
            IKeysService.KEYS_F12 => Keys.F12,

            IKeysService.KEYS_TILDE => Keys.Oemtilde,
            IKeysService.KEYS_1 => Keys.D1,
            IKeysService.KEYS_2 => Keys.D2,
            IKeysService.KEYS_3 => Keys.D3,
            IKeysService.KEYS_4 => Keys.D4,
            IKeysService.KEYS_5 => Keys.D5,
            IKeysService.KEYS_6 => Keys.D6,
            IKeysService.KEYS_7 => Keys.D7,
            IKeysService.KEYS_8 => Keys.D8,
            IKeysService.KEYS_9 => Keys.D9,
            IKeysService.KEYS_0 => Keys.D0,
            IKeysService.KEYS_BACKSPACE => Keys.Back,

            IKeysService.KEYS_TAB => Keys.Tab,
            IKeysService.KEYS_Q => Keys.Q,
            IKeysService.KEYS_W => Keys.W,
            IKeysService.KEYS_E => Keys.E,
            IKeysService.KEYS_R => Keys.R,
            IKeysService.KEYS_T => Keys.T,
            IKeysService.KEYS_Y => Keys.Y,
            IKeysService.KEYS_U => Keys.U,
            IKeysService.KEYS_I => Keys.I,
            IKeysService.KEYS_O => Keys.O,
            IKeysService.KEYS_P => Keys.P,
            IKeysService.KEYS_BRACKET_OPEN => Keys.OemOpenBrackets,
            IKeysService.KEYS_BRACKET_CLOSE => Keys.OemCloseBrackets,
            IKeysService.KEYS_SLASH_BACK => Keys.OemBackslash,

            IKeysService.KEYS_CAPSLOCK => Keys.CapsLock,
            IKeysService.KEYS_A => Keys.A,
            IKeysService.KEYS_S => Keys.S,
            IKeysService.KEYS_D => Keys.D,
            IKeysService.KEYS_F => Keys.F,
            IKeysService.KEYS_G => Keys.G,
            IKeysService.KEYS_H => Keys.H,
            IKeysService.KEYS_J => Keys.J,
            IKeysService.KEYS_K => Keys.K,
            IKeysService.KEYS_L => Keys.L,
            IKeysService.KEYS_SEMI_COLON => Keys.OemSemicolon,
            IKeysService.KEYS_QOUTE => Keys.OemQuotes,
            IKeysService.KEYS_ENTER => Keys.Enter,

            IKeysService.KEYS_SHIFT_LEFT => Keys.LShiftKey,
            IKeysService.KEYS_Z => Keys.Z,
            IKeysService.KEYS_X => Keys.X,
            IKeysService.KEYS_C => Keys.C,
            IKeysService.KEYS_V => Keys.V,
            IKeysService.KEYS_B => Keys.B,
            IKeysService.KEYS_N => Keys.N,
            IKeysService.KEYS_M => Keys.M,
            IKeysService.KEYS_COMMA => Keys.Oemcomma,
            IKeysService.KEYS_PERIOD => Keys.OemPeriod,
            IKeysService.KEYS_SHIFT_RIGHT => Keys.RShiftKey,

            IKeysService.KEYS_CONTROL_LEFT => Keys.LControlKey,
            IKeysService.KEYS_ALT_LEFT => Keys.Alt,
            IKeysService.KEYS_SPACE => Keys.Space,
            IKeysService.KEYS_ALT_RIGHT => Keys.Alt,
            IKeysService.KEYS_CONTROL_RIGHT => Keys.RControlKey,

            _ => null,
        };
    }
}
