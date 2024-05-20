using System.Collections.Generic;
using MVZ2.GameContent;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Buffs;
using PVZEngine;
using UnityEditor;

namespace MVZ2.Vanilla
{
    public class Vanilla : Mod
    {
        public Vanilla() : base("mvz2")
        {
            AddDefinition(areaDefinitions, AreaID.day.name, new Day());

            AddDefinition(stageDefinitions, StageID.prologue.name, new StageDefinition());

            AddDefinition(buffDefinitions, BuffID.randomEnemySpeed.name, new RandomEnemySpeedBuff());
            AddDefinition(buffDefinitions, BuffID.damageColor.name, new DamageColorBuff());

            AddDefinition(shellDefinitions, ShellID.flesh.name, new FleshShell());
            AddDefinition(shellDefinitions, ShellID.stone.name, new StoneShell());

            AddDefinition(entityDefinitions, ContraptionID.dispenser.name, new Dispenser());
            AddDefinition(entityDefinitions, EnemyID.zombie.name, new Zombie());
            AddDefinition(entityDefinitions, ProjectileID.arrow.name, new Arrow());

            Callbacks.PostEntityTakeDamage.Add(PostEntityTakeDamage);
        }

        private void PostEntityTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            if (armorResult != null && !armorResult.Effects.HasEffect(DamageEffects.MUTE))
            {
                var entity = armorResult.Entity;
                var shellId = entity.EquipedArmor.GetShellID();
                var shellDefinition = entity.Game.GetShellDefinition(shellId);
                entity.PlayHitSound(armorResult.Effects, shellDefinition);
            }
            if (bodyResult != null && !bodyResult.Effects.HasEffect(DamageEffects.MUTE))
            {
                var entity = bodyResult.Entity;
                var shellId = entity.GetShellID();
                var shellDefinition = entity.Game.GetShellDefinition(shellId);
                entity.PlayHitSound(bodyResult.Effects, shellDefinition);
            }
        }
    }
}
