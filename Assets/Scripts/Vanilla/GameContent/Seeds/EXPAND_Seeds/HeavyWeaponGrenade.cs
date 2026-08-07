#nullable enable

using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Projectiles;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponGrenade)]
    public class HeavyWeaponGrenade : SeedOptionDefinition
    {
        public HeavyWeaponGrenade(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        private void Use(LevelEngine level)
        {
            var cart = HeavyWeaponBlueprintUtils.FindCart(level);
            if (cart == null) return;
            var origin = cart.GetCenter();
            var target = origin + new Vector3(THROW_DISTANCE, 0, 0);   // 右前方  
            var tnt = level.Spawn(VanillaProjectileID.flyingTNT, origin, cart);
            if (tnt == null) return;
            tnt.SetFaction(level.Option.LeftFaction);                  // 友方，只伤敌  
            float maxY = Mathf.Max(origin.y, target.y) + THROW_ARC;
            tnt.Velocity = VanillaProjectileExt.GetLobVelocity(origin, target, maxY, tnt.GetGravity());
        }
        public const float THROW_DISTANCE = 400f;
        public const float THROW_ARC = 200f;
    }
}
