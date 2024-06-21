using System;

namespace F00F;

public static partial class Utils
{
    // https://www.somacon.com/p576.php
    public static string HumaniseBytes(ulong i, int digits = 2)
    {
        string suffix;
        double readable;
        if (i >= 0x1000000000000000) // Exabyte
        {
            suffix = "EB";
            readable = i >> 50;
        }
        else if (i >= 0x4000000000000) // Petabyte
        {
            suffix = "PB";
            readable = i >> 40;
        }
        else if (i >= 0x10000000000) // Terabyte
        {
            suffix = "TB";
            readable = i >> 30;
        }
        else if (i >= 0x40000000) // Gigabyte
        {
            suffix = "GB";
            readable = i >> 20;
        }
        else if (i >= 0x100000) // Megabyte
        {
            suffix = "MB";
            readable = i >> 10;
        }
        else if (i >= 0x400) // Kilobyte
        {
            suffix = "KB";
            readable = i;
        }
        else
        {
            return i.ToString("0 B"); // Byte
        }

        return $"{Round(readable / 1024)} {suffix}";

        double Round(double value)
            => Math.Round(value, digits, MidpointRounding.ToPositiveInfinity);
    }

    public static string HumaniseTime(ulong ms) => HumaniseTime(TimeSpan.FromMilliseconds(ms));
    public static string HumaniseTime(in TimeSpan ts)
    {
        return
            ts.TotalDays > 1 ? $"{ts.TotalDays:0.##}d" :
            ts.TotalHours > 1 ? $"{ts.TotalHours:0.##}h" :
            ts.TotalMinutes > 1 ? $"{ts.TotalMinutes:0.##}m" :
            ts.TotalSeconds > 1 ? $"{ts.TotalSeconds:0.##}s" :
            $"{ts.TotalMilliseconds:0.###}ms";
    }
}
