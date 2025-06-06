﻿using System;
using System.Globalization;

namespace MVZ2Logic
{
    public static class ParseHelper
    {
        public static int ParseInt(string str)
        {
            return int.Parse(str, provider);
        }
        public static float ParseFloat(string str)
        {
            return float.Parse(str, provider);
        }
        public static bool TryParseInt(string str, out int parsed)
        {
            return int.TryParse(str, NumberStyles.Integer, provider, out parsed);
        }
        public static bool TryParseFloat(string str, out float parsed)
        {
            return float.TryParse(str, NumberStyles.Float, provider, out parsed);
        }
        private static IFormatProvider provider => CultureInfo.InvariantCulture;
    }
}
