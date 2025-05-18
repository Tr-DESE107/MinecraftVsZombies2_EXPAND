using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SFB;

namespace MVZ2.IO
{
    public static class FileHelper
    {
        public static void ValidateDirectory(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        public static void WriteBytes(Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }
        public static Task<string> OpenExternalFile(string[] extensions, Action<string> importAction)
        {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
            var selectedPathes = StandaloneFileBrowser.OpenFilePanel("", "", extensions.Select(e => new ExtensionFilter(string.Empty, extensions)).ToArray(), false);
            if (selectedPathes.Length == 0)
                return Task.FromResult<string>(null);
            var importPath = selectedPathes[0];
            importAction?.Invoke(importPath);
            return Task.FromResult<string>(importPath);
#elif UNITY_IOS || UNITY_ANDROID
            var t = new TaskCompletionSource<string>();
            var fileTypes = extensions.Select(e => NativeFilePicker.ConvertExtensionToFileType(e)).ToArray();
            NativeFilePicker.PickFile(path => {
                if (!string.IsNullOrEmpty(path))
                {
                    importAction?.Invoke(path);
                }
                t.SetResult(path);
            }, fileTypes);
            return t.Task;
#else
            return Task.FromResult<string>(null);
#endif
        }
        public static async Task<string> SaveExternalFile(string defaultName, string[] extensions, Action<string> saveAction)
        {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
            var exportPath = StandaloneFileBrowser.SaveFilePanel("", "", defaultName, extensions.Select(e => new ExtensionFilter(string.Empty, e)).ToArray());
            if (string.IsNullOrEmpty(exportPath))
                return null;
            saveAction?.Invoke(exportPath);
            await Task.CompletedTask;
            return exportPath;
#elif UNITY_IOS || UNITY_ANDROID
            // 这个插件是先写出再复制到目标目录的
            var fileName = defaultName;
            if (extensions.Length > 0)
            {
                fileName = $"{fileName}.{extensions[0]}";
            }
            var tempPath = Path.Combine(Application.temporaryCachePath, fileName);
            saveAction?.Invoke(tempPath);

            if (!File.Exists(tempPath))
            {
                return null;
            }

            var t = new TaskCompletionSource<string>();
            NativeFilePicker.ExportFile(tempPath, success => t.SetResult(success ? fileName : null));
            var exportPath = await t.Task;
            File.Delete(tempPath); // 清理临时文件
            return exportPath;
#else
            await Task.CompletedTask;
            return null;
#endif
        }
    }
}
