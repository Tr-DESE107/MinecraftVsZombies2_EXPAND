using System;
using System.IO;
using System.IO.Compression;
using MukioI18n;
using MVZ2.IO;
using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.Managers
{
    public class DebugManager : MonoBehaviour
    {
        public async void ExportLogFiles()
        {
            bool success = false;
            var path = await FileHelper.SaveExternalFile("log_files", new string[] { "zip" }, dest =>
            {
                if (string.IsNullOrEmpty(dest))
                    return;
                try
                {
                    success = ExportLogFilePack(dest);
                }
                catch (Exception)
                {
                    success = false;
                }
            });
            string title, desc;
            if (!success)
            {
                title = Main.LanguageManager._(VanillaStrings.ERROR);
                desc = Main.LanguageManager._(ERROR_NOT_EXPORTED);
            }
            else
            {
                title = Main.LanguageManager._(VanillaStrings.HINT);
                desc = Main.LanguageManager._(HINT_EXPORTED, path);
            }
            await Main.Scene.ShowDialogMessageAsync(title, desc);
        }
        public bool ExportLogFilePack(string destPath)
        {
            var dir = Application.persistentDataPath;
            if (!Directory.Exists(dir))
                return false;

            FileHelper.ValidateDirectory(destPath);
            var sourceDirInfo = new DirectoryInfo(dir);
            var files = Directory.GetFiles(dir, "*.log", SearchOption.AllDirectories);
            using var stream = File.Open(destPath, FileMode.Create);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            foreach (var filePath in files)
            {
                var entryName = Path.GetRelativePath(dir, filePath);
                entryName = entryName.Replace("\\", "/");
                archive.CreateEntryFromFile(filePath, entryName);
            }
            return true;
        }
        public MainManager Main => MainManager.Instance;
        [TranslateMsg("日志导出失败的警告")]
        public const string ERROR_NOT_EXPORTED = "导出日志失败。";
        [TranslateMsg("日志导出成功的提示，{0}为路径")]
        public const string HINT_EXPORTED = "日志已导出至{0}。";
    }
}
