using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Resources;
using PVZEngine;
using UnityEngine;

namespace MVZ2
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
        public static bool TryParseVector2(string str, out Vector2 parsed)
        {
            parsed = Vector2.zero;
            var split = str.Split(',');
            if (split.Length != 2)
                return false;
            if (!TryParseFloat(split[0], out var x) || !TryParseFloat(split[1], out var y))
                return false;
            parsed = new Vector2(x, y);
            return true;
        }
        public static bool TryParseVector3(string str, out Vector3 parsed)
        {
            parsed = Vector3.zero;
            var split = str.Split(',');
            if (split.Length != 3)
                return false;
            if (!TryParseFloat(split[0], out var x) || !TryParseFloat(split[1], out var y) || !TryParseFloat(split[2], out var z))
                return false;
            parsed = new Vector3(x, y, z);
            return true;
        }
        private static IFormatProvider provider => CultureInfo.InvariantCulture;
    }
}
