using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Difficulties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.pickupCollectClear)]
    public class CollectBehaviour_Clear : CollectBehaviour
    {
        public CollectBehaviour_Clear(string nsp, string name) : base(nsp, name)
        {
        }
        public override bool CanCollect(Entity pickup)
        {
            return true;
        }
        public override void PostCollect(Entity pickup)
        {
            base.PostCollect(pickup);
            pickup.Velocity = Vector3.zero;
            var level = pickup.Level;

            var difficultyDef = level.Content.GetDifficultyDefinition(level.Difficulty);
            int money = 0;
            if (level.IsRerun)
            {
                money = 250;
                if (difficultyDef != null)
                {
                    money = difficultyDef.GetRerunClearMoney();
                }
            }
            else
            {
                if (level.DropsTrophy())
                {
                    money = difficultyDef.GetPuzzleMoney();
                }
                else if (difficultyDef != null)
                {
                    money = difficultyDef.GetClearMoney();
                }
            }
            GemEffect.SpawnGemEffects(level, money, pickup.Position, pickup, false);

            level.Clear();
            level.ResetHeldItem();
            level.StopMusic();
            level.PlaySound(pickup.GetCollectSound());
            level.PlaySound(pickup.Level.GetClearSound());
            level.Spawn(VanillaEffectID.starParticles, pickup.Position, pickup);
        }
    }
}