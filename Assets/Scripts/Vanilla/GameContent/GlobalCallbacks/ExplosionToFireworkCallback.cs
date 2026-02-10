#nullable enable  
  
using MVZ2.GameContent.Effects;  
using MVZ2.Vanilla.Entities;  
using MVZ2Logic.Modding;  
using PVZEngine;  
using PVZEngine.Callbacks;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using UnityEngine;  
  
namespace MVZ2.GameContent.GlobalCallbacks  
{  
    [ModGlobalCallbacks]  
    public class ExplosionToFireworkCallback : VanillaGlobalCallbacks  
    {  
        public override void Apply(Mod mod)  
        {  
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_INIT, OnExplosionInit, filter: EntityTypes.EFFECT);  
        }  
  
        private void OnExplosionInit(EntityCallbackParams param, CallbackResult result)  
        {  
            var entity = param.entity;  
              
            // 只处理 explosion 实体  
            if (!entity.IsEntityOf(VanillaEffectID.explosion))  
                return;  
              
            // 获取爆炸参数  
            var position = entity.GetCenter();  
            var size = entity.GetScaledSize();  
            var radius = Mathf.Max(size.x, size.z) / 2f;  
              
            // 生成烟花爆炸效果  
            FireworkBlast.SpawnFireworkBlast(entity, position, radius, entity.RNG);  
              
            // 移除原始爆炸实体  
            entity.Remove();  
        }  
    }  
}