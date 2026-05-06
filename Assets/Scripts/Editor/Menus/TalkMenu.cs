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
            var groupsNode = document.CreateElement("talks");
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
        public static void ConvertXMLToPlainText()
        {
            var spaceName = VanillaMod.spaceName;
            var sourcePath = Path.Combine(Application.dataPath, "TalkConverter", "source.xml");
            var targetPath = Path.Combine(Application.dataPath, "TalkConverter", "target.txt");

            if (!File.Exists(sourcePath))
            {
                Debug.LogWarning($"File {sourcePath} does not exists for conversion.");
                return;
            }

            var charactersXMLPath = Path.Combine(Application.dataPath, "GameContent", "Assets", spaceName, "metas", "talkcharacters.xml");
            TalkCharacterMetaList? characterMetaList = null;
            if (File.Exists(charactersXMLPath))
            {
                using FileStream characterStream = File.Open(charactersXMLPath, FileMode.Open, FileAccess.Read);
                var characterDoc = characterStream.ReadXmlDocument();
                characterMetaList = TalkCharacterMetaList.FromXmlNode(characterDoc["characters"], spaceName);
            }

            using FileStream sourceStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read);
            var document = sourceStream.ReadXmlDocument();
            var metaList = TalkMeta.FromXmlDocument(document, spaceName);

            using var targetStream = File.Open(targetPath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(targetStream, Encoding.UTF8);
            foreach (var group in metaList.groups)
            {
                var name = group.archive?.name ?? string.Empty;
                writer.WriteLine($"# {name}");

                foreach (var section in group.sections)
                {
                    var sectionName = section.archiveText;
                    if (!string.IsNullOrEmpty(sectionName))
                    {
                        writer.WriteLine($"[{sectionName}]");
                    }

                    foreach (var sentence in section.sentences)
                    {
                        if (!string.IsNullOrEmpty(sentence.description))
                        {
                            writer.WriteLine($"{sentence.description}");
                        }
                        var speakerID = sentence.speaker;
                        string speakerName = "unknown";
                        if (NamespaceID.IsValid(speakerID))
                        {
                            speakerName = speakerID.ToString();
                            if (characterMetaList != null)
                            {
                                foreach (var meta in characterMetaList.metas)
                                {
                                    if (meta.id == speakerID.Path)
                                    {
                                        speakerName = meta.name;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(sentence.speakerName))
                        {
                            speakerName = $"{sentence.speakerName}({speakerName})";
                        }
                        writer.WriteLine($"{speakerName}：{sentence.text}");
                    }
                }
                writer.WriteLine();
            }

            Debug.Log($"Talk xml file converted to {targetPath}.");
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
