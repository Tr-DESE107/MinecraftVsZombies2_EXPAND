using MVZ2.GameContent.Buffs.Effects;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.fireworkBlast)]
    public class FireworkBlast : EffectBehaviour
    {
        public FireworkBlast(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var buff = entity.AddBuff<LightFadeoutBuff>();
            buff.SetProperty(LightFadeoutBuff.PROP_SPEED_MULTIPLIER, 3f);
        }
        public static Entity SpawnFireworkBlast(Entity spawner, Vector3 position, float radius, Color color)
        {
            var param = spawner.GetSpawnParams();
            var size = Vector3.one * (radius * 2);
            param.SetProperty(EngineEntityProps.SIZE, size);
            param.SetProperty(VanillaEntityProps.LIGHT_RANGE, size);
            param.SetProperty(EngineEntityProps.TINT, color);
            param.SetProperty(VanillaEntityProps.LIGHT_COLOR, color);
            return spawner.Spawn(VanillaEffectID.fireworkBlast, position, param);
        }
        public static Entity SpawnFireworkBlast(Entity spawner, Vector3 position, float radius, RandomGenerator rng)
        {
            var h = rng.NextFloat();
            var s = 1;
            var v = 1;
            var color = Color.HSVToRGB(h, s, v);
            return SpawnFireworkBlast(spawner, position, radius, color);
        }
    }
}