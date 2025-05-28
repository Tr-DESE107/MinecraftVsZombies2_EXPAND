using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace MVZ2.Level
{
    public abstract class RuntimeBlueprintController : BlueprintController
    {
        #region 公有方法
        public RuntimeBlueprintController(ILevelController controller, Blueprint ui, int index, SeedPack seedPack) : base(controller, ui, index)
        {
            SeedPack = seedPack;
            ui.gameObject.name = seedPack.GetDefinitionID().ToString();
            seedPack.SetModelInterface(modelInterface);
        }
        public override void Remove()
        {
            base.Remove();
            SeedPack.SetModelInterface(null);
        }
        protected override void AddCallbacks()
        {
            base.AddCallbacks();
            ui.OnPointerRelease += OnPointerReleaseCallback;
            SeedPack.OnDefinitionChanged += OnDefinitionChangedCallback;
        }
        protected override void RemoveCallbacks()
        {
            base.RemoveCallbacks();
            ui.OnPointerRelease -= OnPointerReleaseCallback;
            SeedPack.OnDefinitionChanged -= OnDefinitionChangedCallback;
        }
        public virtual void UpdateFixed()
        {
            var model = GetModel();
            if (model)
            {
                model.UpdateFixed();
            }
        }
        public virtual void UpdateFrame(float deltaTime)
        {
            UpdateView();
            var model = GetModel();
            if (model)
            {
                model.UpdateFrame(deltaTime);
            }
        }
        public override BlueprintViewData GetBlueprintViewData()
        {
            return Main.ResourceManager.GetBlueprintViewData(SeedPack);
        }
        public bool CanPick()
        {
            return CanPick(out _);
        }
        public override void Click()
        {
            bool holdingStarshard = Level.IsHoldingStarshard();
            bool canInstantEvoke = SeedPack?.CanInstantEvoke() ?? false;
            bool instantEvoke = canInstantEvoke && holdingStarshard;

            // 进行立即触发检测。
            bool holdingTrigger = Level.IsHoldingTrigger();
            bool canInstantTrigger = SeedPack?.CanInstantTrigger() ?? false;
            bool usingTrigger = holdingTrigger && canInstantTrigger;

            bool swapped = Controller.IsTriggerSwapped();
            bool instantTrigger = canInstantTrigger && (holdingTrigger != swapped);

            bool skipCancelHeld = usingTrigger || instantEvoke;
            BlueprintPickupInfo info = new BlueprintPickupInfo()
            {
                instantTrigger = instantTrigger,
                instantEvoke = instantEvoke,
            };
            Pickup(info, skipCancelHeld);
        }
        public bool CanPick(out string errorMessage)
        {
            return SeedPack.CanPick(out errorMessage);
        }
        public override TooltipViewData GetTooltipViewData()
        {
            var viewData = base.GetTooltipViewData();
            viewData.error = GetTooltipErrorMessage();
            return viewData;
        }
        public override SeedDefinition GetSeedDefinition()
        {
            return SeedPack.Definition;
        }
        #endregion

        #region 私有方法

        #region 事件回调
        private void OnDefinitionChangedCallback(SeedDefinition seedDef)
        {
            ui.UpdateView(Main.ResourceManager.GetBlueprintViewData(SeedPack));
        }
        private void OnPointerReleaseCallback(Blueprint blueprint)
        {
            // 移动端会额外在手指放开在蓝图上时进行一次立即触发检测。
            if (!Global.IsMobile())
                return;

            bool holdingStarshard = Level.IsHoldingStarshard();
            bool canInstantEvoke = SeedPack?.CanInstantEvoke() ?? false;
            bool instantEvoke = canInstantEvoke && holdingStarshard;

            bool holdingTrigger = Level.IsHoldingTrigger();
            bool canInstantTrigger = SeedPack?.CanInstantTrigger() ?? false;
            bool usingTrigger = holdingTrigger && canInstantTrigger;
            if (instantEvoke || usingTrigger)
            {
                bool swapped = Controller.IsTriggerSwapped();
                bool instantTrigger = canInstantTrigger && (holdingTrigger != swapped);
                BlueprintPickupInfo info = new BlueprintPickupInfo()
                {
                    instantTrigger = instantTrigger,
                    instantEvoke = instantEvoke,
                };
                Pickup(info, true);
            }
        }
        #endregion

        protected abstract void OnPickup(BlueprintPickupInfo info);
        protected virtual bool ShouldBlueprintTwinkle(SeedPack seedPack)
        {
            if (SeedPack.IsTwinkling())
            {
                return true;
            }
            else if (Level.IsHoldingTrigger() && SeedPack.CanInstantTrigger())
            {
                return true;
            }
            else if (Level.IsHoldingStarshard() && SeedPack.CanInstantEvoke())
            {
                return true;
            }
            return false;
        }
        private void Pickup(BlueprintPickupInfo info, bool skipCancelHeld = false)
        {
            // 先取消已经手持的物品。
            if (!skipCancelHeld)
            {
                if (Level.IsHoldingItem())
                {
                    if (Level.CancelHeldItem())
                    {
                        Level.PlaySound(VanillaSoundID.tap);
                        return;
                    }
                }
            }
            // 无法拾取蓝图。
            if (!CanPick())
            {
                Level.PlaySound(VanillaSoundID.buzzer);
                return;
            }
            OnPickup(info);
        }
        private string GetTooltipErrorMessage()
        {
            if (!CanPick(out var errorMessage) && !string.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }
            return string.Empty;
        }
        #endregion

        #region 序列化
        public SerializableBlueprintController ToSerializable()
        {
            var serializable = CreateSerializable();
            var model = GetModel();
            if (model != null)
            {
                serializable.model = model.ToSerializable();
            }
            return serializable;
        }
        public void LoadFromSerializable(SerializableBlueprintController serializable)
        {
            var model = GetModel();
            if (model != null && serializable.model != null)
            {
                model.LoadFromSerializable(serializable.model);
            }
            LoadSerializable(serializable);
        }
        protected abstract SerializableBlueprintController CreateSerializable();
        protected virtual void LoadSerializable(SerializableBlueprintController serializable) { }

        #endregion

        #region 属性字段
        public SeedPack SeedPack { get; private set; }
        #endregion
    }
    public struct BlueprintPickupInfo
    {
        public bool instantTrigger;
        public bool instantEvoke;
    }
}
