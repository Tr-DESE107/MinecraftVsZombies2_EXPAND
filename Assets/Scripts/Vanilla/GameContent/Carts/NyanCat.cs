using MVZ2.GameContent.Areas;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Carts
{
    [EntityBehaviourDefinition(VanillaCartNames.nyanCat)]
    public class NyanCat : CartBehaviour
    {
        public NyanCat(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            UpdateNyaightmareToLevel(entity);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetAnimationBool("Running", entity.IsCartTriggered());
        }
        public static void SetNyaightmare(Entity entity, bool value)
        {
            entity.SetModelProperty("Nyaightmare", value);
            entity.SetCartTriggerSound(value ? VanillaSoundID.nyaightmareScream : VanillaSoundID.meow);
        }
        public static void UpdateNyaightmareToLevel(Entity entity)
        {
            var isNightmare = Dream.IsNightmare(entity.Level);
            SetNyaightmare(entity, isNightmare);
        }
    }
}