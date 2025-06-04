
// FssKeystroke.cs - A platform independent class for keystrokes.

public class FssKeystroke
{
    public string KeyRaw  { get; } = ""; // Raw key code, like 5, even if shifted pressed
    public string KeyChar { get; } = ""; // Modifier character, like "%" for shift-5
    public bool   Shift   { get; } = false;
    public bool   Alt     { get; } = false;
    public bool   Ctrl    { get; } = false;

    public FssKeystroke(string keyR, string keyC, bool shift, bool alt, bool ctrl)
    {
        KeyRaw   = keyR;
        KeyChar  = keyC;
        Shift    = shift;
        Alt      = alt;
        Ctrl     = ctrl;
    }

    public override string ToString()
    {
        // List the various keys and modifiers, join with + characters
        var parts = new List<string>();
        if (Shift) parts.Add("<shift>");
        if (Alt)   parts.Add("<alt>");
        if (Ctrl)  parts.Add("<ctrl>");
        if (!string.IsNullOrEmpty(KeyRaw)) parts.Add(KeyRaw);
        string KeyPress = string.Join("+", parts);

        // Add the printable character if it exists
        if (!string.IsNullOrEmpty(KeyChar) && IsPrintable(KeyChar))
            KeyPress += $" ({KeyChar})";

        return KeyPress;
    }

    // Check if the character is a printable ASCII character
    private bool IsPrintable(string keyChar)
    {
        if (string.IsNullOrEmpty(keyChar)) return false;
        char c = keyChar[0];
        return !char.IsControl(c);
    }
} 



