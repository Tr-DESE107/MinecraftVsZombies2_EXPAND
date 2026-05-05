#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MVZ2.IO;
using MVZ2.Metas;
using MVZ2.TalkData;
using MVZ2.Vanilla;
using MVZ2Logic.Resources;
using NUnit.Framework.Interfaces;
using PVZEngine;
using UnityEditor.Graphs;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

namespace MVZ2.Editor
{
    public static class TalkMenu
    {
        public static void FormatTalkDatas()
        {
            var spaceName = VanillaMod.spaceName;

            var talkDir = GetTalkMetaDirectory(spaceName);
            foreach (var filePath in Directory.GetFiles(talkDir, "*.xml", SearchOption.TopDirectoryOnly))
            {
                using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
                var document = stream.ReadXmlDocument();
                var metaList = TalkMeta.FromXmlDocument(document, spaceName);
                var newDocument = metaList.ToXmlDocument();

                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                var settings = new XmlWriterSettings()
                {
                    Indent = true,
                    Encoding = Encoding.UTF8,
                    IndentChars = "  ",
                    NewLineChars = "\n"
                };
                using var writer = XmlWriter.Create(stream, settings);
                newDocument.WriteTo(writer);
                Debug.Log($"Formatted talk data {filePath}.");
            }
            Debug.Log("Talk Data Files Formatted.");
        }
        public static void ConvertNaturalLanguageToTalks()
        {
            var spaceName = VanillaMod.spaceName;
            var sourcePath = Path.Combine(Application.dataPath, "TalkConverter", "source.txt");
            var targetPath = Path.Combine(Application.dataPath, "TalkConverter", "target.xml");

            if (!File.Exists(sourcePath))
            {
                Debug.LogWarning($"File {sourcePath} does not exists for conversion.");
                return;
            }

            using var reader = new StreamReader(sourcePath);

            var document = new XmlDocument();
            var groupsNode = document.CreateElement("groups");
            document.AppendChild(groupsNode);

            TalkConverter convertor = new TalkConverter(reader, spaceName);
            convertor.Convert(document, groupsNode);

            var settings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                IndentChars = "  ",
                NewLineChars = "\n"
            };
            using var outputStream = File.Open(targetPath, FileMode.OpenOrCreate);
            using var writer = XmlWriter.Create(outputStream, settings);
            document.WriteTo(writer);
            Debug.Log($"Talk data file converted to {targetPath}.");
        }
        private static string GetTalkMetaDirectory(string nsp)
        {
            return Path.Combine(Application.dataPath, "GameContent", "Assets", nsp, "metas", "talks");
        }
        private static XmlDocument LoadTalkMetaXmlDocument(string nsp, string path)
        {
            var metaDirectory = GetTalkMetaDirectory(nsp);
            var absPath = Path.Combine(metaDirectory, path);
            using FileStream stream = File.Open(absPath, FileMode.Open);
            return stream.ReadXmlDocument();
        }
    }
}
