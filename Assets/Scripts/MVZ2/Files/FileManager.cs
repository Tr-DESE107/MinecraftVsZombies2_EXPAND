using System;
using MVZ2Logic;
using UnityEngine;

namespace MVZ2.Managers
{
    public class FileManager : MonoBehaviour
    {
        public void WriteJsonFile(string path, string json)
        {
            if (!compressed && IsEditor())
            {
                SerializeHelper.WriteJson(path, json);
            }
            else
            {
                SerializeHelper.WriteCompressedJson(path, json);
            }
        }
        public string ReadJsonFile(string path)
        {
            try
            {
                return SerializeHelper.ReadCompressedJson(path);
            }
            catch (Exception)
            {
                return SerializeHelper.ReadJson(path);
            }
        }
        private bool IsEditor()
        {
            return Application.isEditor;
        }
        [SerializeField]
        private bool compressed = true;
    }
}
