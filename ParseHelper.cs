// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 TheMonHub

using System.Text.RegularExpressions;

namespace JimmyRewrite;

public static partial class ParseHelper
{
    public static int[]? ParseRuleIds(string ruleIds)
    {
        try
        {
            var ids = ruleIds.Split(',').Select(int.Parse);
            var nonZeroNonNegativeNonDuplicateIds = ids.Distinct().Where(id => id > 0);
            return [.. nonZeroNonNegativeNonDuplicateIds];
        }
        catch
        {
            return null;
        }
    }

    [GeneratedRegex(@"(\d+)([dhms])", RegexOptions.IgnoreCase)]
    private static partial Regex DurationRegex();
    
    public static long? ParseDuration(string input)
    {
        MatchCollection matches;
        try
        {
            matches = DurationRegex().Matches(input);
        }
        catch
        {
            return null;
        }

        int days = 0, hours = 0, minutes = 0;

        foreach (Match match in matches)
        {
            var value = int.Parse(match.Groups[1].Value);
            var unit = match.Groups[2].Value.ToLower();

            switch (unit)
            {
                case "d": days = value; break;
                case "h": hours = value; break;
                case "m": minutes = value; break;
                case "s":
                    return null;
            }
        }

        return days * 24 * 60 + hours * 60 + minutes;
    }
}