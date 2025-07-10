using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Fragments;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Entities;
using PVZEngine.Armors;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Armors
{
    [ArmorBehaviourDefinition(VanillaArmorBehaviourNames.cannon)]
    public class Cannon : ArmorBehaviourDefinition
    {
        public Cannon(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_DESTROY_ARMOR, PostArmorDestroyCallback);
        }
        public override void PostUpdate(Armor armor)
        {
            base.PostUpdate(armor);
            var timer = GetTimer(armor);
            if (timer == null)
            {
                timer = new FrameTimer(SHOOT_INTERVAL);
                SetTimer(armor, timer);
            }
            timer.Run();
            if (timer.Expired)
            {
                timer.Reset();

                var pos = armor.Owner.Position + armor.Owner.GetArmorOffset(armor.Slot, armor.Definition.GetID()) + armor.Owner.GetFacingDirection() * 20;
                var ball = armor.Owner.SpawnWithParams(VanillaEnemyID.cannonballZombie, pos);

                ball.PlaySound(VanillaSoundID.smallExplosion);
            }
        }
        private void PostArmorDestroyCallback(LevelCallbacks.PostArmorDestroyParams param, CallbackResult result)
        {
            var entity = param.entity;
            var armor = param.armor;
            var info = param.info;
            if (!armor.Definition.HasBehaviour(this))
                return;
            var pos = entity.Position + new Vector3(entity.GetFacingX() * 20, 40, 0);
            entity.CreateFragmentAndPlay(pos, VanillaFragmentID.cannon, emitSpeed: 500);
        }
        public static FrameTimer GetTimer(Armor armor) => armor.GetProperty<FrameTimer>(PROP_TIMER);
        public static void SetTimer(Armor armor, FrameTimer value) => armor.SetProperty(PROP_TIMER, value);
        public const int SHOOT_INTERVAL = 240;
        public static readonly VanillaArmorPropertyMeta<FrameTimer> PROP_TIMER = new VanillaArmorPropertyMeta<FrameTimer>("timer");
    }
}
