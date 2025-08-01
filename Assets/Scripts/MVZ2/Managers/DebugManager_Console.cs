using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MVZ2.IO;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class DebugManager : MonoBehaviour
    {
        #region 历史
        public void SaveCommandHistory(IEnumerable<string> history)
        {
            var path = GetCommandHistoryPath();
            FileHelper.ValidateDirectory(path);
            using var stream = File.Open(path, FileMode.Create);
            using var textWriter = new StreamWriter(stream);
            textWriter.Write(string.Join('\n', history));
        }
        public void LoadCommandHistory(List<string> history)
        {
            var path = GetCommandHistoryPath();
            if (!File.Exists(path))
            {
                return;
            }
            using var stream = File.Open(path, FileMode.Open);
            using var textReader = new StreamReader(stream);
            while (!textReader.EndOfStream)
            {
                history.Add(textReader.ReadLine());
            }
        }
        private string GetCommandHistoryPath()
        {
            return Path.Combine(Application.persistentDataPath, commandHistoryFileName);
        }
        #endregion

        private string GetCommandName(NamespaceID id)
        {
            if (id.SpaceName == Main.BuiltinNamespace)
            {
                return id.Path;
            }
            return id.ToString();
        }
        public string[] SplitInputText(string input)
        {
            List<string> parts = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsWhiteSpace(c))
                {
                    parts.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            parts.Add(sb.ToString());
            sb.Clear();

            return parts.ToArray();
        }

        #region 自动补全
        private bool FitsCommandVariant(CommandMetaVariant variant, string[] parts)
        {
            if (parts.Length <= 1)
                return false;
            bool hasSubname = !String.IsNullOrEmpty(variant.Subname);
            for (int i = 1; i < parts.Length - 1; i++)
            {
                int parameterIndex = i - 1;
                if (hasSubname)
                {
                    if (i == 1 && !variant.Subname.Equals(parts[i], StringComparison.OrdinalIgnoreCase))
                        // 子命令不一致。
                        return false;

                    parameterIndex--;
                }


                if (parameterIndex < 0 || parameterIndex >= variant.Parameters.Length)
                    continue;
                var parameter = variant.Parameters[parameterIndex];
                if (parameter == null)
                    continue;

                if (!FitsCommandParameter(parameter, parts[i]))
                    // 参数不一致。
                    return false;
            }
            return true;
        }
        private bool FitsCommandParameter(CommandMetaParam param, string paramText)
        {
            switch (param.Type)
            {
                case "command":
                    if (NamespaceID.TryParse(paramText, Main.BuiltinNamespace, out var id))
                    {
                        return Main.ResourceManager.GetCommandMeta(id) != null;
                    }
                    break;
            }
            return false;
        }
        public void FillSuggestions(string[] parts, List<string> currentSuggestions)
        {
            string currentCommand = parts[0].ToLower();
            if (!NamespaceID.TryParse(currentCommand, Main.BuiltinNamespace, out var commandID))
                return;

            // 命令建议
            if (parts.Length == 1)
            {
                var currentCommandName = GetCommandName(commandID);
                FillNameSuggestions(currentCommandName, currentSuggestions);
            }
            // 参数建议
            else
            {
                CommandMeta meta = Main.ResourceManager.GetCommandMeta(commandID);
                FillParameterSuggestions(meta, parts, currentSuggestions);
            }
        }
        private void FillNameSuggestions(string currentCommandName, List<string> currentSuggestions)
        {
            foreach (var cmd in Main.ResourceManager.GetAllCommandsID())
            {
                var cmdName = GetCommandName(cmd);
                if (cmdName.StartsWith(currentCommandName, StringComparison.OrdinalIgnoreCase))
                {
                    currentSuggestions.Add(cmdName);
                }
            }
        }
        private void FillParameterSuggestions(CommandMeta meta, string[] parts, List<string> currentSuggestions)
        {
            if (meta == null)
                return;

            // 查找目前最符合的命令变体。
            IEnumerable<CommandMetaVariant> validVariants = meta.Variants;
            for (int i = 1; i < parts.Length - 1; i++)
            {
                validVariants = validVariants.Where(v => FitsCommandVariant(v, parts));
            }

            var variant = validVariants.FirstOrDefault();
            FillSuggestionsOfCommandVariant(variant, parts, currentSuggestions);
        }
        private void FillSuggestionsOfCommandVariant(CommandMetaVariant variant, string[] parts, List<string> currentSuggestions)
        {
            if (variant == null)
                return;
            int parameterIndex = parts.Length - 2;
            if (!String.IsNullOrEmpty(variant.Subname))
            {
                parameterIndex--;
            }
            if (parameterIndex < 0 || parameterIndex >= variant.Parameters.Length)
                return;
            var parameter = variant.Parameters[parameterIndex];
            FillSuggestionsOfCommandParameter(parameter, parts, currentSuggestions);
        }
        private void FillSuggestionsOfCommandParameter(CommandMetaParam param, string[] parts, List<string> currentSuggestions)
        {
            if (param == null)
                return;
            var last = parts.Last();
            var suggestions = GetSuggestionsOParameterType(param.Type);
            foreach (var suggestion in suggestions)
            {
                if (string.IsNullOrEmpty(last) || suggestion.StartsWith(last))
                {
                    currentSuggestions.Add(suggestion);
                }
            }
        }
        private IEnumerable<string> GetSuggestionsOParameterType(string type)
        {
            switch (type)
            {
                case "command":
                    {
                        var ids = Main.ResourceManager.GetAllCommandsID();
                        foreach (var id in ids)
                        {
                            var name = GetCommandName(id);
                            yield return name;
                        }
                    }
                    break;
            }
        }
        #endregion

        public void ExecuteCommand(string input)
        {
            PrintLine($"> {input}");

            string[] parts = SplitInputText(input);
            string command = parts[0];
            string[] args = parts.Skip(1).ToArray();

            //CommandDefinition definition = null;
            //if (NamespaceID.TryParse(command, Main.BuiltinNamespace, out var commandID))
            //{
            //    definition = Global.Game.GetCommandDefinition(commandID);
            //}
            //if (definition == null)
            //{
            //    PrintLine($"<color=red>Command not found: {command}</color>");
            //    return;
            //}

            //try
            //{
            //    definition.Invoke(args);
            //}
            //catch (Exception ex)
            //{
            //    PrintLine($"<color=red>Error: {ex.Message}</color>");
            //}
        }
        private void PrintLine(string text)
        {
            Main.Scene.ConsolePrintLine(text);
        }
        [SerializeField]
        private string commandHistoryFileName = "commands.txt";
    }
}
