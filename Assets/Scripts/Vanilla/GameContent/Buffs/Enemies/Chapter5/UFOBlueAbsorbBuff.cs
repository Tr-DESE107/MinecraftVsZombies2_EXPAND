using System.Collections.Generic;
using MVZ2.Vanilla.Entities;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.ufoBlueAbsorb)]
    public class UFOBlueAbsorbBuff : BuffDefinition
    {
        public UFOBlueAbsorbBuff(string nsp, string name) : base(nsp, name)
        {
            AddAura(new Aura());
        }
        public class Aura : AuraEffectDefinition
        {
            public Aura() : base(VanillaBuffID.Pickup.absorbedByUFO, 4)
            {
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Level;
                foreach (var pickup in level.FindEntities(e => CanAbsorb(auraEffect, e)))
                {
                    results.Add(pickup);
                }
            }

            public override void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff)
            {
                base.UpdateTargetBuff(effect, target, buff);
                var entity = effect.Source.GetEntity();
                if (entity != null)
                {
                    AbsorbedByUFOBuff.SetUFOID(buff, new EntityID(entity));
                }
            }
            private bool CanAbsorb(AuraEffect effect, Entity entity)
            {
                if (entity.Type != EntityTypes.PICKUP) // 是掉落物
                    return false;
                if (entity.IsCollected()) // 没有被拾取
                    return false;
                if (entity.NoPickupStolen()) // 能被偷取
                    return false;
                if (entity.IsImportantPickup())
                    return false;
                var sourceEntity = effect.Source?.GetEntity();
                if (sourceEntity != null)
                {
                    var entityID = sourceEntity.ID;
                    // 掉落物本身有被吸取BUFF，并且这个BUFF的主人不是自己。
                    var absorbBuffs = entity.GetBuffs<AbsorbedByUFOBuff>();
                    bool absorbedByThis = true;
                    if (absorbBuffs.Length > 0)
                    {
                        absorbedByThis = false;
                        foreach (var buff in absorbBuffs)
                        {
                            EntityID ufoID = AbsorbedByUFOBuff.GetUFOID(buff);
                            if (ufoID.ID == entityID)
                            {
                                absorbedByThis = true;
                                break;
                            }
                        }
                    }
                    if (!absorbedByThis)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
