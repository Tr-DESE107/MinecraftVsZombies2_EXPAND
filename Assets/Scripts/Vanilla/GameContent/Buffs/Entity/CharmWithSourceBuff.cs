using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.charmWithSource)]
    public class CharmWithSourceBuff : BuffDefinition
    {
        public CharmWithSourceBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, new Color(1, 0, 1, 0.5f)));
            AddModifier(new IntModifier(EngineEntityProps.FACTION, NumberOperator.Set, PROP_FACTION));
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, PROP_SIZE_MULTIPLIER));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_SIZE_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_SIZE_MULTIPLIER, Vector3.one);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;

            var sourceID = buff.GetProperty<EntityID>(PROP_SOURCE);
            var source = sourceID?.GetEntity(buff.Level);
            if (source == null || !source.Exists() || source.IsDead)
            {
                entity.PlaySound(VanillaSoundID.mindClear);
                buff.Remove();
            }
            else
            {
                var faction = source.GetFaction();
                buff.SetProperty(PROP_FACTION, faction);

                var faceRight = faction == buff.Level.Option.LeftFaction;
                var xScale = entity.FaceLeftAtDefault() == faceRight ? -1 : 1;
                buff.SetProperty(PROP_SIZE_MULTIPLIER, new Vector3(xScale, 1, 1));
            }
        }
        public const string PROP_SIZE_MULTIPLIER = "SizeMultiplier";
        public const string PROP_SOURCE = "Source";
        public const string PROP_FACTION = "Faction";
    }
}
