using MVZ2.GameContent.Buffs;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.fireworkDispenser)]
    public class FireworkDispenser : EntityBehaviourDefinition
    {
        public FireworkDispenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Evoked", entity.HasBuff(VanillaBuffID.Contraption.fireworkDispenserEvoked));
        }
    }
}