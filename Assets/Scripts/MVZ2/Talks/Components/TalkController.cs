using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MVZ2.GameContent;
using MVZ2.UI;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Talk
{
    public class TalkController : MonoBehaviour, ITalkSystem
    {
        #region 公有方法
        /// <summary>
        /// 开始进行对话。
        /// </summary>
        public void StartTalk(NamespaceID groupId, int section, float delay = 0)
        {
            gameObject.SetActive(true);
            IsTalking = true;


            groupID = groupId;
            sectionIndex = section;
            sentenceIndex = 0;

            speechBubble.SetShowing(false);
            blockerObject.SetActive(true);
            raycastReceiver.gameObject.SetActive(true);
            ClearCharacters();

            StartCoroutine(DelayedStart(delay));
        }
        public TalkCharacterController CreateCharacter(NamespaceID characterId, NamespaceID variant, CharacterSide side)
        {
            TalkCharacterController chr = Instantiate(characterTemplate, characterRoot).GetComponent<TalkCharacterController>();
            var xScale = side == CharacterSide.Left ? -1 : 1;
            var scale = new Vector3(xScale, 1, 1);
            chr.SetScale(scale);

            Sprite sprite;
            if (!NamespaceID.IsValid(variant))
            {
                sprite = Main.ResourceManager.GetCharacterSprite(characterId);
            }
            else
            {
                sprite = Main.ResourceManager.GetCharacterSprite(characterId, variant);
            }
            chr.SetCharacter(sprite);
            chr.gameObject.SetActive(true);
            dialogCharacters.Add(characterId, chr);
            return chr;
        }


        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            characterTemplate.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);

            skipButton.onClick.AddListener(OnSkipClicked);
            raycastReceiver.OnPointerDown += OnClick;
            foregroundFader.OnValueChanged += OnForegroundAlphaChanged;
            forecolorFader.OnValueChanged += OnForegroundColorChanged;
        }
        #endregion

        #region 事件回调
        private void OnClick(PointerEventData eventData)
        {
            var sentence = GetTalkSentence();
            IEnumerable<TalkScript> scripts = sentence.clickScripts != null ? sentence.clickScripts : defaultClickScripts;
            StartCoroutine(ExecuteScripts(scripts));
        }
        private void OnForegroundAlphaChanged(float value)
        {
            foregroundCanvasGroup.alpha = value;
        }
        private void OnForegroundColorChanged(Color value)
        {
            var color = foregroundImage.color;
            color.r = value.r;
            color.g = value.g;
            color.b = value.b;
            foregroundImage.color = color;
        }
        private void OnSkipClicked()
        {
            var section = GetTalkSection();
            IEnumerable<TalkScript> scripts = section.skipScripts != null ? section.skipScripts : defaultSkipScripts;
            StartCoroutine(ExecuteScripts(scripts));
        }
        #endregion

        #region 脚本
        private int ParseArgumentInt(string str)
        {
            return int.Parse(str);
        }
        private float ParseArgumentFloat(string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
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

        /// <summary>
        /// 执行脚本。
        /// </summary>
        /// <param name="script">对话脚本。</param>
        private IEnumerator ExecuteScript(TalkScript script)
        {
            switch (script.function)
            {
                case "action":
                    ExecuteAction(script.arguments);
                    break;

                case "bubble":
                    speechBubble.SetShowing(script.arguments[0] != "hide");
                    break;

                case "changecharacter":
                    {
                        var chr = dialogCharacters[ParseArgumentNamespaceID(script.arguments[0])];
                        var characterId = ParseArgumentNamespaceID(script.arguments[1]);
                        var variant = ParseArgumentNamespaceID(script.arguments[2]);

                        var sprite = Main.ResourceManager.GetCharacterSprite(characterId, variant);
                        chr.SetCharacter(sprite);
                    }
                    break;

                case "variant":
                    {
                        var characterId = ParseArgumentNamespaceID(script.arguments[0]);
                        var chr = dialogCharacters[characterId];
                        var variant = ParseArgumentNamespaceID(script.arguments[1]);

                        var sprite = Main.ResourceManager.GetCharacterSprite(characterId, variant);
                        chr.SetCharacter(sprite);
                    }
                    break;

                case "delay":
                    yield return new WaitForSeconds(ParseArgumentFloat(script.arguments[0]));
                    break;

                case "createcharacter":
                    {
                        var characterId = ParseArgumentNamespaceID(script.arguments[0]);
                        CharacterSide side = ParseCharacterSide(script.arguments[1]);
                        var variant = script.arguments.Length > 2 ? ParseArgumentNamespaceID(script.arguments[2]) : DEFAULT_VARIANT_ID;

                        CreateCharacter(characterId, variant, side);
                    }
                    break;

                case "leave":
                    {
                        var characterId = ParseArgumentNamespaceID(script.arguments[0]);
                        dialogCharacters[characterId].SetLeaving(true);
                    }
                    break;
                case "faint":
                    {
                        var characterId = ParseArgumentNamespaceID(script.arguments[0]);
                        var chr = dialogCharacters[characterId];
                        chr.SetDisappear(true);
                        chr.SetDisappearSpeed(1 / ParseArgumentFloat(script.arguments[1]));
                    }
                    break;
                case "foreground":
                    {
                        switch (script.arguments[0])
                        {
                            case "change":
                                foregroundImage.sprite = Main.LanguageManager.GetSprite(ParseArgumentSpriteReference(script.arguments[1]));
                                break;
                            case "alpha":
                                foregroundFader.Value = ParseArgumentFloat(script.arguments[1]);
                                break;
                            case "fade":
                                foregroundFader.StartFade(ParseArgumentFloat(script.arguments[1]), script.arguments.Length > 2 ? ParseArgumentFloat(script.arguments[2]) : 1);
                                break;
                        }
                    }
                    break;
                case "music":
                    switch (script.arguments[0])
                    {
                        case "fade":
                            float target = ParseArgumentFloat(script.arguments[1]);
                            float duration = script.arguments.Length > 2 ? ParseArgumentFloat(script.arguments[2]) : 1;
                            Main.MusicManager.StartFade(target, duration);
                            break;
                        case "volume":
                            float volume = ParseArgumentFloat(script.arguments[1]);
                            Main.MusicManager.SetVolume(volume);
                            break;
                        case "play":
                            NamespaceID musicId = ParseArgumentNamespaceID(script.arguments[1]);
                            Main.MusicManager.Play(musicId);
                            break;
                    }
                    break;
                case "next":
                    NextSentence();
                    break;
                case "playsound":
                    {
                        Main.SoundManager.Play2D(ParseArgumentNamespaceID(script.arguments[0]));
                    }
                    break;
                case "forecolor":
                    {
                        ColorFader fader = forecolorFader;
                        switch (script.arguments[0])
                        {
                            case "set":
                                fader.Value = ParseArgumentColor(script.arguments[1]);
                                break;
                            case "fade":
                                fader.StartFade(ParseArgumentColor(script.arguments[1]), script.arguments.Length > 2 ? ParseArgumentFloat(script.arguments[2]) : 1);
                                break;
                        }
                    }
                    break;
                case "shake":
                    {
                        float shakeAmp = 0.1f;
                        float shakeTime = 0.5f;
                        float endAmp = 0;
                        if (script.arguments.Length > 0)
                        {
                            shakeAmp = ParseArgumentFloat(script.arguments[0]);
                        }
                        if (script.arguments.Length > 1)
                        {
                            shakeTime = ParseArgumentFloat(script.arguments[1]);
                        }
                        if (script.arguments.Length > 2)
                        {
                            endAmp = ParseArgumentFloat(script.arguments[2]);
                        }
                        Main.ShakeManager.AddShake(shakeAmp, endAmp, shakeTime);
                    }
                    break;

                case "section":
                    StartSection(ParseArgumentInt(script.arguments[0]));
                    break;

                case "sentence":
                    SetSentence(ParseArgumentInt(script.arguments[0]));
                    break;

                case "end":
                    NamespaceID endMode;
                    if (script.arguments.Length > 0)
                    {
                        endMode = NamespaceID.Parse(script.arguments[0], Main.BuiltinNamespace);
                    }
                    else
                    {
                        endMode = new NamespaceID(Main.BuiltinNamespace, "none");
                    }
                    EndTalk(endMode);
                    break;
            }
        }

        private IEnumerator ExecuteScripts(IEnumerable<TalkScript> scripts)
        {
            if (scripts != null)
            {
                IsRunningScripts = true;
                if (!blockerObject.activeSelf)
                {
                    blockerObject.SetActive(true);
                }
                foreach (TalkScript scr in scripts)
                {
                    yield return ExecuteScript(scr);
                }
                IsRunningScripts = false;
                if (blockerObject.activeSelf)
                {
                    blockerObject.SetActive(false);
                }
            }
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

        void ITalkSystem.StartSection(int section)
        {
            StartSection(section);
        }


        #endregion

        #region 指令 
        private IEnumerator DelayedStart(float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            var section = GetTalkSection();
            var characters = section.characters;
            if (characters != null)
            {
                foreach (TalkCharacter chr in characters)
                {
                    CreateCharacter(chr.id, chr.variant, ParseCharacterSide(chr.side));
                }
            }

            yield return new WaitForSeconds(0.5f);

            blockerObject.SetActive(false);
            SetSkipButtonActive(true);
            StartSentence();
        }
        #endregion

        #region UI
        private void SetSkipButtonActive(bool value)
        {
            skipButton.gameObject.SetActive(value);
        }
        #endregion

        private TalkGroup GetTalkGroup()
        {
            return Main.ResourceManager.GetTalkGroup(groupID);
        }
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
        private void StartSection(int index)
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
            StartCoroutine(ExecuteScripts(scripts));

            NamespaceID speakerID = sentence.speaker;
            foreach (NamespaceID charId in dialogCharacters.Keys)
            {
                var character = dialogCharacters[charId];
                character.SetSpeaking(speakerID == charId);
            }
            SpeechBubbleDirection bubbleDirection = SpeechBubbleDirection.Up;
            if (speechBubbleDirDict.TryGetValue(speakerID, out var p))
            {
                bubbleDirection = p;
            }
            else if (dialogCharacters.TryGetValue(speakerID, out var chr))
            {
                bubbleDirection = chr.IsLeft() ? SpeechBubbleDirection.Left : SpeechBubbleDirection.Right;
            }

            var context = GetCurrentTextContext();
            var textKey = sentence.text;
            speechBubble.SetText(Main.LanguageManager._p(context, textKey));
            speechBubble.UpdateBubble(bubbleDirection);
            speechBubble.SetShowing(true);
            speechBubble.ForceReshow();
        }

        private void EndTalk(NamespaceID mode)
        {
            groupID = null;
            sectionIndex = -1;
            sentenceIndex = -1;

            IsTalking = false;

            speechBubble.SetShowing(false);

            SetSkipButtonActive(false);

            foreach (var pair in dialogCharacters)
            {
                TalkCharacterController character = pair.Value;
                character.SetLeaving(true);
            }
            dialogCharacters.Clear();
            blockerObject.SetActive(false);
            raycastReceiver.gameObject.SetActive(false);
            OnTalkEnd?.Invoke(mode);
        }
        private void ClearCharacters()
        {
            foreach (var pair in dialogCharacters)
            {
                var character = pair.Value;
                Destroy(character.gameObject);
            }
            dialogCharacters.Clear();
        }
        private string GetCurrentTextContext()
        {
            return $"talk-{groupID.spacename}:{groupID.path}";
        }
        #endregion

        #region 事件
        public event Action<NamespaceID> OnTalkEnd;
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
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private GameObject characterTemplate;
        [SerializeField]
        private Transform characterRoot;
        [SerializeField]
        private SpeechBubble speechBubble;
        [SerializeField]
        private Button skipButton;
        [SerializeField]
        private RaycastReceiver raycastReceiver;
        [SerializeField]
        private GameObject blockerObject;
        [SerializeField]
        private Image foregroundImage;
        [SerializeField]
        private CanvasGroup foregroundCanvasGroup;
        [SerializeField]
        private FloatFader foregroundFader;
        [SerializeField]
        private ColorFader forecolorFader;

        private int sectionIndex = 0;
        private int sentenceIndex = 0;
        private NamespaceID groupID;


        // 存在的人物 (人物ID - 人物实例)
        private Dictionary<NamespaceID, TalkCharacterController> dialogCharacters = new Dictionary<NamespaceID, TalkCharacterController>();
        #endregion 属性
    }
}