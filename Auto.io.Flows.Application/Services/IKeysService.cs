namespace Auto.io.Flows.Application.Services;
public class IKeysService
{
    public const string KEYS_ESCAPE = "Escape";
    public const string KEYS_F1 = "F1";
    public const string KEYS_F2 = "F2";
    public const string KEYS_F3 = "F3";
    public const string KEYS_F4 = "F4";
    public const string KEYS_F5 = "F5";
    public const string KEYS_F6 = "F6";
    public const string KEYS_F7 = "F7";
    public const string KEYS_F8 = "F8";
    public const string KEYS_F9 = "F9";
    public const string KEYS_F10 = "F10";
    public const string KEYS_F11 = "F11";
    public const string KEYS_F12 = "F12";

    public const string KEYS_TILDE = "Tilde";
    public const string KEYS_1 = "1";
    public const string KEYS_2 = "2";
    public const string KEYS_3 = "3";
    public const string KEYS_4 = "4";
    public const string KEYS_5 = "5";
    public const string KEYS_6 = "6";
    public const string KEYS_7 = "7";
    public const string KEYS_8 = "8";
    public const string KEYS_9 = "9";
    public const string KEYS_0 = "0";
    public const string KEYS_SUBTRACT = "Subtract";
    public const string KEYS_EQUAL = "Equal";
    public const string KEYS_BACKSPACE = "Backspace";

    public const string KEYS_TAB = "Tab";
    public const string KEYS_Q = "q";
    public const string KEYS_W = "w";
    public const string KEYS_E = "e";
    public const string KEYS_R = "r";
    public const string KEYS_T = "t";
    public const string KEYS_Y = "y";
    public const string KEYS_U = "u";
    public const string KEYS_I = "i";
    public const string KEYS_O = "o";
    public const string KEYS_P = "p";
    public const string KEYS_BRACKET_OPEN = "[";
    public const string KEYS_BRACKET_CLOSE = "]";
    public const string KEYS_SLASH_BACK = "\\";

    public const string KEYS_CAPSLOCK = "Caps Lock";
    public const string KEYS_A = "a";
    public const string KEYS_S = "s";
    public const string KEYS_D = "d";
    public const string KEYS_F = "f";
    public const string KEYS_G = "g";
    public const string KEYS_H = "h";
    public const string KEYS_J = "j";
    public const string KEYS_K = "k";
    public const string KEYS_L = "l";
    public const string KEYS_SEMI_COLON = ";";
    public const string KEYS_QOUTE = "'";
    public const string KEYS_ENTER = "Enter";

    public const string KEYS_SHIFT_LEFT = "Left Shift";
    public const string KEYS_Z = "z";
    public const string KEYS_X = "x";
    public const string KEYS_C = "c";
    public const string KEYS_V = "v";
    public const string KEYS_B = "b";
    public const string KEYS_N = "n";
    public const string KEYS_M = "m";
    public const string KEYS_COMMA = ",";
    public const string KEYS_PERIOD = ".";
    public const string KEYS_SLASH_FORWARD = "/";
    public const string KEYS_SHIFT_RIGHT= "Right Shift";

    public const string KEYS_CONTROL_LEFT = "Left Control";
    public const string KEYS_ALT_LEFT = "Left Alt";
    public const string KEYS_SPACE = "Space";
    public const string KEYS_ALT_RIGHT = "Right Alt";
    public const string KEYS_CONTROL_RIGHT = "Right Control";

    public const string KEYS_UP = "Up";
    public const string KEYS_LEFT = "Left";
    public const string KEYS_DOWN = "Down";
    public const string KEYS_RIGHT = "Right";

    public static readonly IReadOnlyList<string> KEYS = new List<string>
    {
        KEYS_F4,KEYS_F1,KEYS_F2,KEYS_F3,KEYS_F5,KEYS_F6,KEYS_F7,KEYS_F8,KEYS_F9,KEYS_F10,KEYS_F11,KEYS_F12,KEYS_ESCAPE,
        KEYS_TILDE,KEYS_1,KEYS_2,KEYS_3,KEYS_4,KEYS_5,KEYS_6,KEYS_7,KEYS_8,KEYS_9,KEYS_0,KEYS_SUBTRACT,KEYS_EQUAL,KEYS_BACKSPACE,
        KEYS_TAB,KEYS_Q,KEYS_W,KEYS_E,KEYS_R,KEYS_T,KEYS_Y,KEYS_U,KEYS_I,KEYS_O,KEYS_P,KEYS_BRACKET_OPEN,KEYS_BRACKET_CLOSE,KEYS_SLASH_BACK,
        KEYS_CAPSLOCK,KEYS_A,KEYS_S,KEYS_D,KEYS_F,KEYS_G,KEYS_H,KEYS_J,KEYS_K,KEYS_L,KEYS_SEMI_COLON,KEYS_QOUTE,KEYS_ENTER,
        KEYS_SHIFT_LEFT,KEYS_Z,KEYS_X,KEYS_C,KEYS_V,KEYS_B,KEYS_N,KEYS_M,KEYS_COMMA,KEYS_PERIOD,KEYS_SLASH_FORWARD,KEYS_SHIFT_RIGHT,
        KEYS_CONTROL_LEFT,KEYS_ALT_LEFT,KEYS_SPACE,KEYS_ALT_RIGHT,KEYS_CONTROL_RIGHT,
        KEYS_UP,KEYS_LEFT,KEYS_DOWN,KEYS_RIGHT,
    };
}
