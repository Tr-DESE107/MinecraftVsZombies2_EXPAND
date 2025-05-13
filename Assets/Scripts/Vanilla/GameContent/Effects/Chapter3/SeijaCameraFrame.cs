using System.Collections.Generic;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Models;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.seijaCameraFrame)]
    public class SeijaCameraFrame : EffectBehaviour
    {
        #region 公有方法
        public SeijaCameraFrame(string nsp, string name) : base(nsp, name)
        {
            flashDetector = new CameraFlashDetector();
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.Timeout == 30)
            {
                detectBuffer.Clear();
                flashDetector.DetectEntities(entity, detectBuffer);
                bool soundPlayed = false;
                foreach (var target in detectBuffer)
                {
                    if (target.Type == EntityTypes.PLANT)
                    {
                        if (target.CanDeactive())
                        {
                            target.ShortCircuit(300);
                            if (!soundPlayed)
                            {
                                target.PlaySound(VanillaSoundID.powerOff);
                                soundPlayed = true;
                            }
                        }
                    }
                    else if (target.Type == EntityTypes.PROJECTILE)
                    {
                        target.Remove();
                    }
                }
                entity.TriggerAnimation("Flash");
                entity.PlaySound(VanillaSoundID.shutter);
            }
            else if (entity.Timeout <= 15)
            {
                var color = new Color(1, 1, 1, (entity.Timeout / 15f));
                entity.SetTint(color);
            }
        }
        #endregion

        private List<Entity> detectBuffer = new List<Entity>();
        private Detector flashDetector;
    }
}