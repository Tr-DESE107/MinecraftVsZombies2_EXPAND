using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.Entity.charm)]
    public class CharmBuff : BuffDefinition
    {
        public CharmBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, new Color(1, 0, 1, 0.5f)));
            AddModifier(new IntModifier(EngineEntityProps.FACTION, NumberOperator.Set, PROP_FACTION));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;

            var mode = buff.GetProperty<int>(PROP_MODE);
            int targetFaction = buff.GetProperty<int>(PROP_FACTION);
            if (mode == CharmModes.SOURCE)
            {
                var sourceID = buff.GetProperty<EntityID>(PROP_SOURCE);
                var source = sourceID?.GetEntity(buff.Level);
                if (source == null || !source.Exists() || source.IsDead)
                {
                    entity.PlaySound(VanillaSoundID.mindClear);
                    buff.Remove();
                    return;
                }
                else
                {
                    targetFaction = source.GetFaction();
                    buff.SetProperty(PROP_FACTION, targetFaction);
                }
            }
        }

        public static void SetPermanent(Buff buff, int faction)
        {
            buff.SetProperty(PROP_MODE, CharmModes.PERMANENT);
            buff.SetProperty(PROP_FACTION, faction);
        }
        public static void SetController(Buff buff, Entity source)
        {
            buff.SetProperty(PROP_MODE, CharmModes.SOURCE);
            buff.SetProperty(PROP_SOURCE, new EntityID(source));
        }
        public static void CloneCharm(Buff buff, Entity target)
        {
            var targetBuff = target.GetFirstBuff<CharmBuff>();
            if (targetBuff == null)
            {
                targetBuff = target.AddBuff<CharmBuff>();
            }
            targetBuff.SetProperty(PROP_MODE, buff.GetProperty<int>(PROP_MODE));
            targetBuff.SetProperty(PROP_FACTION, buff.GetProperty<int>(PROP_FACTION));
            var oldSource = buff.GetProperty<EntityID>(PROP_SOURCE);
            var newSource = oldSource != null ? new EntityID(oldSource.ID) : null;
            targetBuff.SetProperty(PROP_SOURCE, newSource);
        }

        public static readonly VanillaBuffPropertyMeta<int> PROP_MODE = new VanillaBuffPropertyMeta<int>("Mode");
        public static readonly VanillaBuffPropertyMeta<int> PROP_FACTION = new VanillaBuffPropertyMeta<int>("Faction");
        public static readonly VanillaBuffPropertyMeta<EntityID> PROP_SOURCE = new VanillaBuffPropertyMeta<EntityID>("Source");
    }

    public static class CharmModes
    {
        public const int PERMANENT = 0;
        public const int SOURCE = 1;
        public const int TIMEOUT = 2;
    }
}
