using System;

/// <summary>
/// Converts raw double values into human-readable German idle-game currency
/// strings, e.g. 4_200_000_000 → "4.2Mrd €".
///
/// Usage:
///   string display = NumberFormatter.Format(myDouble);
///   string display = NumberFormatter.FormatRate(incomePerSec);  // appends "/s"
/// </summary>
public static class NumberFormatter
{
    private static readonly (double Threshold, string Suffix)[] Tiers =
    {
        (1e21, "Trld"),
        (1e18, "Tri"),
        (1e15, "Bld"),
        (1e12, "Bio"),
        (1e9,  "Mrd"),
        (1e6,  "M"),
        (1e3,  "K"),
    };

    /// <summary>
    /// Formats a double as a German-style idle currency string with the € suffix.
    /// Returns "ERROR" for NaN/Infinity. Values below 1000 are shown as integers.
    /// </summary>
    public static string Format(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return "ERROR";

        if (value < 0)
            return $"-{Format(-value)}";

        foreach (var (threshold, suffix) in Tiers)
        {
            if (value >= threshold)
            {
                double scaled = value / threshold;
                // Show one decimal only when it adds information (e.g. 1.0 → "1K", 1.5 → "1.5K")
                string number = (scaled % 1.0 < 0.05) ? $"{(long)scaled}" : $"{scaled:F1}";
                return $"{number}{suffix} \u20ac";
            }
        }

        return $"{(long)value} \u20ac";
    }

    /// <summary>
    /// Same as Format() but appends "/s" — use for passive-income-rate display.
    /// </summary>
    public static string FormatRate(double valuePerSecond)
    {
        if (double.IsNaN(valuePerSecond) || double.IsInfinity(valuePerSecond))
            return "ERROR/s";

        return $"{Format(valuePerSecond).Replace(" \u20ac", "")}/s \u20ac";
    }

    /// <summary>
    /// Formats without the € symbol — use for XP or other non-currency values.
    /// </summary>
    public static string FormatRaw(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return "ERROR";

        if (value < 0)
            return $"-{FormatRaw(-value)}";

        foreach (var (threshold, suffix) in Tiers)
        {
            if (value >= threshold)
            {
                double scaled = value / threshold;
                string number = (scaled % 1.0 < 0.05) ? $"{(long)scaled}" : $"{scaled:F1}";
                return $"{number}{suffix}";
            }
        }

        return $"{(long)value}";
    }
}
