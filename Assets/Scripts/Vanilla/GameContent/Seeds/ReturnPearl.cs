using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.SeedPacks;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [Definition(VanillaBlueprintNames.returnPearl)]
    public class ReturnPearl : SeedOptionDefinition
    {
        public ReturnPearl(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(SeedPack seedPack, float rechargeSpeed)
        {
            base.Update(seedPack, rechargeSpeed);
            seedPack.SetProperty(EngineSeedProps.DISABLED, !IsValid(seedPack));
        }
        public override void Use(SeedPack seedPack)
        {
            base.Use(seedPack);
            var level = seedPack.Level;
            var pearl = level.FindFirstEntity(VanillaProjectileID.breakoutPearl);
            if (pearl == null)
                return;
            var board = level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.breakoutBoard) && (e.Target == null || !e.Target.Exists()));
            if (board == null)
                return;
            BreakoutBoard.ReturnPearl(board, pearl);
            board.PlaySound(VanillaSoundID.starshardUse);
        }
        private bool IsValid(SeedPack seedPack)
        {
            var level = seedPack.Level;
            var pearl = level.FindFirstEntity(VanillaProjectileID.breakoutPearl);
            if (pearl == null)
                return false;
            var board = level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.breakoutBoard) && (e.Target == null || !e.Target.Exists()));
            if (board == null)
                return false;
            return true;
        }
    }
}
