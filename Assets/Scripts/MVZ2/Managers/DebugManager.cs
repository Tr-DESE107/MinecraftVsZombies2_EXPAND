using System;
using MukioI18n;
using MVZ2.Debugs;
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
                    success = Logger.ExportLogFilePack(dest);
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
        public MainManager Main => MainManager.Instance;
        public MVZ2Logger Logger => MVZ2Logger.Instance;
        [TranslateMsg("日志导出失败的警告")]
        public const string ERROR_NOT_EXPORTED = "导出日志失败。";
        [TranslateMsg("日志导出成功的提示，{0}为路径")]
        public const string HINT_EXPORTED = "日志已导出至{0}。";
    }
}
