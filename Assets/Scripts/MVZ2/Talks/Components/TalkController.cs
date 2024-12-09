using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Managers;
using MVZ2.TalkData;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Talk
{
    public class TalkController : MonoBehaviour
    {
        #region 公有方法
        /// <summary>
        /// 开始进行对话。
        /// </summary>
        public async void StartTalk(NamespaceID groupId, int sectionIndex, float delay = 0)
        {
            var group = Main.ResourceManager.GetTalkGroup(groupId);
            if (group == null)
                throw new InvalidOperationException($"Could not find talk group with id {groupId}.");
            IsTalking = true;

            groupID = groupId;
            this.sectionIndex = sectionIndex;
            sentenceIndex = 0;

            ui.SetSpeechBubbleShowing(false);
            ui.SetBlockerActive(true);
            ui.SetRaycastReceiverActive(true);
            ClearCharacters();

            var section = group.sections[sectionIndex];
            await ExecuteScriptsAsync(section.startScripts);

            _ = DelayedStart(delay);
        }
        public void TryStartTalk(NamespaceID groupId, int sectionIndex, float delay = 0, Action<bool> onFinished = null)
        {
            if (!CanStartTalk(groupId))
            {
                onFinished?.Invoke(false);
                return;
            }

            if (WillSkipTalk(groupId, sectionIndex))
            {
                var section = Main.ResourceManager.GetTalkSection(groupId, sectionIndex);
                if (section == null)
                {
                    onFinished?.Invoke(false);
                }
                else
                {
                    StartCoroutine(routine());

                    IEnumerator routine()
                    {
                        var task1 = ExecuteScriptsAsync(section.startScripts);
                        while (!task1.IsCompleted)
                            yield return null;
                        var task2 = ExecuteScriptsAsync(section.skipScripts);
                        while (!task2.IsCompleted)
                            yield return null;
                        onFinished?.Invoke(false);
                    }
                }
            }
            else
            {
                StartTalk(groupId, sectionIndex, delay);
                onFinished?.Invoke(true);
            }
        }
        public bool CanStartTalk(NamespaceID groupId)
        {
            var group = Main.ResourceManager.GetTalkGroup(groupId);
            if (group == null)
                return false;
            if (NamespaceID.IsValid(group.requires) && !Main.SaveManager.IsUnlocked(group.requires))
                return false;
            if (NamespaceID.IsValid(group.requiresNot) && Main.SaveManager.IsUnlocked(group.requiresNot))
                return false;
            return true;
        }
        public bool WillSkipTalk(NamespaceID groupId, int sectionIndex)
        {
            if (!Main.OptionsManager.SkipAllTalks())
                return false;
            var section = Main.ResourceManager.GetTalkSection(groupId, sectionIndex);
            if (section == null)
                return false;
            if (!section.canAutoSkip)
                return false;
            return true;
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            ui.OnSkipClick += OnSkipClickedCallback;
            ui.OnClick += OnClickCallback;
        }
        private void OnDisable()
        {
            StopAllCoroutines();
            ClearCharacters();
        }
        #endregion

        #region 事件回调
        private void OnClickCallback()
        {
            var sentence = GetTalkSentence();
            IEnumerable<TalkScript> scripts = sentence.clickScripts != null ? sentence.clickScripts : defaultClickScripts;
            _ = ExecuteScriptsAsync(scripts);
        }
        private void OnSkipClickedCallback()
        {
            var section = GetTalkSection();
            IEnumerable<TalkScript> scripts = section.skipScripts != null ? section.skipScripts : defaultSkipScripts;
            _ = ExecuteScriptsAsync(scripts);
        }
        #endregion

        #region 参数
        private int ParseArgumentInt(string str)
        {
            return ParseHelper.ParseInt(str);
        }
        private float ParseArgumentFloat(string str)
        {
            return ParseHelper.ParseFloat(str);
        }
        private Color ParseArgumentColor(string str)
        {
            if (ColorUtility.TryParseHtmlString(str, out var color))
            {
                return color;
            }
            return new Color(1, 1, 1, 0);
        }
        private NamespaceID ParseArgumentNamespaceID(string str)
        {
            return NamespaceID.Parse(str, Main.BuiltinNamespace);
        }
        private SpriteReference ParseArgumentSpriteReference(string str)
        {
            return SpriteReference.Parse(str, Main.BuiltinNamespace);
        }
        private CharacterSide ParseCharacterSide(string side)
        {
            return side == "right" ? CharacterSide.Right : CharacterSide.Left;
        }
        private void FadeArgumentsColor(ColorFader fader, string[] args)
        {
            if (args.Length >= 3)
            {
                fader.Value = ParseArgumentColor(args[0]);
                fader.StartFade(ParseArgumentColor(args[1]), ParseArgumentFloat(args[2]));
            }
            else if (args.Length == 2)
            {
                fader.StartFade(ParseArgumentColor(args[0]), ParseArgumentFloat(args[1]));
            }
            else if (args.Length == 1)
            {
                fader.StartFade(ParseArgumentColor(args[0]), 1);
            }
        }
        #endregion

        #region 脚本
        private void ExecuteScriptSync(TalkScript script)
        {
            var args = script.arguments;
            switch (script.function)
            {
                #region 流程控制
                case "next":
                    NextSentence();
                    break;

                case "section":
                    StartSection(ParseArgumentInt(args[0]));
                    break;

                case "sentence":
                    SetSentence(ParseArgumentInt(args[0]));
                    break;

                case "end":
                    EndTalk();
                    break;
                #endregion

                #region 人物相关
                case "character":
                    {
                        switch (args[0])
                        {
                            case "create":
                                {
                                    var characterId = ParseArgumentNamespaceID(args[1]);
                                    CharacterSide side = ParseCharacterSide(args[2]);
                                    var variant = DEFAULT_VARIANT_ID;
                                    if (args.Length > 3)
                                    {
                                        variant = ParseArgumentNamespaceID(args[3]);
                                    }
                                    CreateCharacter(characterId, variant, side);
                                }
                                break;
                            case "change":
                                {
                                    var characterIndex = GetCharacterIndex(ParseArgumentNamespaceID(args[1]));
                                    if (characterIndex < 0)
                                        break;
                                    var targetCharacter = ParseArgumentNamespaceID(args[2]);
                                    var variant = ParseArgumentNamespaceID(args[3]);

                                    var sprite = Main.ResourceManager.GetCharacterSprite(targetCharacter, variant);
                                    ui.SetCharacterSprite(characterIndex, sprite);
                                }
                                break;
                            case "variant":
                                {
                                    var targetCharacter = ParseArgumentNamespaceID(args[1]);
                                    var characterIndex = GetCharacterIndex(targetCharacter);
                                    if (characterIndex < 0)
                                        break;
                                    var variant = ParseArgumentNamespaceID(args[2]);

                                    var sprite = Main.ResourceManager.GetCharacterSprite(targetCharacter, variant);
                                    ui.SetCharacterSprite(characterIndex, sprite);
                                }
                                break;
                            case "leave":
                                {
                                    var characterId = ParseArgumentNamespaceID(args[1]);
                                    LeaveCharacter(characterId);
                                }
                                break;
                            case "faint":
                                {
                                    var characterId = ParseArgumentNamespaceID(args[1]);
                                    var duration = 1f;
                                    if (args.Length > 2)
                                    {
                                        duration = ParseArgumentFloat(args[2]);
                                    }

                                    var characterIndex = GetCharacterIndex(characterId);
                                    if (characterIndex < 0)
                                        break;
                                    ui.CharacterDisappear(characterIndex, 1 / duration);
                                }
                                break;
                        }
                    }
                    break;
                #endregion

                #region 贴图
                case "foreground":
                    {
                        switch (args[0])
                        {
                            case "change":
                                ui.SetForegroundSprite(Main.LanguageManager.GetSprite(ParseArgumentSpriteReference(args[1])));
                                break;
                            case "alpha":
                                ui.SetForegroundAlpha(ParseArgumentFloat(args[1]));
                                break;
                            case "fade":
                                if (args.Length >= 4)
                                {
                                    ui.SetForegroundAlpha(ParseArgumentFloat(args[1]));
                                    ui.StartForegroundFade(ParseArgumentFloat(args[2]), ParseArgumentFloat(args[3]));
                                }
                                else if (args.Length == 3)
                                {
                                    ui.StartForegroundFade(ParseArgumentFloat(args[1]), ParseArgumentFloat(args[2]));
                                }
                                else if (args.Length == 2)
                                {
                                    ui.StartForegroundFade(ParseArgumentFloat(args[1]), 1);
                                }
                                break;
                        }
                    }
                    break;
                case "portal":
                    {
                        switch (args[0])
                        {
                            case "set":
                                Main.Scene.SetPortalAlpha(ParseArgumentFloat(args[1]));
                                break;
                            case "fade":
                                {
                                    if (args.Length >= 4)
                                    {
                                        Main.Scene.SetPortalAlpha(ParseArgumentFloat(args[1]));
                                        Main.Scene.StartPortalFade(ParseArgumentFloat(args[2]), ParseArgumentFloat(args[3]));
                                    }
                                    else if (args.Length == 3)
                                    {
                                        Main.Scene.StartPortalFade(ParseArgumentFloat(args[1]), ParseArgumentFloat(args[2]));
                                    }
                                    else if (args.Length == 2)
                                    {
                                        Main.Scene.StartPortalFade(ParseArgumentFloat(args[1]), 1);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "forecolor":
                    {
                        switch (args[0])
                        {
                            case "set":
                                ui.SetForecolor(ParseArgumentColor(args[1]));
                                break;
                            case "fade":
                                {
                                    if (args.Length >= 4)
                                    {
                                        ui.SetForecolor(ParseArgumentColor(args[1]));
                                        ui.StartForecolorFade(ParseArgumentColor(args[2]), ParseArgumentFloat(args[3]));
                                    }
                                    else if (args.Length == 3)
                                    {
                                        ui.StartForecolorFade(ParseArgumentColor(args[1]), ParseArgumentFloat(args[2]));
                                    }
                                    else if (args.Length == 2)
                                    {
                                        ui.StartForecolorFade(ParseArgumentColor(args[1]), 1);
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case "shake":
                    {
                        float shakeAmp = 0.1f;
                        float shakeTime = 0.5f;
                        float endAmp = 0;
                        if (args.Length > 0)
                        {
                            shakeAmp = ParseArgumentFloat(args[0]);
                        }
                        if (args.Length > 1)
                        {
                            shakeTime = ParseArgumentFloat(args[1]);
                        }
                        if (args.Length > 2)
                        {
                            endAmp = ParseArgumentFloat(args[2]);
                        }
                        Main.ShakeManager.AddShake(shakeAmp, endAmp, shakeTime);
                    }
                    break;
                #endregion

                #region 音频
                case "music":
                    switch (args[0])
                    {
                        case "fade":
                            {
                                var fadeArgs = args.Skip(1).ToArray();
                                if (fadeArgs.Length >= 3)
                                {
                                    Main.MusicManager.SetVolume(ParseArgumentFloat(fadeArgs[0]));
                                    Main.MusicManager.StartFade(ParseArgumentFloat(fadeArgs[1]), ParseArgumentFloat(fadeArgs[2]));
                                }
                                else if (fadeArgs.Length == 2)
                                {
                                    Main.MusicManager.StartFade(ParseArgumentFloat(fadeArgs[0]), ParseArgumentFloat(fadeArgs[1]));
                                }
                                else if (fadeArgs.Length == 1)
                                {
                                    Main.MusicManager.StartFade(ParseArgumentFloat(fadeArgs[0]), ParseArgumentFloat(fadeArgs[1]));
                                }
                            }
                            break;
                        case "volume":
                            float volume = ParseArgumentFloat(args[1]);
                            Main.MusicManager.SetVolume(volume);
                            break;
                        case "play":
                            NamespaceID musicId = ParseArgumentNamespaceID(args[1]);
                            Main.MusicManager.Play(musicId);
                            break;
                    }
                    break;
                case "playsound":
                    {
                        Main.SoundManager.Play2D(ParseArgumentNamespaceID(args[0]));
                    }
                    break;
                #endregion

                #region 其他
                case "action":
                    ExecuteAction(args);
                    break;

                case "bubble":
                    ui.SetSpeechBubbleShowing(args[0] != "hide");
                    break;
                    #endregion
            }
        }
        /// <summary>
        /// 执行脚本。
        /// </summary>
        /// <param name="script">对话脚本。</param>
        private async Task ExecuteScriptAsync(TalkScript script)
        {
            var args = script.arguments;
            switch (script.function)
            {
                case "delay":
                    await Task.Delay((int)(ParseArgumentFloat(args[0]) * 1000));
                    break;
                default:
                    ExecuteScriptSync(script);
                    break;
            }
        }
        private async Task ExecuteScriptsAsync(IEnumerable<TalkScript> scripts)
        {
            if (scripts == null)
                return;
            IsRunningScripts = true;
            ui.SetBlockerActive(true);
            foreach (TalkScript scr in scripts)
            {
                await ExecuteScriptAsync(scr);
            }
            IsRunningScripts = false;
            ui.SetBlockerActive(false);
        }
        private void ExecuteScriptsSync(IEnumerable<TalkScript> scripts)
        {
            if (scripts == null)
                return;
            IsRunningScripts = true;
            ui.SetBlockerActive(true);
            foreach (TalkScript scr in scripts)
            {
                ExecuteScriptSync(scr);
            }
            IsRunningScripts = false;
            ui.SetBlockerActive(false);
        }

        private void ExecuteAction(string[] args)
        {
            if (OnTalkAction == null)
                return;
            if (args.Length <= 0)
                return;
            string func = args[0];
            string[] actionArgs = args.Skip(1).ToArray();
            try
            {
                OnTalkAction.Invoke(func, actionArgs);
            }
            catch (ArgumentException e)
            {
                Debug.Log($"动作\"{func}\"的参数数组大小({actionArgs.Length})不足：" + e);
            }
        }


        #endregion

        #region 指令 
        private async Task DelayedStart(float delay)
        {
            if (delay > 0)
            {
                await Task.Delay(Mathf.FloorToInt(delay * 1000));
            }

            var section = GetTalkSection();
            var characters = section.characters;
            if (characters != null)
            {
                foreach (TalkCharacter chr in characters)
                {
                    CreateCharacter(chr.id, chr.variant, ParseCharacterSide(chr.side));
                }
            }

            await Task.Delay(500);

            ui.SetBlockerActive(false);
            ui.SetSkipButtonActive(true);
            StartSentence();
        }
        #endregion

        #region 流程控制
        private TalkSection GetTalkSection()
        {
            return Main.ResourceManager.GetTalkSection(groupID, sectionIndex);
        }
        private TalkSentence GetTalkSentence()
        {
            return Main.ResourceManager.GetTalkSentence(groupID, sectionIndex, sentenceIndex);
        }
        /// <summary>
        /// 开始区间。
        /// </summary>
        public void StartSection(int index)
        {
            sectionIndex = index;
            sentenceIndex = 0;
            StartSentence();
        }

        /// <summary>
        /// 跳转到下一个句子。
        /// </summary>
        private void NextSentence()
        {
            sentenceIndex += 1;
            StartSentence();
        }

        /// <summary>
        /// 跳转到指定句子。
        /// </summary>
        /// <param name="index">指定句子的索引。</param>
        private void SetSentence(int index)
        {
            sentenceIndex = index;
            StartSentence();
        }

        /// <summary>
        /// 开始句子。
        /// </summary>
        private void StartSentence()
        {
            // 获取脚本组。
            var sentence = GetTalkSentence();

            foreach (var sound in sentence.sounds)
            {
                Main.SoundManager.Play2D(sound);
            }

            IEnumerable<TalkScript> scripts = sentence.startScripts != null ? sentence.startScripts : defaultStartScripts;
            // 执行脚本组。
            _ = ExecuteScriptsAsync(scripts);

            NamespaceID speakerID = sentence.speaker;
            // 对话状态。
            for (int i = 0; i < characterList.Count; i++)
            {
                var charId = characterList[i];
                bool isSpeaker = speakerID == charId;
                ui.SetCharacterSpeaking(i, isSpeaker);
            }

            // 切换变种贴图。
            var speakerIndex = GetCharacterIndex(speakerID);
            if (speakerIndex >= 0 && NamespaceID.IsValid(sentence.variant))
            {
                var sprite = Main.ResourceManager.GetCharacterSprite(speakerID, sentence.variant);
                ui.SetCharacterSprite(speakerIndex, sprite);
            }

            // 气泡位置。
            SpeechBubbleDirection bubbleDirection = SpeechBubbleDirection.Up;
            if (speechBubbleDirDict.TryGetValue(speakerID, out var p))
            {
                bubbleDirection = p;
            }
            else if (speakerIndex >= 0)
            {
                bubbleDirection = ui.IsCharacterAtLeft(speakerIndex) ? SpeechBubbleDirection.Left : SpeechBubbleDirection.Right;
            }

            var context = VanillaStrings.GetTalkTextContext(groupID);
            var textKey = sentence.text;
            ui.SetSpeechBubbleText(Main.LanguageManager._p(context, textKey));
            ui.SetSpeechBubbleDirection(bubbleDirection);
            ui.SetSpeechBubbleShowing(true);
            ui.ForceReshowSpeechBubble();
        }

        private void EndTalk()
        {
            groupID = null;
            sectionIndex = -1;
            sentenceIndex = -1;

            IsTalking = false;

            for (int i = characterList.Count - 1; i >= 0; i--)
            {
                var characterID = characterList[i];
                LeaveCharacter(characterID);
            }
            ui.SetSpeechBubbleShowing(false);
            ui.SetSkipButtonActive(false);
            ui.SetBlockerActive(false);
            ui.SetRaycastReceiverActive(false);
            OnTalkEnd?.Invoke();
        }
        #endregion

        private int GetCharacterIndex(NamespaceID id)
        {
            return characterList.IndexOf(id);
        }
        public void CreateCharacter(NamespaceID characterId, NamespaceID variant, CharacterSide side)
        {
            Sprite sprite;
            if (!NamespaceID.IsValid(variant))
            {
                sprite = Main.ResourceManager.GetCharacterSprite(characterId);
            }
            else
            {
                sprite = Main.ResourceManager.GetCharacterSprite(characterId, variant);
            }

            var viewData = new TalkCharacterViewData()
            {
                name = characterId?.ToString(),
                side = side,
                sprite = sprite
                };
            ui.CreateCharacter(viewData);
            characterList.Add(characterId);
        }
        public bool RemoveCharacter(NamespaceID characterID)
        {
            var index = GetCharacterIndex(characterID);
            if (index < 0)
                return false;
            ui.RemoveCharacterAt(index);
            characterList.RemoveAt(index);
            return true;
        }
        private void LeaveCharacter(NamespaceID id)
        {
            var index = GetCharacterIndex(id);
            if (index < 0)
                return;
            ui.LeaveCharacterAt(index);
            characterList.RemoveAt(index);
        }
        public bool DestroyCharacter(NamespaceID characterID)
        {
            var index = GetCharacterIndex(characterID);
            if (index < 0)
                return false;
            ui.DestroyCharacterAt(index);
            characterList.RemoveAt(index);
            return true;
        }
        public void ClearCharacters()
        {
            ui.ClearCharacters();
            characterList.Clear();
        }

        #endregion

        #region 事件
        public event Action OnTalkEnd;
        public event Action<string, string[]> OnTalkAction;
        #endregion 动作

        #region 属性字段
        public bool IsRunningScripts { get; private set; }
        public bool IsTalking { get; private set; }
        public readonly static NamespaceID DEFAULT_VARIANT_ID = new NamespaceID("mvz2", "normal");
        private readonly static Dictionary<NamespaceID, SpeechBubbleDirection> speechBubbleDirDict = new Dictionary<NamespaceID, SpeechBubbleDirection>()
        {
            { new NamespaceID("mvz2", "self"), SpeechBubbleDirection.Down },
        };
        private readonly static TalkScript[] defaultStartScripts = new TalkScript[0];
        private readonly static TalkScript[] defaultClickScripts = new TalkScript[]
        {
            new TalkScript()
            {
                function = "next",
                arguments = Array.Empty<string>()
            }
        };
        private readonly static TalkScript[] defaultSkipScripts = new TalkScript[]
        {
            new TalkScript()
            {
                function = "end",
                arguments = Array.Empty<string>()
            }
        };
        private MainManager Main => MainManager.Instance;

        private int sectionIndex = 0;
        private int sentenceIndex = 0;
        private NamespaceID groupID;
        private List<NamespaceID> characterList = new List<NamespaceID>();
        [SerializeField]
        private TalkUI ui;
        #endregion 属性
    }
}