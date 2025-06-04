using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public static class FssCentralLog
{
    private static string runtimeFilename;
    private static ConcurrentQueue<string> logEntries = new ConcurrentQueue<string>();
    private static readonly object fileLock = new object();
    private static readonly int maxEntries = 100;

    static FssCentralLog()
    {
        runtimeFilename = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
    }

    public static void AddEntry(string entry)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp} : {entry}";
        
        logEntries.Enqueue(logEntry);

        // Keep the logEntries within the maxEntries limit
        while (logEntries.Count > maxEntries)
        {
            logEntries.TryDequeue(out _);
        }

        // Write the entry to the file
        Task.Run(() => AppendToFile(logEntry));
    }

    public static string GetLatestLines(int numLines)
    {
        var latestEntries = logEntries.ToArray();
        int startIndex = Math.Max(0, latestEntries.Length - numLines);
        return string.Join("\n", latestEntries[startIndex..]);
    }

    private static void AppendToFile(string entry)
    {
        lock (fileLock)
        {
            try
            {
                File.AppendAllText(runtimeFilename, entry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Alternative logging, e.g., to console or a different file
                Console.WriteLine($"Error writing to log file: {ex.Message}");
                // Optionally, write to a different file or system log
            }
        }
    }
}
