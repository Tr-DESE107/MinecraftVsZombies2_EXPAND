using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.GameContent.Maps;
using MVZ2.GameContent.Talk;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Options;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2.Talk;
using MVZ2.Talks;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.Map;
using MVZ2Logic.Maps;
using MVZ2Logic.Scenes;
using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Map
{
    public class MapController : MainScenePage, IMapInterface
    {
        #region 公有方法
        public override void Display()
        {
            base.Display();
            ResetCamera();
            ui.SetButtonActive(MapUI.ButtonType.Almanac, Main.SaveManager.IsAlmanacUnlocked());
            ui.SetButtonActive(MapUI.ButtonType.Store, Main.SaveManager.IsStoreUnlocked());
            ui.SetButtonActive(MapUI.ButtonType.Map, Main.SaveManager.IsGensokyoUnlocked());
            ui.SetHintText(Main.LanguageManager._(Main.IsMobile() ? HINT_TEXT_MOBILE : HINT_TEXT));
            ui.SetDragRootVisible(false);
            ui.SetOptionsDialogActive(false);
            ui.SetRaycastBlockerActive(false);
            if (!Main.SoundManager.IsPlaying(VanillaSoundID.travel))
            {
                Main.SoundManager.Play2D(VanillaSoundID.travel);
            }
            Main.Scene.SetPortalAlpha(1);
            Main.Scene.PortalFadeOut();
        }
        public override void Hide()
        {
            base.Hide();
            if (model)
            {
                model.OnMapButtonClick -= OnMapButtonClickCallback;
                model.OnEndlessButtonClick -= OnEndlessButtonClickCallback;
                model.OnMapKeyClick -= OnMapKeyClickCallback;
                model.OnMapNightmareBoxClick -= OnMapNightmareBoxClickCallback;
                model.OnMapPinClick -= OnMapPinClickCallback;
                Destroy(model.gameObject);
                model = null;
            }
            SetCameraBackgroundColor(Color.black);
        }
        public async void SetMap(NamespaceID mapId)
        {
            MapID = mapId;
            mapMeta = Main.ResourceManager.GetMapMeta(mapId);
            if (mapMeta == null)
                return;

            var unlockedPresets = mapMeta.presets.Where(p => p.conditions == null || Main.SaveManager.MeetsXMLConditions(p.conditions));
            MapPreset mapPreset = unlockedPresets.OrderByDescending(p => p.priority).FirstOrDefault();
            SetMapPreset(mapPreset);

            Main.MusicManager.Play(mapPreset.music);
            Main.SaveManager.SetLastMapID(mapId);

            if (mapId == VanillaMapID.gensokyo)
            {
                ui.SetButtonActive(MapUI.ButtonType.Map, false);
            }

            // 对话
            HideUIArrows();
            // 上一次地图的对话
            var mapTalk = Main.SaveManager.GetMapTalk();
            var queue = new Queue<NamespaceID>();
            queue.Enqueue(mapTalk);

            // 地图自带剧情对话
            var mapLores = mapMeta.loreTalks?.GetLoreTalks(Main.SaveManager);
            if (mapLores != null)
            {
                foreach (var lore in mapLores)
                {
                    if (!queue.Contains(lore))
                        queue.Enqueue(lore);
                }
            }

            while (queue.Count > 0)
            {
                var talk = queue.Dequeue();
                if (!NamespaceID.IsValid(talk))
                    continue;
                Main.SaveManager.SetMapTalk(talk);
                await talkController.SimpleStartTalkAsync(talk, 0, 3);
            }
            UpdateUIArrows();
            Main.SaveManager.SetMapTalk(null);
        }
        public void SetMapPreset(MapPreset mapPreset)
        {
            if (mapPreset == null)
                return;
            var modelPrefab = Main.ResourceManager.GetMapModel(mapPreset.model);
            if (model)
            {
                model.OnMapButtonClick -= OnMapButtonClickCallback;
                model.OnEndlessButtonClick -= OnEndlessButtonClickCallback;
                model.OnMapKeyClick -= OnMapKeyClickCallback;
                model.OnMapNightmareBoxClick -= OnMapNightmareBoxClickCallback;
                model.OnMapPinClick -= OnMapPinClickCallback;
                Destroy(model.gameObject);
            }

            model = Instantiate(modelPrefab.gameObject, modelRoot).GetComponent<MapModel>();
            model.OnMapButtonClick += OnMapButtonClickCallback;
            model.OnEndlessButtonClick += OnEndlessButtonClickCallback;
            model.OnMapKeyClick += OnMapKeyClickCallback;
            model.OnMapNightmareBoxClick += OnMapNightmareBoxClickCallback;
            model.OnMapPinClick += OnMapPinClickCallback;

            UpdateModelButtons();
            UpdateModelElements();
            UpdateModelEndlessFlags();
            SetCameraBackgroundColor(mapPreset.backgroundColor);
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            ui.OnButtonClick += OnButtonClickCallback;
            talkController.OnTalkAction += OnTalkActionCallback;

            talkSystem = new MapTalkSystem(this, talkController);
        }
        private void Update()
        {
            UpdateTouchDatas();
            if (Input.touchCount > 0)
            {
                UpdateTouch();
            }
            else
            {
                UpdateMouse();
            }
            var maxCameraSize = mapMeta.size.y / 100 * 0.5f;
            mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize + cameraScaleSpeed, minCameraSize, maxCameraSize);
            LimitCameraPosition();
            cameraScaleSpeed *= 0.8f;
            mapCameraShakeRoot.localPosition = (Vector3)Main.ShakeManager.GetShake2D();
        }
        #endregion

        #region 事件回调
        private void OnButtonClickCallback(MapUI.ButtonType button)
        {
            switch (button)
            {
                case MapUI.ButtonType.Back:
                    Main.Scene.DisplayPage(MainScenePageType.Mainmenu);
                    Main.SaveManager.SaveModDatas();
                    break;
                case MapUI.ButtonType.Almanac:
                    Main.Scene.DisplayAlmanac(() => Main.Scene.DisplayMap(MapID));
                    break;
                case MapUI.ButtonType.Store:
                    Main.Scene.DisplayStore(() => Main.Scene.DisplayMap(MapID));
                    break;
                case MapUI.ButtonType.Map:
                    Main.Scene.DisplayMap(VanillaMapID.gensokyo);
                    break;
                case MapUI.ButtonType.Setting:
                    ui.SetOptionsDialogActive(true);
                    ui.OptionsDialog.ResetPosition();
                    optionsLogic = new OptionsLogicMap(ui.OptionsDialog);
                    optionsLogic.InitDialog();
                    optionsLogic.OnClose += OnOptionsDialogCloseCallback;
                    break;
            }
        }
        private void OnTalkActionCallback(string cmd, string[] parameters)
        {
            Global.Game.RunCallbackFiltered(VanillaCallbacks.TALK_ACTION, cmd, c => c(talkSystem, cmd, parameters));
        }
        private void OnOptionsDialogCloseCallback()
        {
            optionsLogic.OnClose -= OnOptionsDialogCloseCallback;
            optionsLogic.Dispose();
            optionsLogic = null;
            ui.SetOptionsDialogActive(false);
        }
        private void OnMapButtonClickCallback(int index)
        {
            var stageID = mapMeta.stages[index];
            StartCoroutine(EnterLevel(stageID));
        }
        private void OnEndlessButtonClickCallback()
        {
            var stageID = mapMeta.endlessStage;
            StartCoroutine(EnterLevel(stageID));
        }
        private async void OnMapKeyClickCallback()
        {
            ui.SetRaycastBlockerActive(true);
            if (!Main.SaveManager.IsUnlocked(VanillaUnlockID.enteredDream) && MapID == VanillaMapID.halloween)
            {
                await talkController.SimpleStartTalkAsync(VanillaTalkID.halloweenFinal, 0, 0);
            }
            else
            {
                Hide();
                if (MapID == VanillaMapID.halloween)
                {
                    Main.Scene.DisplayMap(VanillaMapID.dream);
                }
                else
                {
                    Main.Scene.DisplayMap(VanillaMapID.halloween);
                }
            }
        }
        private void OnMapNightmareBoxClickCallback()
        {
            if (Main.SaveManager.IsUnlocked(VanillaUnlockID.dreamIsNightmare))
            {
                Main.SaveManager.Relock(VanillaUnlockID.dreamIsNightmare);
            }
            else
            {
                Main.SaveManager.Unlock(VanillaUnlockID.dreamIsNightmare);
            }
            Main.Scene.DisplayMap(MapID);
        }
        private void OnMapPinClickCallback(NamespaceID id)
        {
            if (id == MapPinID.halloween)
            {
                Main.Scene.DisplayMap(VanillaMapID.halloween);
            }
            else if (id == MapPinID.dream)
            {
                Main.Scene.DisplayMap(VanillaMapID.dream);
            }
            else if (id == MapPinID.teruharijou)
            {
                Main.Scene.DisplayMap(VanillaMapID.teruharijou);
            }
            else if (id == MapPinID.kourindou)
            {
                Main.Scene.DisplayStore(() => Main.Scene.DisplayMap(MapID));
            }
        }
        #endregion

        void IMapInterface.SetPreset(NamespaceID presetID)
        {
            var preset = mapMeta.presets.FirstOrDefault(p => p.id == presetID);
            SetMapPreset(preset);
        }

        #region 相机
        private void ResetCamera()
        {
            mapCamera.transform.localPosition = Vector3.zero;
        }
        private void SetCameraBackgroundColor(Color color)
        {
            mapCamera.backgroundColor = color;
        }
        private void LimitCameraPosition()
        {
            var position = mapCamera.transform.position;
            var aspect = mapCamera.aspect;
            var fullHeight = mapCamera.orthographicSize * 2;
            var cameraHeight = mapCamera.rect.height * fullHeight;
            var cameraWidth = cameraHeight * aspect;

            var mapSize = mapMeta.size * 0.01f;
            var minX = -mapSize.x * 0.5f;
            var maxX = mapSize.x * 0.5f;
            var minY = -mapSize.y * 0.5f;
            var maxY = mapSize.y * 0.5f;
            position.x = Mathf.Clamp(position.x, minX + cameraWidth * 0.5f, maxX - cameraWidth * 0.5f);
            position.y = Mathf.Clamp(position.y, minY + cameraHeight * 0.5f, maxY - cameraHeight * 0.5f);
            mapCamera.transform.position = position;
        }
        #endregion

        #region 关卡
        private NamespaceID GetStageID(int index)
        {
            return mapMeta.stages[index];
        }
        private StageMeta GetStageMeta(NamespaceID stageID)
        {
            if (stageID == null)
                return null;
            return Main.ResourceManager.GetStageMeta(stageID);
        }
        private StageMeta GetStageMeta(int index)
        {
            var stageID = GetStageID(index);
            return GetStageMeta(stageID);
        }
        private string GetStageType(int index)
        {
            var stageMeta = GetStageMeta(index);
            if (stageMeta == null)
                return string.Empty;
            return stageMeta.Type;
        }
        private bool IsLevelUnlocked(NamespaceID stageID)
        {
            var stageMeta = GetStageMeta(stageID);
            if (stageMeta == null)
                return false;
            return Main.SaveManager.IsUnlocked(stageMeta.Unlock);
        }
        private bool IsLevelUnlocked(int index)
        {
            var stageID = GetStageID(index);
            return IsLevelUnlocked(stageID);
        }
        private NamespaceID GetLevelDifficulty(int index)
        {
            var stageID = GetStageID(index);
            if (!NamespaceID.IsValid(stageID))
                return null;
            var records = Main.SaveManager.GetLevelDifficultyRecords(stageID);
            return records.OrderByDescending(r =>
            {
                var meta = Main.ResourceManager.GetDifficultyMeta(r);
                if (meta == null)
                    return int.MinValue;
                return meta.value;
            }).FirstOrDefault();
        }
        private bool IsEndlessUnlocked()
        {
            var stageID = mapMeta.endlessStage;
            return IsLevelUnlocked(stageID);
        }
        private bool IsLevelCleared(int index)
        {
            return Main.SaveManager.IsLevelCleared(mapMeta.stages[index]);
        }
        #endregion

        #region 触摸输入
        private void UpdateTouchDatas()
        {
            touchDatas.RemoveAll(d => !Input.touches.Any(t => d.fingerId == t.fingerId));
            for (int i = 0; i < Input.touchCount; i++)
            {
                UpdateTouchData(i, Input.GetTouch(i));
            }
        }
        private void UpdateTouch()
        {
            if (touchDatas.Count > 1)
            {
                var touch0 = touchDatas[0];
                var touch1 = touchDatas[1];
                var position0 = (Vector2)mapCamera.ScreenToWorldPoint(touch0.position);
                var position1 = (Vector2)mapCamera.ScreenToWorldPoint(touch1.position);
                var lastPosition0 = (Vector2)mapCamera.ScreenToWorldPoint(touch0.position - touch0.delta);
                var lastPosition1 = (Vector2)mapCamera.ScreenToWorldPoint(touch1.position - touch1.delta);

                var lastLength = (lastPosition0 - lastPosition1).magnitude;
                var currentLength = (position0 - position1).magnitude;
                var scale = lastLength / currentLength;

                var lastCenter = (lastPosition0 + lastPosition1) * 0.5f;
                var currentCenter = (position0 + position1) * 0.5f;
                var motion = lastCenter - currentCenter;

                var maxCameraSize = mapMeta.size.y / 100 * 0.5f;
                mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize * scale, minCameraSize, maxCameraSize);
                mapCamera.transform.position += (Vector3)motion;
            }
            else if (touchDatas.Count > 0)
            {
                var touch0 = touchDatas[0];
                var position0 = (Vector2)mapCamera.ScreenToWorldPoint(touch0.position);
                var lastPosition0 = (Vector2)mapCamera.ScreenToWorldPoint(touch0.position - touch0.delta);

                var motion = lastPosition0 - position0;

                mapCamera.transform.position += (Vector3)motion;
            }
        }
        private TouchData GetTouchData(int fingerId)
        {
            return touchDatas.FirstOrDefault(t => t.fingerId == fingerId);
        }
        private void UpdateTouchData(int index, Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (IsPositionOnReceiver(touch.position))
                {
                    touchDatas.Add(new TouchData()
                    {
                        fingerId = touch.fingerId,
                        position = touch.position,
                    });
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                touchDatas.RemoveAll(t => t.fingerId == touch.fingerId);
            }
            else
            {
                var touchData = GetTouchData(touch.fingerId);
                if (touchData != null)
                {
                    touchData.delta = touch.deltaPosition;
                    touchData.position = touch.position;
                }
            }
        }
        #endregion

        #region 鼠标输入
        private void UpdateMouse()
        {
            var position = Input.mousePosition;
            if (Input.GetMouseButtonDown(1))
                OnRightMouseDown(position);
            if (Input.GetMouseButton(1))
                OnRightMouse(position);
            if (Input.GetMouseButtonUp(1))
                OnRightMouseUp();
            OnMouseScroll(position, Input.mouseScrollDelta);
        }
        private void OnRightMouseDown(Vector2 position)
        {
            if (!IsPositionOnReceiverOrButton(position))
                return;
            ui.SetDragRootVisible(true);
            ui.SetDragRootPosition(position);
            draggingView = true;
            mapDragStartPos = position;
        }
        private void OnRightMouse(Vector2 position)
        {
            if (!draggingView)
                return;
            ui.SetDragArrowTargetPosition(position);
            var cameraPos = mapCamera.transform.position;
            var targetWorldPos = (Vector2)mapCamera.ScreenToWorldPoint(position);
            var fromWorldPos = (Vector2)mapCamera.ScreenToWorldPoint(mapDragStartPos);
            cameraPos += (Vector3)((targetWorldPos - fromWorldPos) * 0.1f);
            mapCamera.transform.position = cameraPos;
        }
        private void OnRightMouseUp()
        {
            draggingView = false;
            ui.SetDragRootVisible(false);
        }
        private void OnMouseScroll(Vector2 position, Vector2 scrollDelta)
        {
            if (scrollDelta.y == 0)
                return;
            if (!IsPositionOnReceiverOrButton(position))
                return;
            cameraScaleSpeed = Mathf.Clamp(cameraScaleSpeed + -scrollDelta.y * 0.1f, -1, 1);
        }
        private bool IsPositionOnReceiver(Vector2 position)
        {
            var eventSystem = EventSystem.current;
            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = position
            };
            raycastResultCache.Clear();
            eventSystem.RaycastAll(pointerEventData, raycastResultCache);
            var first = raycastResultCache.FirstOrDefault(r => r.gameObject).gameObject;
            return !first || first == raycastHitbox;
        }
        private bool IsPositionOnReceiverOrButton(Vector2 position)
        {
            raycastResultCache.Clear();
            var eventSystem = EventSystem.current;
            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = position
            };
            eventSystem.RaycastAll(pointerEventData, raycastResultCache);
            var first = raycastResultCache.FirstOrDefault(r => r.gameObject).gameObject;
            return !first || first == raycastHitbox || first.GetComponentInParent<MapButton>();
        }
        #endregion

        private IEnumerator EnterLevel(NamespaceID stageID)
        {
            ui.SetHintText(Main.LanguageManager._(HINT_TEXT_ENTERING_LEVEL));
            ui.SetRaycastBlockerActive(true);
            Main.MusicManager.Stop();
            Main.SoundManager.Play2D(VanillaSoundID.spring);
            yield return new WaitForSeconds(1);
            var task = GotoLevelAsync(stageID);
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
        private async Task GotoLevelAsync(NamespaceID stageID)
        {
            Main.SaveManager.SaveModDatas();
            await Main.LevelManager.GotoLevelSceneAsync();
            Main.LevelManager.InitLevel(mapMeta.area, stageID);
            Hide();
        }

        private void UpdateModelButtons()
        {
            int unclearedMapButtonIndex = -1;
            for (int i = 0; i < model.GetMapButtonCount(); i++)
            {
                var unlocked = IsLevelUnlocked(i);
                var cleared = IsLevelCleared(i);
                var stageType = GetStageType(i);

                var color = buttonColorCleared;
                if (!unlocked)
                    color = buttonColorLocked;
                else if (stageType == StageTypes.TYPE_MINIGAME)
                    color = buttonColorMinigame;
                else if (stageType == StageTypes.TYPE_BOSS)
                    color = buttonColorBoss;
                else if (!cleared)
                    color = buttonColorUncleared;

                if (unlocked && !cleared)
                {
                    unclearedMapButtonIndex = i;
                }

                model.SetMapButtonInteractable(i, unlocked);
                model.SetMapButtonColor(i, color);
                model.SetMapButtonText(i, (i + 1).ToString());
                model.SetMapButtonDifficulty(i, GetLevelDifficulty(i));
            }
            var endlessColor = buttonColorEndless;
            var endlessUnlocked = IsEndlessUnlocked();
            if (!endlessUnlocked)
                endlessColor = buttonColorLocked;
            model.SetEndlessButtonInteractable(endlessUnlocked);
            model.SetEndlessButtonColor(endlessColor);
            model.SetEndlessButtonText("E");

            model.SetMapKeyActive(Main.SaveManager.IsUnlocked(VanillaUnlockID.halloween11));


            if (unclearedMapButtonIndex >= 0)
            {
                var unclearedMapButton = model.GetMapButton(unclearedMapButtonIndex);
                if (unclearedMapButton)
                {
                    var pos = unclearedMapButton.transform.position;
                    pos.z = mapCamera.transform.position.z;
                    mapCamera.transform.position = pos;
                }
            }
        }
        private void UpdateModelElements()
        {
            var unlocks = model.GetMapElementUnlocks();
            for (int i = 0; i < unlocks.Length; i++)
            {
                var unlock = unlocks[i];
                model.SetMapElementUnlocked(unlock, Main.SaveManager.IsUnlocked(unlock));
            }
        }
        private void UpdateModelEndlessFlags()
        {
            var currentFlags = GetEndlessFlags();
            var maxFlags = GetMaxEndlessFlags();
            var text = Main.LanguageManager._(ENDLESS_FLAGS_TEMPLATE, currentFlags, maxFlags);
            model.SetEndlessFlagsTextActive(IsEndlessUnlocked());
            model.SetEndlessFlagsText(text);
        }
        private void UpdateUIArrows()
        {
            var talks = Main.ResourceManager.GetCurrentStoreLoreTalks();
            ui.SetStoreArrowVisible(talks.Length > 0);
        }
        private void HideUIArrows()
        {
            ui.SetStoreArrowVisible(false);
        }
        private int GetEndlessFlags()
        {
            var stageID = mapMeta.endlessStage;
            return Main.SaveManager.GetCurrentEndlessFlag(stageID);
        }
        private int GetMaxEndlessFlags()
        {
            var stageID = mapMeta.endlessStage;
            if (!NamespaceID.IsValid(stageID))
                return 0;
            return (int)Main.SaveManager.GetSaveStat(VanillaStats.CATEGORY_MAX_ENDLESS_FLAGS, stageID);
        }

        #endregion

        [TranslateMsg("地图的提示文本")]
        public const string HINT_TEXT = "按住右键拖动以移动视图\n滚轮以缩放视图";
        [TranslateMsg("地图的提示文本")]
        public const string HINT_TEXT_MOBILE = "单指拖动以移动视图\n双指触摸以缩放视图";
        [TranslateMsg("地图的提示文本")]
        public const string HINT_TEXT_ENTERING_LEVEL = "正在进入关卡……";
        [TranslateMsg("地图的无尽模式提示文本，{0}为当前轮数，{1}为历史最高")]
        public const string ENDLESS_FLAGS_TEMPLATE = "轮数\n{0}/{1}";
        private MainManager Main => MainManager.Instance;
        private MapModel model;
        private MapMeta mapMeta;
        private bool draggingView;
        private Vector2 mapDragStartPos;
        private float cameraScaleSpeed;
        private OptionsLogicMap optionsLogic;
        private List<RaycastResult> raycastResultCache = new List<RaycastResult>();
        private List<TouchData> touchDatas = new List<TouchData>();
        private ITalkSystem talkSystem;
        public NamespaceID MapID { get; private set; }
        [SerializeField]
        private MapUI ui;
        [SerializeField]
        private TalkController talkController;
        [SerializeField]
        private GameObject raycastHitbox;
        [SerializeField]
        private Transform modelRoot;
        [SerializeField]
        private Transform mapCameraShakeRoot;
        [SerializeField]
        private Camera mapCamera;
        [SerializeField]
        private float minCameraSize = 2;

        [Header("Button Colors")]
        [SerializeField]
        private Color buttonColorMinigame = Color.yellow;
        [SerializeField]
        private Color buttonColorLocked = Color.gray;
        [SerializeField]
        private Color buttonColorBoss = Color.red;
        [SerializeField]
        private Color buttonColorEndless = Color.magenta;
        [SerializeField]
        private Color buttonColorUncleared = new Color(0, 1, 0, 1);
        [SerializeField]
        private Color buttonColorCleared = new Color(0, 0.5f, 1, 1);

        private class TouchData
        {
            public int fingerId;
            public Vector2 position;
            public Vector2 delta;
        }
    }
}
