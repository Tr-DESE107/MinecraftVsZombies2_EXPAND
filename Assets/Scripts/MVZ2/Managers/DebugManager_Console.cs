using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MukioI18n;
using MVZ2.IO;
using MVZ2.Metas;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.Games;
using MVZ2Logic.IZombie;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class DebugManager : MonoBehaviour, IDebugManager
    {
        public void LoadCommandParameterSuggestions()
        {
            entityIDSet.Clear();
            blueprintIDSet.Clear();
            foreach (var def in Main.Game.GetDefinitions<EntityDefinition>(EngineDefinitionTypes.ENTITY))
            {
                entityIDSet.Add(def.GetID().ToString());
            }
            foreach (var def in Main.Game.GetDefinitions<SeedDefinition>(EngineDefinitionTypes.SEED))
            {
                blueprintIDSet.Add(def.GetID().ToString());
            }
        }
        public bool IsConsoleActive()
        {
            return Main.Scene.IsConsoleActive();
        }
        public string[] GetCommandHistory()
        {
            return Main.Scene.GetCommandHistory();
        }
        public void ExecuteCommand(string input)
        {
            PrintLine($"> {input}");

            string[] parts = SplitCommand(input);
            if (parts.Length < 1)
                return;
            string command = parts[0];
            string[] args = parts.Skip(1).ToArray();

            CommandDefinition definition = null;
            if (NamespaceID.TryParse(command, Main.BuiltinNamespace, out var commandID))
            {
                definition = Global.Game.GetCommandDefinition(commandID);
            }
            if (definition == null)
            {
                var msg = Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, COMMAND_NOT_FOUND, command);
                PrintLine(msg);
                return;
            }

            try
            {
                ValidateCommand(parts);
                definition.Invoke(args);
            }
            catch (Exception ex)
            {
                var msg = Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, COMMAND_ERROR, ex.Message);
                PrintLine(msg);
            }
        }
        public string[] SplitCommand(string input)
        {
            if (!input.StartsWith(COMMAND_CHARACTER))
                return Array.Empty<string>();
            input = input.Substring(1);

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
        public void ClearConsole()
        {
            Main.Scene.ClearConsole();
        }

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
            return Main.ResourceManager.GetCommandNameByID(id);
        }

        #region 自动补全
        private CommandMetaVariant GetBestFitCommandVariant(CommandMetaVariant[] variants, string[] parts)
        {
            var minDifference = int.MaxValue;
            CommandMetaVariant bestVariant = null;
            foreach (var variant in variants)
            {
                var idx = GetFitCommandVariantParamIndex(variant, parts);
                if (idx == parts.Length)
                    return variant;
                if (idx - parts.Length < minDifference)
                {
                    minDifference = idx;
                    bestVariant = variant;
                }
            }
            return bestVariant;
        }
        private int GetFitCommandVariantParamIndex(CommandMetaVariant variant, string[] parts)
        {
            bool hasSubname = !String.IsNullOrEmpty(variant.Subname);
            for (int i = 1; i < parts.Length; i++)
            {
                int parameterIndex = i - 1;
                if (hasSubname)
                {
                    if (i == 1 && !variant.Subname.Equals(parts[i], StringComparison.OrdinalIgnoreCase))
                        // 子命令不一致。
                        return i;

                    parameterIndex--;
                }


                if (parameterIndex < 0 || parameterIndex >= variant.Parameters.Length)
                    continue;
                var parameter = variant.Parameters[parameterIndex];
                if (parameter == null)
                    continue;

                if (!FitsCommandParameter(parameter, parts[i]))
                    // 参数不一致。
                    return i;
            }
            return parts.Length;
        }
        private bool FitsCommandParameter(CommandMetaParam param, string paramText)
        {
            switch (param.Type)
            {
                case CommandMetaParam.TYPE_COMMAND:
                    {
                        var id = Main.ResourceManager.GetCommandIDByName(paramText);
                        return Main.ResourceManager.GetCommandMeta(id) != null;
                    }
                case CommandMetaParam.TYPE_INT:
                    {
                        return int.TryParse(paramText, out _);
                    }
                case CommandMetaParam.TYPE_FLOAT:
                    {
                        return float.TryParse(paramText, out _);
                    }
                case CommandMetaParam.TYPE_ID:
                    {
                        if (!NamespaceID.TryParse(paramText, Main.BuiltinNamespace, out var id))
                        {
                            return false;
                        }
                        return FitsCommandParameterID(param, id);
                    }
            }
            return false;
        }
        private bool FitsCommandParameterID(CommandMetaParam param, NamespaceID id)
        {
            switch (param.IDType)
            {
                case CommandMetaParam.ID_TYPE_ENTITY:
                    {
                        return Main.Game.GetEntityDefinition(id) != null;
                    }
                case CommandMetaParam.ID_TYPE_BLUEPRINT:
                    {
                        return Main.Game.GetSeedDefinition(id) != null;
                    }
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
                    var meta = Main.ResourceManager.GetCommandMeta(cmd);
                    if (meta == null)
                        continue;
                    if (!Main.Game.IsInLevel() && meta.InLevel)
                        continue;
                    currentSuggestions.Add(cmdName);
                }
            }
        }
        private void FillParameterSuggestions(CommandMeta meta, string[] parts, List<string> currentSuggestions)
        {
            if (meta == null)
                return;

            // 变体子名称
            if (parts.Length == 2)
            {
                var subnameVariants = meta.Variants.Where(v => !string.IsNullOrEmpty(v.Subname));
                foreach (var v in subnameVariants)
                {
                    if (v.Subname.StartsWith(parts[1]))
                    {
                        currentSuggestions.Add(v.Subname);
                    }
                }
            }
            // 查找目前最符合的命令变体。
            var completedParts = parts.SkipLast(1).ToArray();
            var variant = GetBestFitCommandVariant(meta.Variants, completedParts);

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
            var suggestions = GetSuggestionsOfParameter(param);
            foreach (var suggestion in suggestions)
            {
                if (string.IsNullOrEmpty(last) || suggestion.StartsWith(last))
                {
                    currentSuggestions.Add(suggestion);
                }
                else if (NamespaceID.TryParseStrict(suggestion, out var parsed) && parsed.Path.StartsWith(last))
                {
                    currentSuggestions.Add(suggestion);
                }
            }
        }
        private IEnumerable<string> GetSuggestionsOfParameter(CommandMetaParam param)
        {
            switch (param.Type)
            {
                case CommandMetaParam.TYPE_COMMAND:
                    {
                        var ids = Main.ResourceManager.GetAllCommandsID();
                        foreach (var id in ids)
                        {
                            var name = GetCommandName(id);
                            yield return name;
                        }
                    }
                    break;
                case CommandMetaParam.TYPE_ID:
                    {
                        switch (param.IDType)
                        {
                            case CommandMetaParam.ID_TYPE_ENTITY:
                                {
                                    foreach (var sug in entityIDSet)
                                    {
                                        yield return sug;
                                    }
                                }
                                break;
                            case CommandMetaParam.ID_TYPE_BLUEPRINT:
                                {
                                    foreach (var sug in blueprintIDSet)
                                    {
                                        yield return sug;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        #endregion

        #region 命令校验
        private void ValidateCommand(string[] parts)
        {
            if (parts.Length == 0)
                return;
            var commandName = parts[0];
            var commandID = Main.ResourceManager.GetCommandIDByName(commandName);
            var commandMeta = Main.ResourceManager.GetCommandMeta(commandID);
            if (commandMeta == null)
                return;
            // 在关卡外执行关卡命令
            if (commandMeta.InLevel && !Global.Game.IsInLevel())
                throw new InvalidOperationException(Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, VanillaStrings.COMMAND_MUST_IN_LEVEL));

            var variant = GetBestFitCommandVariant(commandMeta.Variants, parts);
            // 命令变体不存在。
            if (variant == null)
                throw new ArgumentException(Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, VanillaStrings.COMMAND_INCORRECT_FORMAT));

            // 检测命令变体的子名称是否正确。
            var hasSubname = !string.IsNullOrEmpty(variant.Subname);
            if (hasSubname)
            {
                // 没有子名称
                if (parts.Length < 2)
                    throw new ArgumentException(Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, VanillaStrings.COMMAND_MISSING_PARAMETER, variant.Subname));
                // 子名称不正确
                if (!variant.Subname.Equals(parts[1], StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException(Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, VanillaStrings.COMMAND_INCORRECT_PARAMETER, variant.Subname));
            }
            // 检测命令变体的参数是否正确。
            for (int i = 0; i < variant.Parameters.Length; i++)
            {
                var partIndex = i + 1;
                if (hasSubname)
                {
                    partIndex++;
                }
                var parameter = variant.Parameters[i];
                if (partIndex >= parts.Length)
                {
                    if (parameter.Optional)
                        continue;

                    throw new ArgumentException(Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, VanillaStrings.COMMAND_MISSING_PARAMETER, parameter.Name));
                }
                var part = parts[partIndex];

                if (!FitsCommandParameter(parameter, part))
                {
                    throw new ArgumentException(Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_OUTPUT, VanillaStrings.COMMAND_INCORRECT_PARAMETER, parameter.Name));
                }
            }
        }
        #endregion

        private void Print(string text)
        {
            Main.Scene.Print(text);
        }
        private void PrintLine(string text)
        {
            Print(text + "\n");
        }
        [SerializeField]
        private string commandHistoryFileName = "commands.txt";
        private HashSet<string> entityIDSet = new HashSet<string>();
        private HashSet<string> blueprintIDSet = new HashSet<string>();

        public const char COMMAND_CHARACTER = '/';
        [TranslateMsg("命令输出，{0}为命令名", VanillaStrings.CONTEXT_COMMAND_OUTPUT)]
        public const string COMMAND_NOT_FOUND = "<color=red>命令不存在：{0}</color>";
        [TranslateMsg("命令输出，{0}为错误", VanillaStrings.CONTEXT_COMMAND_OUTPUT)]
        public const string COMMAND_ERROR = "<color=red>错误：{0}</color>";
    }
}
