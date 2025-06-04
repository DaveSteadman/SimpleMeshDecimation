using SkiaSharp;
// static class listin a load of colors we would want to use

public static class FssSkiaPalette
{
    // Random numbers


    // Usage: FssSkiaPalette.TransFillColors[0]
    // FssSkiaPalette.TransFillColors.Length
    public static SKColor[] TransFillColors = new SKColor[]
    {
        SKColors.Red.WithAlpha(128),
        SKColors.Green.WithAlpha(128),
        SKColors.Blue.WithAlpha(128),
        SKColors.Yellow.WithAlpha(128),
        SKColors.Cyan.WithAlpha(128),
        SKColors.Magenta.WithAlpha(128),
        SKColors.Black.WithAlpha(128),
        SKColors.White.WithAlpha(128),
        SKColors.Gray.WithAlpha(128),
        SKColors.LightGray.WithAlpha(128),
        SKColors.DarkGray.WithAlpha(128),
        SKColors.Brown.WithAlpha(128),
        SKColors.Orange.WithAlpha(128),
        SKColors.Purple.WithAlpha(128),
        SKColors.Pink.WithAlpha(128),
        SKColors.Lime.WithAlpha(128)
    };

    // Usage: FssSkiaPalette.RandomFillColor()
    public static SKColor RandomFillColor()
    {
        // Generate a light color - capped at 150 to avoid creating a white
        Random random = new Random();
        byte randomR = (byte)random.Next(20, 150);
        byte randomG = (byte)random.Next(20, 150);
        byte randomB = (byte)random.Next(20, 150);
        byte randomA = (byte)random.Next(100, 150);
        SKColor color = new SKColor(randomR, randomG, randomB, randomA);

        return color;
    }

    // Create a palette of bold, highly saturated but slightly off-beat colors
    // Usage: FssSkiaPalette.PaletteColors[0] (see comments for color names)
    public static List<SKColor> PaletteColors = new List<SKColor>
    {
        new SKColor(200, 0, 0, 255),   // Main Red
        new SKColor(120, 0, 0, 255),   // Dark Red
        new SKColor(0, 170, 0, 255),   // Main Green
        new SKColor(0, 100, 0, 255),   // Dark Green
        new SKColor(0, 0, 180, 255),   // Main Blue
        new SKColor(0, 0, 100, 255),   // Dark Blue
        new SKColor(180, 180, 0, 255), // Main Yellow
        new SKColor(100, 100, 0, 255), // Dark Yellow
        new SKColor(0, 180, 180, 255), // Main Cyan
        new SKColor(0, 100, 100, 255), // Dark Cyan
        new SKColor(180, 0, 180, 255), // Main Magenta
        new SKColor(100, 0, 100, 255), // Dark Magenta
        new SKColor(220, 165, 0, 255), // Main Orange
        new SKColor(165, 110, 0, 255)  // Dark Orange
    };

    public static SKColor RandomPaletteColor()
    {
        Random random = new Random();
        int index = random.Next(PaletteColors.Count);
        return PaletteColors[index];
    }
}




