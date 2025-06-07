using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.TalkData;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Talk
{
    public class TalkController : MonoBehaviour
    {
        #region 公有方法
        /// <summary>
        /// 开始进行对话。
        /// </summary>
        public async void StartTalk(NamespaceID groupId, int sectionIndex, float delay = 0, Action onEnd = null)
        {
            await StartTalkAsync(groupId, sectionIndex, delay);
            onEnd?.Invoke();
        }
        /// <summary>
        /// 开始进行对话。
        /// </summary>
        public async Task StartTalkAsync(NamespaceID groupId, int sectionIndex, float delay = 0)
        {
            var group = Main.ResourceManager.GetTalkGroup(groupId);
            if (group == null)
                throw new ArgumentException($"Could not find talk group with id {groupId}.");
            if (IsTalking)
                return;
            tcs = new TaskCompletionSource<object>();
            IsTalking = true;

            groupID = groupId;
            this.sectionIndex = sectionIndex;
            sentenceIndex = 0;

            ui.SetSpeechBubbleShowing(false);
            ui.SetBlockerActive(true);
            ui.SetRaycastReceiverActive(true);
            ui.SetForecolor(Color.clear);
            ui.SetBackcolor(Color.clear);
            ui.SetForegroundAlpha(0);
            ui.SetBackgroundAlpha(0);
            ui.SetForegroundSprite(null);
            ui.SetBackgroundSprite(null);
            ClearCharacters();

            // 执行开始指令。
            var section = group.sections[sectionIndex];
            await ExecuteScriptsAsync(section.startScripts);

            // 延迟。
            if (delay > 0)
            {
                await Main.CoroutineManager.DelaySeconds(delay);
            }
            // 延迟完毕。
            // 创建角色。
            var characters = section.characters;
            if (characters != null)
            {
                foreach (TalkCharacter chr in characters)
                {
                    CreateCharacter(chr.id, chr.variant, ParseCharacterSide(chr.side));
                }
            }
            // 延迟半秒。
            await Main.CoroutineManager.DelaySeconds(0.5f);

            // 设置阻挡和跳过按钮。
            ui.SetBlockerActive(false);
            ui.SetSkipButtonActive(true);

            // 开始语句。
            canClick = true;
            StartSentence();

            await tcs.Task;
        }
        public async void SkipTalk(NamespaceID groupId, int sectionIndex, Action onSkipped = null)
        {
            await SkipTalkAsync(groupId, sectionIndex);
            onSkipped?.Invoke();
        }
        public async Task SkipTalkAsync(NamespaceID groupId, int sectionIndex)
        {
            var meta = Main.ResourceManager.GetTalkGroup(groupId);
            if (meta != null && meta.archive != null)
            {
                var dialogName = Main.LanguageManager._p(VanillaStrings.CONTEXT_ARCHIVE, meta.archive.name);
                var popup = Main.LanguageManager._(DIALOG_SKIPPED, dialogName);
                Main.Scene.ShowPopup(popup);
            }

            var section = Main.ResourceManager.GetTalkSection(groupId, sectionIndex);
            await ExecuteScriptsAsync(section.startScripts);
            await ExecuteScriptsAsync(section.skipScripts);
        }
        public bool CanStartTalk(NamespaceID groupId, int sectionIndex)
        {
            return Main.ResourceManager.CanStartTalk(groupId, sectionIndex);
        }
        public bool WillSkipTalk(NamespaceID groupId, int sectionIndex)
        {
            return Main.ResourceManager.WillSkipTalk(groupId, sectionIndex);
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            ui.OnSkipClick += OnSkipClickedCallback;
            ui.OnClick += OnClickCallback;
        }
        private void Update()
        {
            ui.SetShake(((Vector3)Main.ShakeManager.GetShake2D()) * 100);
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
            if (!canClick)
                return;
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
            switch (side)
            {
                case "left":
                    return CharacterSide.Left;
                case "right":
                    return CharacterSide.Right;
                case "self":
                    return CharacterSide.Self;
                default:
                    return CharacterSide.None;
            }
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
                                    var meta = Main.ResourceManager.GetCharacterMeta(targetCharacter);
                                    var variantMeta = meta.variants.FirstOrDefault(v => v.id == variant);
                                    Vector2 widthExtend = Vector2.zero;
                                    if (variantMeta != null)
                                    {
                                        widthExtend = variantMeta.widthExtend;
                                    }
                                    ui.SetCharacterSprite(characterIndex, sprite);
                                    ui.SetCharacterWidthExtend(characterIndex, widthExtend);
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
                                    FaintCharacter(characterId, duration);
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
                                ui.SetForegroundSprite(Main.GetFinalSprite(ParseArgumentSpriteReference(args[1])));
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
                case "background":
                    {
                        switch (args[0])
                        {
                            case "change":
                                ui.SetBackgroundSprite(Main.GetFinalSprite(ParseArgumentSpriteReference(args[1])));
                                break;
                            case "alpha":
                                ui.SetBackgroundAlpha(ParseArgumentFloat(args[1]));
                                break;
                            case "fade":
                                if (args.Length >= 4)
                                {
                                    ui.SetBackgroundAlpha(ParseArgumentFloat(args[1]));
                                    ui.StartBackgroundFade(ParseArgumentFloat(args[2]), ParseArgumentFloat(args[3]));
                                }
                                else if (args.Length == 3)
                                {
                                    ui.StartBackgroundFade(ParseArgumentFloat(args[1]), ParseArgumentFloat(args[2]));
                                }
                                else if (args.Length == 2)
                                {
                                    ui.StartBackgroundFade(ParseArgumentFloat(args[1]), 1);
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
                                if (args.Length >= 4)
                                {
                                    Main.MusicManager.SetVolume(ParseArgumentFloat(args[1]));
                                    Main.MusicManager.StartFade(ParseArgumentFloat(args[2]), ParseArgumentFloat(args[3]));
                                }
                                else if (args.Length == 3)
                                {
                                    Main.MusicManager.StartFade(ParseArgumentFloat(args[1]), ParseArgumentFloat(args[2]));
                                }
                                else if (args.Length == 2)
                                {
                                    Main.MusicManager.StartFade(ParseArgumentFloat(args[1]), 1);
                                }
                            }
                            break;
                        case "volume":
                            float volume = ParseArgumentFloat(args[1]);
                            Main.MusicManager.SetVolume(volume);
                            Main.MusicManager.StopFade();
                            break;
                        case "play":
                            NamespaceID musicId = ParseArgumentNamespaceID(args[1]);
                            Main.MusicManager.Play(musicId);
                            break;
                        case "stop":
                            Main.MusicManager.Stop();
                            break;
                    }
                    break;
                case "playsound":
                    {
                        Main.SoundManager.Play2D(ParseArgumentNamespaceID(args[0]));
                    }
                    break;
                case "loopsound":
                    switch (args[0])
                    {
                        case "play":
                            {
                                NamespaceID soundId = ParseArgumentNamespaceID(args[1]);
                                Main.SoundManager.PlayLoopSound(soundId);
                            }
                            break;
                        case "stop":
                            {
                                NamespaceID soundId = ParseArgumentNamespaceID(args[1]);
                                Main.SoundManager.StopLoopSound(soundId);
                            }
                            break;
                        case "volume":
                            {
                                NamespaceID soundId = ParseArgumentNamespaceID(args[1]);
                                float volume = ParseArgumentFloat(args[2]);
                                Main.SoundManager.SetLoopSoundIntensity(soundId, volume);
                                Main.SoundManager.StopFadeLoopSound(soundId);
                            }
                            break;
                        case "fade":
                            {
                                NamespaceID soundId = ParseArgumentNamespaceID(args[1]);
                                if (args.Length >= 5)
                                {
                                    Main.SoundManager.SetLoopSoundIntensity(soundId, ParseArgumentFloat(args[2]));
                                    Main.SoundManager.StartFadeLoopSound(soundId, ParseArgumentFloat(args[3]), ParseArgumentFloat(args[4]));
                                }
                                else if (args.Length == 4)
                                {
                                    Main.SoundManager.StartFadeLoopSound(soundId, ParseArgumentFloat(args[2]), ParseArgumentFloat(args[3]));
                                }
                                else if (args.Length == 3)
                                {
                                    Main.SoundManager.StartFadeLoopSound(soundId, ParseArgumentFloat(args[2]), 1);
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 展示物品
                case "item":
                    {
                        switch (args[0])
                        {
                            case "show":
                                var sprite = Main.GetFinalSprite(ParseArgumentSpriteReference(args[1]));
                                ui.ShowTalkItem(sprite);
                                showingTalkItem = true;
                                Main.SoundManager.Play2D(VanillaSoundID.dialogItemShow);
                                break;
                            case "hide":
                                if (showingTalkItem)
                                {
                                    showingTalkItem = false;
                                    ui.HideTalkItem();
                                    Main.SoundManager.Play2D(VanillaSoundID.dialogItemHide);
                                }
                                break;
                        }
                    }
                    break;
                #endregion

                #region 其他
                case "unlock":
                    {
                        if (!canUnlock)
                            break;
                        if (args.Length <= 0)
                            break;
                        Main.SaveManager.Unlock(ParseArgumentNamespaceID(args[0]));
                    }
                    break;

                case "relock":
                    {
                        if (!canUnlock)
                            break;
                        if (args.Length <= 0)
                            break;
                        Main.SaveManager.Relock(ParseArgumentNamespaceID(args[0]));
                    }
                    break;

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
                    canClick = false;
                    await Main.CoroutineManager.DelaySeconds(ParseArgumentFloat(args[0]));
                    canClick = true;
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

            if (sentence.sounds != null)
            {
                foreach (var sound in sentence.sounds)
                {
                    Main.SoundManager.Play2D(sound);
                }
            }

            IEnumerable<TalkScript> scripts = sentence.startScripts != null ? sentence.startScripts : defaultStartScripts;
            // 执行脚本组。
            _ = ExecuteScriptsAsync(scripts);

            NamespaceID speakerID = sentence.speaker;
            // 对话状态。
            for (int i = 0; i < characterList.Count; i++)
            {
                var characterData = characterList[i];
                bool isSpeaker = speakerID == characterData.id;
                ui.SetCharacterSpeaking(i, isSpeaker);
                if (isSpeaker)
                {
                    ui.SetCharacterToTheFirstLayer(i);
                }
            }

            // 切换变种贴图。
            var speakerIndex = GetCharacterIndex(speakerID);
            if (speakerIndex >= 0 && NamespaceID.IsValid(sentence.variant))
            {
                var sprite = Main.ResourceManager.GetCharacterSprite(speakerID, sentence.variant);
                ui.SetCharacterSprite(speakerIndex, sprite);
            }

            var context = VanillaStrings.GetTalkTextContext(groupID);
            var textKey = sentence.text;
            var bubbleText = Main.LanguageManager._p(context, textKey);


            string speakerName = sentence.GetSpeakerName(Main);

            // 气泡位置。
            SpeechBubbleDirection bubbleDirection = SpeechBubbleDirection.Up;
            bool showSpeakerName = false;
            if (speakerIndex >= 0)
            {
                var characterData = characterList[speakerIndex];
                bubbleDirection = GetSpeechBubbleDirectionBySide(characterData.side);
            }
            else
            {
                bubbleDirection = SpeechBubbleDirection.Up;
                showSpeakerName = true;
            }

            if (ui.GetForegroundAlpha() > 0.1f && bubbleDirection != SpeechBubbleDirection.Up)
            {
                bubbleDirection = SpeechBubbleDirection.Down;
                showSpeakerName = true;
            }
            if (showSpeakerName)
            {
                bubbleText = Main.LanguageManager._p(VanillaStrings.CONTEXT_TALK, FORGROUND_TALK_TEMPLATE, speakerName, bubbleText);
            }
            ui.SetSpeechBubbleText(bubbleText);
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
            canClick = false;

            for (int i = characterList.Count - 1; i >= 0; i--)
            {
                var characterData = characterList[i];
                LeaveCharacter(characterData.id);
            }
            ui.SetSpeechBubbleShowing(false);
            ui.SetSkipButtonActive(false);
            ui.SetBlockerActive(false);
            ui.SetRaycastReceiverActive(false);

            ui.StartBackcolorFade(Color.clear, 1);
            ui.StartForecolorFade(Color.clear, 1);
            ui.StartBackgroundFade(0, 1);
            ui.StartForegroundFade(0, 1);
            if (tcs != null)
            {
                var source = tcs;
                tcs = null;
                source.SetResult(null);
            }
        }
        #endregion

        private int GetCharacterIndex(NamespaceID id)
        {
            return characterList.FindIndex(d => d.id == id);
        }
        public void CreateCharacter(NamespaceID characterId, NamespaceID variant, CharacterSide side)
        {
            var viewData = Main.ResourceManager.GetCharacterViewData(characterId, variant, side);
            ui.CreateCharacter(viewData);
            characterList.Add(new CharacterData()
            {
                id = characterId,
                side = side,
            });
        }
        public bool RemoveCharacter(NamespaceID characterID)
        {
            var index = GetCharacterIndex(characterID);
            if (index < 0)
                return false;
            characterList.RemoveAt(index);
            ui.RemoveCharacterAt(index);
            return true;
        }
        private void LeaveCharacter(NamespaceID id)
        {
            var index = GetCharacterIndex(id);
            if (index < 0)
                return;
            characterList.RemoveAt(index);
            ui.LeaveCharacterAt(index);
        }
        private void FaintCharacter(NamespaceID id, float duration)
        {
            var index = GetCharacterIndex(id);
            if (index < 0)
                return;
            characterList.RemoveAt(index);
            ui.CharacterDisappear(index, 1 / duration);
        }
        public bool DestroyCharacter(NamespaceID characterID)
        {
            var index = GetCharacterIndex(characterID);
            if (index < 0)
                return false;
            characterList.RemoveAt(index);
            ui.DestroyCharacterAt(index);
            return true;
        }
        public void ClearCharacters()
        {
            ui.ClearCharacters();
            characterList.Clear();
        }
        private SpeechBubbleDirection GetSpeechBubbleDirectionBySide(CharacterSide side)
        {
            switch (side)
            {
                case CharacterSide.Left:
                    return SpeechBubbleDirection.Left;
                case CharacterSide.Right:
                    return SpeechBubbleDirection.Right;
                case CharacterSide.Self:
                    return SpeechBubbleDirection.Down;
                default:
                    return SpeechBubbleDirection.Up;
            }
        }

        #endregion

        #region 事件
        public event Action<string, string[]> OnTalkAction;
        #endregion 动作

        #region 属性字段
        [TranslateMsg("前景图生效时的对话模板，{0}为讨论者，{1}为文本")]
        public const string FORGROUND_TALK_TEMPLATE = "<color=blue>[{0}]</color>\n{1}";
        [TranslateMsg("跳过对话时的提示，{0}为对话名称")]
        public const string DIALOG_SKIPPED = "已跳过对话\"{0}\"";
        public bool IsRunningScripts { get; private set; }
        public bool IsTalking { get; private set; }
        public readonly static NamespaceID DEFAULT_VARIANT_ID = new NamespaceID("mvz2", "normal");
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

        private bool showingTalkItem = false;
        private bool canClick = false;
        private int sectionIndex = 0;
        private int sentenceIndex = 0;
        private NamespaceID groupID;
        private List<CharacterData> characterList = new List<CharacterData>();
        private TaskCompletionSource<object> tcs;
        [SerializeField]
        private TalkUI ui;
        [SerializeField]
        private bool canUnlock = true;
        #endregion 属性

        public class CharacterData
        {
            public NamespaceID id;
            public CharacterSide side;
        }
    }
}