using System;

static public class FssSize
{
    // Function to turn a (potentitially large) byte size into a human-readable string (e.g. 1.2 GB)
    static public string SizeToString(long size)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        if (size == 0)
            return "0" + suf[0];

        long bytes = Math.Abs(size);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        
        return (Math.Sign(size) * num).ToString() + suf[place];
    }
}
