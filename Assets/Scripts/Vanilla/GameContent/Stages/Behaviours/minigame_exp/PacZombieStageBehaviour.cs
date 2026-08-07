#nullable enable  

using System;  
using MVZ2.GameContent.Contraptions;  
using MVZ2.GameContent.Effects;  
using MVZ2.Vanilla.Entities;  
using MVZ2Logic.Contents.Enemies;  
using MVZ2Logic.Entities;  
using MVZ2Logic.Level;  
using PVZEngine;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using UnityEngine;  
  
namespace MVZ2.GameContent.Stages  
{  
    public class PacZombieStageBehaviour : StageBehaviour  
    {  
        public PacZombieStageBehaviour(StageDefinition stageDef) : base(stageDef) { }  
  
        public override void Start(LevelEngine level)  
        {  
            base.Start(level);  
            level.SetPickaxeActive(false);  
            level.SetTriggerActive(false);  
            SpawnPacDevourer(level);  
        }  
  
        private void SpawnPacDevourer(LevelEngine level)  
        {  
            var pos = new Vector3(level.GetEntityColumnX(2), 0, level.GetLaneZ(2));

            var cart = level.Spawn(VanillaEffectID.minecartRideable, pos, null);
            if (cart == null) return;
            MinecartRideable.SetInvisible(cart);   // 用 tint alpha=0 实现真正隐形

            // 吃怪模式吞噬者，骑上矿车  
            var devParams = new SpawnParams();  
            devParams.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());  
            devParams.SetProperty(Devourer.PROP_ENDLESS_GHOST, true);  
            var devourer = level.Spawn(VanillaContraptionID.devourer, cart.Position, cart, devParams);  
            devourer?.RideOn(cart);  
        }  
    }  
}
