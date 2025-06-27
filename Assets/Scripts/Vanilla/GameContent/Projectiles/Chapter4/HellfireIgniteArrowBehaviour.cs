﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.hellfireIgnitedArrow)]
    public class HellfireIgnitedArrowBehaviour : ProjectileBehaviour, IHellfireIgniteBehaviour
    {
        public HellfireIgnitedArrowBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            ignitedBuffBuffer.Clear();
            projectile.GetBuffs<HellfireIgnitedBuff>(ignitedBuffBuffer);

            // 在水中移除普通火焰。
            if (projectile.IsInWater())
            {
                for (int i = ignitedBuffBuffer.Count - 1; i >= 0; i--)
                {
                    var buff = ignitedBuffBuffer[i];
                    if (!HellfireIgnitedBuff.GetCursed(buff))
                    {
                        projectile.RemoveBuff(buff);
                        ignitedBuffBuffer.RemoveAt(i);
                    }
                }
            }

            // 更新模型。
            var ignited = 0;
            if (ignitedBuffBuffer.Count > 0)
            {
                if (ignitedBuffBuffer.Any(b => HellfireIgnitedBuff.GetCursed(b)))
                {
                    ignited = 2;
                }
                else
                {
                    ignited = 1;
                }
            }
            projectile.SetAnimationInt("Ignited", ignited);
        }
        public void Ignite(Entity entity, Entity hellfire, bool cursed)
        {
            var igniteBuff = entity.GetFirstBuff<HellfireIgnitedBuff>();
            if (igniteBuff == null)
            {
                igniteBuff = entity.AddBuff<HellfireIgnitedBuff>();
            }
            if (!HellfireIgnitedBuff.GetCursed(igniteBuff) && cursed)
            {
                HellfireIgnitedBuff.Curse(igniteBuff);
            }
        }
        private List<Buff> ignitedBuffBuffer = new List<Buff>();
    }
}
