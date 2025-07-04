using System.IO;
using MVZ2Logic;
using UnityEngine;

namespace MVZ2.IO
{
    public class FileManager : MonoBehaviour
    {
        public void WriteStringFile(string path, string content)
        {
            if (!compressed && IsEditor())
            {
                SerializeHelper.Write(path, content);
            }
            else
            {
                SerializeHelper.WriteCompressedStringFile(path, content);
            }
        }
        public string ReadStringFile(string path)
        {
            if (SerializeHelper.IsGZipCompressed(path))
            {
                return SerializeHelper.ReadCompressed(path);
            }
            return SerializeHelper.Read(path);
        }
        public Stream OpenFileWrite(string path)
        {
            if (!compressed && IsEditor())
            {
                return SerializeHelper.OpenWrite(path);
            }
            else
            {
                return SerializeHelper.OpenCompressedWrite(path);
            }
        }
        public MemoryStream OpenFileRead(string path)
        {
            if (SerializeHelper.IsGZipCompressed(path))
            {
                return SerializeHelper.OpenCompressedRead(path);
            }
            return SerializeHelper.OpenRead(path);
        }
        private bool IsEditor()
        {
            return Application.isEditor;
        }
        [SerializeField]
        private bool compressed = true;
    }
}
