using System;
using System.Collections.Generic;

// FssColor: A platform independent class to represent a color with floating point values for RGBA.

public class FssColor
{
    // Fields are to enforce immutability
    public float R { get; }
    public float G { get; }
    public float B { get; }
    public float A { get; }

    // Constructor
    public FssColor(float r, float g, float b, float a = 1.0f)
    {
        this.R = Clamp(r);
        this.G = Clamp(g);
        this.B = Clamp(b);
        this.A = Clamp(a);
    }

    // Methods for getting the integer values of the colors
    public int RInt => ToInt(R);
    public int GInt => ToInt(G);
    public int BInt => ToInt(B);
    public int AInt => ToInt(A);

    // Hex code output
    public string ToHex(bool includeAlpha = false)
    {
        return includeAlpha 
            ? $"{RInt:X2}{GInt:X2}{BInt:X2}{AInt:X2}" 
            : $"{RInt:X2}{GInt:X2}{BInt:X2}";
    }
   
    // ------------------------------------------------------------------------------------------------------------------------------
    // #MARK: Helper methods
    // ------------------------------------------------------------------------------------------------------------------------------

    private static float Clamp(float value) => Math.Clamp(value, 0f, 1f);
    private static int ToInt(float value) => (int)(Clamp(value) * 255);

    // ToString override for debugging
    public override string ToString() => $"RGBA({R:F2}, {G:F2}, {B:F2}, {A:F2})";

    public static FssColor Lerp(FssColor color1, FssColor color2, float t)
    {
        byte r = (byte)(color1.R + t * (color2.R - color1.R));
        byte g = (byte)(color1.G + t * (color2.G - color1.G));
        byte b = (byte)(color1.B + t * (color2.B - color1.B));
        byte a = (byte)(color1.A + t * (color2.A - color1.A));

        return new FssColor(r, g, b, a);
    }
    
    // Static dictionary of predefined colors
    public static readonly Dictionary<string, FssColor> PredefinedColors = new Dictionary<string, FssColor>
    {
        // Main colors: 1s and 0s.
        {"Red",     new FssColor(1, 0, 0)},
        {"Green",   new FssColor(0, 1, 0)},
        {"Blue",    new FssColor(0, 0, 1)},
        {"Yellow",  new FssColor(1, 1, 0)},
        {"Cyan",    new FssColor(0, 1, 1)},
        {"Magenta", new FssColor(1, 0, 1)},

        // Monochrome
        {"Black",     new FssColor(0, 0, 0)},
        {"NearBlack", new FssColor(0.05f, 0.05f, 0.05f)},
        {"DarkGray",  new FssColor(0.25f, 0.25f, 0.25f)},
        {"MidGray",   new FssColor(0.5f,  0.5f,  0.5f)},
        {"LightGray", new FssColor(0.75f, 0.75f, 0.75f)},
        {"OffWhite",  new FssColor(0.95f, 0.95f, 0.95f)},
        {"White",     new FssColor(1, 1, 1)},
        
        // Secondary colors, strictly using 1s, 0.5s, and 0s.
        {"LightRed",    new FssColor(1, 0.5f, 0.5f)},
        {"DarkRed",     new FssColor(0.5f, 0, 0)},
        {"SpringGreen", new FssColor(0, 1f, 0.5f)},
        {"LightOlive",  new FssColor(0.5f, 1, 0)},
        {"LightGreen",  new FssColor(0.5f, 1, 0.5f)},
        {"DarkGreen",   new FssColor(0, 0.5f, 0)},
        {"PaleCyan",    new FssColor(0.5f, 1, 1)},
        {"LightBlue",   new FssColor(0.5f, 0.5f, 1)},
        {"DarkBlue",    new FssColor(0, 0, 0.5f)},
        {"Orange",      new FssColor(1, 0.5f, 0)},
        {"Olive",       new FssColor(0.5f, 0.5f, 0)},
        {"Teal",        new FssColor(0, 0.5f, 0.5f)},
        {"Purple",      new FssColor(0.5f, 0, 0.5f)},
        {"DeepPurple",  new FssColor(0.5f, 0, 1)},
        {"PaleYellow",  new FssColor(1, 1, 0.5f)},
        {"PaleMagenta", new FssColor(1, 0.5f, 1)},
        {"SoftPink",    new FssColor(1, 0.5f, 1)},

        // Reamining colors, any fractions.
        {"Salmon",        new FssColor(0.98f, 0.50f, 0.45f)},
        {"Crimson",       new FssColor(0.86f, 0.08f, 0.24f)},
        {"Pink",          new FssColor(1f, 0.75f, 0.80f)},
        {"HotPink",       new FssColor(1f, 0.41f, 0.71f)},
        {"DeepPink",      new FssColor(1f, 0.08f, 0.58f)},
        {"OrangeRed",     new FssColor(1f, 0.27f, 0)},
        {"DarkOrange",    new FssColor(1f, 0.55f, 0)},
        {"Gold",          new FssColor(1f, 0.84f, 0)},
        {"Khaki",         new FssColor(0.94f, 0.90f, 0.55f)},
        {"Lavender",      new FssColor(0.90f, 0.90f, 0.98f)},
        {"Thistle",       new FssColor(0.85f, 0.75f, 0.85f)},
        {"Plum",          new FssColor(0.87f, 0.63f, 0.87f)},
        {"Violet",        new FssColor(0.93f, 0.51f, 0.93f)},
        {"Orchid",        new FssColor(0.85f, 0.44f, 0.84f)},
        {"Azure",         new FssColor(0.94f, 1f, 1f)},
        {"LightCyan",     new FssColor(0.88f, 1f, 1f)},
        {"PaleTurquoise", new FssColor(0.69f, 0.93f, 0.93f)},
        {"Aquamarine",    new FssColor(0.50f, 1f, 0.83f)},
        {"Turquoise",     new FssColor(0.25f, 0.88f, 0.82f)},
        {"MidnightBlue",  new FssColor(0.10f, 0.10f, 0.44f)},
        {"DodgerBlue",    new FssColor(0.12f, 0.56f, 1f)},
        {"LightSeaGreen", new FssColor(0.13f, 0.70f, 0.67f)},
        {"ForestGreen",   new FssColor(0.13f, 0.55f, 0.13f)},
        {"OliveDrab",     new FssColor(0.42f, 0.56f, 0.14f)},
        {"LimeGreen",     new FssColor(0.20f, 0.80f, 0.20f)},
        {"LightGreen",    new FssColor(0.56f, 0.93f, 0.56f)},
        {"PaleGreen",     new FssColor(0.60f, 0.98f, 0.60f)}        
    };
}
