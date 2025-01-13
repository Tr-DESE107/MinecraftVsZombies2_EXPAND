using System.IO;
using System.Text;
using UnityEngine;

namespace MVZ2
{
    public class AndroidLogger : MonoBehaviour
    {
        #region 生命周期
        private void Awake()
        {
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
        }
        void OnEnable()
        {
            Application.logMessageReceived += OnLogReceivedCallback;
        }
        void OnDisable()
        {
            Application.logMessageReceived -= OnLogReceivedCallback;
        }
        private void OnApplicationFocus(bool focus)
        {
            // 安卓失去焦点后，关闭文件流。
            // 因为安卓切换到其他应用时，可能会在后台被系统施放，而不调用OnApplicationQuit.
            if (!focus)
            {
                CloseLogWriter();
            }
        }
        private void OnApplicationQuit()
        {
            // 退出游戏后，关闭文件流。
            CloseLogWriter();
        }
        #endregion
        void OnLogReceivedCallback(string logString, string stackTrace, LogType type)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{type}]");
            sb.Append(logString);
            sb.Append("\n");
            sb.Append(stackTrace);
            sb.Append("\n\n");

            if (logWriter == null)
            {
                StartLogWriter(GetLogPath());
            }
            logWriter.WriteLine(sb.ToString());
        }

        private string GetLogPath()
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}{extension}");
        }
        private string GetPrevLogPath()
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}{prevSuffix}{extension}");
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

        [SerializeField]
        string fileName = "mvz2_log";
        [SerializeField]
        string prevSuffix = "-prev";
        [SerializeField]
        string extension = ".log";
        StreamWriter logWriter;
    }
}


