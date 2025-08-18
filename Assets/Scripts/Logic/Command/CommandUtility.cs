using System;
using System.Collections.Generic;
using System.Text;

namespace MVZ2Logic.Command
{
    public static class CommandUtility
    {
        public static string[] SplitCommand(string command)
        {
            if (!command.StartsWith(COMMAND_CHARACTER))
                return Array.Empty<string>();
            command = command.Substring(1);

            List<string> parts = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < command.Length; i++)
            {
                char c = command[i];
                if (char.IsWhiteSpace(c))
                {
                    parts.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            parts.Add(sb.ToString());
            sb.Clear();

            return parts.ToArray();
        }
        public static float ParseOptionalFloat(string text, float defaultValue)
        {
            if (text == DEFAULT_VALUE_PARAMETER)
            {
                return defaultValue;
            }
            return ParseHelper.ParseFloat(text);
        }
        public const char COMMAND_CHARACTER = '/';
        public const string DEFAULT_VALUE_PARAMETER = "~";
    }
}
