using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Options;
using MVZ2.Scenes;
using MVZ2.Talk;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Saves;
using MVZ2Logic.Level;
using MVZ2Logic.Map;
using MVZ2Logic.Scenes;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Map
{
    public class MapController : MainScenePage
    {
        #region 公有方法
        public override void Display()
        {
            base.Display();
            ResetCamera();
            ui.SetButtonActive(MapUI.ButtonType.Almanac, Main.SaveManager.IsAlmanacUnlocked());
            ui.SetButtonActive(MapUI.ButtonType.Store, Main.SaveManager.IsStoreUnlocked());
            ui.SetHintText(Main.LanguageManager._(Main.IsMobile() ? HINT_TEXT_MOBILE : HINT_TEXT));
            ui.SetDragRootVisible(false);
            ui.SetOptionsDialogActive(false);
            ui.SetRaycastBlockerActive(false);
            if (!Main.SoundManager.IsPlaying(VanillaSoundID.travel))
            {
                Main.SoundManager.Play2D(VanillaSoundID.travel);
            }
            Main.Scene.ShowPortal();
        }
        public override void Hide()
        {
            base.Hide();
            if (model)
            {
                model.OnMapButtonClick -= OnMapButtonClickCallback;
                model.OnEndlessButtonClick -= OnEndlessButtonClickCallback;
                Destroy(model.gameObject);
                model = null;
            }
            SetCameraBackgroundColor(Color.black);
        }
        public void SetMap(NamespaceID mapId)
        {
            MapID = mapId;
            mapMeta = Main.ResourceManager.GetMapMeta(mapId);
            if (mapMeta == null)
                return;

            var savedPreset = Main.SaveManager.GetMapPresetID(mapId);
            MapPreset mapPreset = null;
            if (NamespaceID.IsValid(savedPreset))
            {
                mapPreset = mapMeta.presets.FirstOrDefault(p => p.id == savedPreset);
            }
            if (mapPreset == null)
            {
                mapPreset = mapMeta.presets.FirstOrDefault();
            }
            var modelPrefab = Main.ResourceManager.GetMapModel(mapPreset.model);
            model = Instantiate(modelPrefab.gameObject, modelRoot).GetComponent<MapModel>();
            model.OnMapButtonClick += OnMapButtonClickCallback;
            model.OnEndlessButtonClick += OnEndlessButtonClickCallback;

            UpdateModelButtons();
            SetCameraBackgroundColor(mapPreset.backgroundColor);
            Main.MusicManager.Play(mapPreset.music);

            var mapTalk = Main.SaveManager.GetMapTalk();
            if (talkController.CanStartTalk(mapTalk))
            {
                talkController.StartTalk(mapTalk, 0, 3);
            }
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            ui.OnButtonClick += OnButtonClickCallback;
            talkController.OnTalkAction += OnTalkActionCallback;
            talkController.OnTalkEnd += OnTalkEndCallback;
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
            mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize + cameraScaleSpeed, minCameraSize, maxCameraY);
            LimitCameraPosition();
            cameraScaleSpeed *= 0.8f;
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
                    break;
                case MapUI.ButtonType.Store:
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
            VanillaCallbacks.TalkAction.RunFiltered(cmd, talkController, cmd, parameters);
        }
        private void OnTalkEndCallback()
        {
            Main.SaveManager.SetMapTalk(null);
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
            StartCoroutine(EnterLevel(index));
        }
        private void OnEndlessButtonClickCallback()
        {
        }
        #endregion

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
            var minX = -maxCameraY * maxCameraAspect;
            var maxX = maxCameraY * maxCameraAspect;
            var minY = -maxCameraY;
            var maxY = maxCameraY;
            position.x = Mathf.Clamp(position.x, minX + cameraWidth * 0.5f, maxX - cameraWidth * 0.5f);
            position.y = Mathf.Clamp(position.y, minY + cameraHeight * 0.5f, maxY - cameraHeight * 0.5f);
            mapCamera.transform.position = position;
        }
        #endregion

        #region 关卡
        private StageMeta GetStageMeta(int index)
        {
            var stageID = mapMeta.stages[index];
            if (stageID == null)
                return null;
            return Main.ResourceManager.GetStageMeta(stageID);
        }
        private bool IsMinigameStage(int index)
        {
            var stageMeta = GetStageMeta(index);
            if (stageMeta == null)
                return false;
            return stageMeta.Type == StageTypes.TYPE_MINIGAME;
        }
        private bool IsLevelUnlocked(int index)
        {
            var stageMeta = GetStageMeta(index);
            if (stageMeta == null)
                return false;
            return Main.SaveManager.IsUnlocked(stageMeta.Unlock);
        }
        private bool IsEndlessUnlocked()
        {
            return Main.SaveManager.IsUnlocked(mapMeta.endlessUnlock);
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
                var scale = currentLength / lastLength;

                var lastCenter = (lastPosition0 + lastPosition1) * 0.5f;
                var currentCenter = (position0 + position1) * 0.5f;
                var motion = lastCenter - currentCenter;

                mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize * scale, minCameraSize, maxCameraY);
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

        private IEnumerator EnterLevel(int index)
        {
            ui.SetHintText(Main.LanguageManager._(HINT_TEXT_ENTERING_LEVEL));
            ui.SetRaycastBlockerActive(true);
            Main.MusicManager.Stop();
            Main.SoundManager.Play2D(VanillaSoundID.spring);
            yield return new WaitForSeconds(1);
            var task = GotoLevelAsync(index);
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
        private async Task GotoLevelAsync(int index)
        {
            Main.SaveManager.SaveModDatas();
            await Main.LevelManager.GotoLevelSceneAsync();
            Main.LevelManager.InitLevel(mapMeta.area, mapMeta.stages[index]);
            Hide();
        }

        private void UpdateModelButtons()
        {
            int unclearedMapButtonIndex = -1;
            for (int i = 0; i < model.GetMapButtonCount(); i++)
            {
                var unlocked = IsLevelUnlocked(i);
                var color = buttonColorCleared;
                if (!unlocked)
                    color = buttonColorLocked;
                else if (IsMinigameStage(i))
                    color = buttonColorMinigame;
                else if (!IsLevelCleared(i))
                {
                    color = buttonColorUncleared;
                    unclearedMapButtonIndex = i;
                }

                model.SetMapButtonInteractable(i, unlocked);
                model.SetMapButtonColor(i, color);
                model.SetMapButtonText(i, (i + 1).ToString());
            }
            var endlessColor = buttonColorCleared;
            var endlessUnlocked = IsEndlessUnlocked();
            if (!endlessUnlocked)
                endlessColor = buttonColorLocked;
            model.SetEndlessButtonInteractable(endlessUnlocked);
            model.SetEndlessButtonColor(endlessColor);
            model.SetEndlessButtonText("E");


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

        #endregion

        [TranslateMsg("地图的提示文本")]
        public const string HINT_TEXT = "按住右键拖动以移动视图\n滚轮以缩放视图";
        [TranslateMsg("地图的提示文本")]
        public const string HINT_TEXT_MOBILE = "单指拖动以移动视图\n双指触摸以缩放视图";
        [TranslateMsg("地图的提示文本")]
        public const string HINT_TEXT_ENTERING_LEVEL = "正在进入关卡……";
        private MainManager Main => MainManager.Instance;
        private MapModel model;
        private MapMeta mapMeta;
        private bool draggingView;
        private Vector2 mapDragStartPos;
        private float cameraScaleSpeed;
        private OptionsLogicMap optionsLogic;
        private List<RaycastResult> raycastResultCache = new List<RaycastResult>();
        private List<TouchData> touchDatas = new List<TouchData>();
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
        private Camera mapCamera;
        [SerializeField]
        private float maxCameraY = 12;
        [SerializeField]
        private float maxCameraAspect = 1.7f;
        [SerializeField]
        private float minCameraSize = 2;

        [Header("Button Colors")]
        [SerializeField]
        private Color buttonColorMinigame = Color.yellow;
        [SerializeField]
        private Color buttonColorLocked = Color.gray;
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
