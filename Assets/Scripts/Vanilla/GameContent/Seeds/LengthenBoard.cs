using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.SeedPacks;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [Definition(VanillaBlueprintNames.lengthenBoard)]
    public class LengthenBoard : SeedOptionDefinition
    {
        public LengthenBoard(string nsp, string name) : base(nsp, name)
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
            var board = level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.breakoutBoard) && !BreakoutBoard.IsUpgraded(e));
            if (board == null)
                return;
            BreakoutBoard.Upgrade(board);
            board.PlaySound(VanillaSoundID.gem);
        }
        private bool IsValid(SeedPack seedPack)
        {
            var level = seedPack.Level;
            var board = level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.breakoutBoard) && !BreakoutBoard.IsUpgraded(e));
            if (board == null)
                return false;
            return true;
        }
    }
}
