using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.SeedPacks;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [SeedOptionDefinition(VanillaBlueprintNames.addPearl)]
    public class AddPearl : SeedOptionDefinition
    {
        public AddPearl(string nsp, string name) : base(nsp, name)
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
            var board = level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.breakoutBoard) && (e.Target == null || !e.Target.Exists()));
            if (board == null)
                return;
            BreakoutBoard.SpawnPearl(board);
            board.PlaySound(VanillaSoundID.starshardUse);
        }
        private bool IsValid(SeedPack seedPack)
        {
            var level = seedPack.Level;
            var board = level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.breakoutBoard) && (e.Target == null || !e.Target.Exists()));
            if (board == null)
                return false;
            return true;
        }
    }
}
