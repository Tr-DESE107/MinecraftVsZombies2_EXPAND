﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MVZ2.TalkData
{
    public class TalkScript
    {
        public string function;
        public string[] arguments;

        public static bool TryParse(string str, out TalkScript script)
        {
            script = null;
            str = str.Trim(' ', ';');
            var strings = str.Split(' ');
            if (strings.Length <= 0)
                return false;
            var function = strings[0];
            string[] arguments;
            if (strings.Length > 1)
            {
                arguments = strings.Skip(1).ToArray();
            }
            else
            {
                arguments = Array.Empty<string>();
            }
            script = new TalkScript()
            {
                function = function,
                arguments = arguments
            };
            return true;
        }
        public static TalkScript Parse(string str)
        {
            if (TryParse(str, out var parsed))
            {
                return parsed;
            }
            throw new FormatException($"Invalid TalkScript {str}.");
        }
        public static TalkScript[] ParseArray(string str)
        {
            return str?.Split(';')?.Select(s => Parse(s))?.ToArray();
        }
        public static TalkScript[] FromArrayXmlNode(XmlNode node)
        {
            List<TalkScript> scripts = new List<TalkScript>();
            var childNodes = node.ChildNodes;
            for (int i = 0; i < childNodes.Count; i++)
            {
                var child = childNodes[i];
                if (child.Name == "script")
                {
                    if (TryParse(child.InnerText, out var script))
                    {
                        scripts.Add(script);
                    }
                }
            }
            return scripts.ToArray();
        }
        public override string ToString()
        {
            if (arguments == null)
                return function;
            else
                return string.Join(" ", arguments.Prepend(function));
        }
    }
}
