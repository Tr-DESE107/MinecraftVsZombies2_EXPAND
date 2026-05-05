using System.IO;
using System.Text;
using System.Xml;
using MVZ2.IO;
using MVZ2.TalkData;
using MVZ2.Vanilla;
using UnityEngine;

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
