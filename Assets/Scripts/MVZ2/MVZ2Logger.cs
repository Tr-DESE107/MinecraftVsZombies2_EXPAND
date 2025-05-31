using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using MVZ2.IO;
using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Debugs
{
    public class MVZ2Logger : MonoBehaviour
    {
        public bool ExportLogFilePack(string destPath)
        {
            var dir = Application.persistentDataPath;
            if (!Directory.Exists(dir))
                return false;

            CloneCurrentLog();

            FileHelper.ValidateDirectory(destPath);
            var sourceDirInfo = new DirectoryInfo(dir);
            var files = new string[]
            {
                GetLogClonePath(),
                GetPrevLogPath(),
            };
            using var stream = File.Open(destPath, FileMode.Create);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            foreach (var filePath in files)
            {
                if (!File.Exists(filePath))
                    continue;
                var entryName = Path.GetRelativePath(dir, filePath);
                entryName = entryName.Replace("\\", "/");
                archive.CreateEntryFromFile(filePath, entryName);
            }

            DeleteLogClone();

            return true;
        }

        #region 生命周期
        void OnEnable()
        {
            Application.logMessageReceived += OnLogReceivedCallback;
        }
        void OnDisable()
        {
            Application.logMessageReceived -= OnLogReceivedCallback;
        }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            string path = GetLogPath();
            // 开启游戏时将上一次的日志前缀加上-prev。
            if (File.Exists(path))
            {
                string prevPath = GetPrevLogPath();
                if (File.Exists(prevPath))
                {
                    File.Delete(prevPath);
                }
                File.Move(path, prevPath);
            }
            StartLogWriter(path);

            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                throw new DuplicateInstanceException(name);
            }
        }
#if UNITY_ANDROID
        private void OnApplicationFocus(bool focus)
        {
            // 安卓失去焦点后，关闭文件流。
            // 因为安卓切换到其他应用时，可能会在后台被系统施放，而不调用OnApplicationQuit.
            if (!focus)
            {
                CloseLogWriter();
            }
        }
#endif
        private void OnApplicationQuit()
        {
            // 退出游戏后，关闭文件流。
            CloseLogWriter();
        }
        #endregion

        #region 事件回调
        void OnLogReceivedCallback(string logString, string stackTrace, LogType type)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{timestamp}] ");
            sb.Append($"[{type}] ");
            sb.Append(logString);
            sb.Append("\n");
            sb.Append(stackTrace);
            sb.Append("\n\n");

            lock (logExportlock)
            {
                if (logWriter == null)
                {
                    StartLogWriter(GetLogPath());
                }
                logWriter.WriteLine(sb.ToString());
            }
        }
        #endregion

        #region 输出日志内容
        private string GetLogPath()
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}{extension}");
        }
        private string GetPrevLogPath()
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}{prevSuffix}{extension}");
        }
        private string GetLogClonePath()
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}{cloneSuffix}{extension}");
        }
        private void StartLogWriter(string path)
        {
            if (logWriter == null)
            {
                logWriter = new StreamWriter(path, true);
                logWriter.AutoFlush = true;
            }
        }
        private void CloseLogWriter()
        {
            if (logWriter != null)
            {
                logWriter.Close();
                logWriter = null;
            }
        }
        #endregion

        #region 导出日志
        private void CloneCurrentLog()
        {
            lock (logExportlock)
            {
                CloseLogWriter();

                DeleteLogClone();
                var currentLogPath = GetLogPath();
                var cloneLogPath = GetLogClonePath();
                File.Copy(currentLogPath, cloneLogPath);
            }
        }
        private void DeleteLogClone()
        {
            var cloneLogPath = GetLogClonePath();
            if (!File.Exists(cloneLogPath))
                return;
            File.Delete(cloneLogPath);
        }
        #endregion

        public static MVZ2Logger Instance { get; private set; }
        [SerializeField]
        string fileName = "mvz2_log";
        [SerializeField]
        string cloneSuffix = "-current";
        [SerializeField]
        string prevSuffix = "-prev";
        [SerializeField]
        string extension = ".log";
        StreamWriter logWriter;
        object logExportlock = new object();
    }
}
