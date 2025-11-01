using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using System.Linq;
using Tools;
using MVZ2.GameContent.Damages;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.RandomZombie)]
    public class RandomZombie : MeleeEnemy
    {
        public RandomZombie(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var level = entity.Level;
            var lane = entity.GetLane();
            if (level.IsWaterLane(lane) || level.IsAirLane(lane))
            {
                entity.AddBuff<BoatBuff>();
                entity.SetModelProperty("HasBoat", true);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
            entity.SetModelProperty("HasBoat", entity.HasBuff<BoatBuff>());
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // �����鴬������
                var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity);
                effect.Velocity = new Vector3(effect.RNG.NextFloat() * 20 - 10, 5, 0);
                effect.ChangeModel(VanillaModelID.boatItem);
                effect.SetDisplayScale(entity.GetDisplayScale());
            }

            //var grid = entity.GetGrid();
            //if (grid == null)
            //    return;

            

            var game = Global.Game;
            var level = entity.Level;
            var rng = entity.RNG;

            // ��ȡ�����ѽ����ĵ���  
            var unlockedEnemies = game.GetUnlockedEnemies();

            // ���˳���Ч�Ľ�ʬ  
            var validEnemies = unlockedEnemies.Where(id =>
            {
                // ����ṩ�˰�����,ֻѡ��������еĽ�ʬ  
                //if (whitelist != null && !whitelist.Contains(id))
                //    return false;

                // ����Ƿ���ͼ����  
                if (!game.IsEnemyInAlmanac(id))
                    return false;

                // ��鵱ǰ�����Ƿ�������ɸõ���  
                return true;
            });

            if (validEnemies.Count() <= 0)
                return;

            // ���ѡ��һ����ʬ  
            var enemyID = validEnemies.Random(rng);

            // ��ԭλ�����ɽ�ʬ  
            var spawned = entity.SpawnWithParams(enemyID, entity.Position);


        }
    }
}
