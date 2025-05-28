using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareDarkness)]
    public class NightmareDarkness : EffectBehaviour
    {
        public NightmareDarkness(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Position = Vector3.zero;
            GenerateEyes(entity);
        }
        private void GenerateEyes(Entity entity)
        {
            var infos = new List<NightmareEyeInfo>();
            var rng = entity.RNG;
            for (int i = 0; i < MAX_EYE_COUNT; i++)
            {
                Vector3 pos = Vector3.zero;
                var scale = Mathf.Lerp(1, 0.4f, i / 15f);
                int times = 0;
                const int limit = 256;
                do
                {
                    if (times >= limit)
                    {
                        Debug.LogWarning("Limit reached while generating eyes on nightmare darkness.");
                        break;
                    }
                    var x = rng.Next(0, 1020f);
                    var z = rng.Next(0, 600f);
                    pos = new Vector3(x, 0, z);
                    times++;
                }
                while (infos.Any(i => Vector3.Distance(pos, i.position) < (scale + i.scale) * 50));

                NightmareEyeInfo info = new NightmareEyeInfo()
                {
                    position = pos,
                    angle = rng.Next(0, 360f),
                    scale = scale,
                    time = GetInfoTimeByIndex(i)
                };

                infos.Add(info);
            }
            entity.SetModelProperty("Eyes", infos);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Timeout", entity.Timeout);
            if (entity.Timeout == 30)
            {
                var level = entity.Level;
                level.SetModelAnimatorBool("Boss", true);
                level.ShakeScreen(0, 50, 30);
            }
        }
        private int GetInfoTimeByIndex(int index)
        {
            float time = 0;
            if (index == 0)
            {
                time = 0;
            }
            else if (index <= 2)
            {
                time = Mathf.Lerp(30, 60, index - 1);
            }
            else if (index <= 4)
            {
                time = Mathf.Lerp(60, 75, index - 3);
            }
            else if (index <= 25)
            {
                time = Mathf.Lerp(75, 105, (index - 5) / 20f);
            }
            else if (index <= MAX_EYE_COUNT)
            {
                time = Mathf.Lerp(105, 135, (index - 26) / (float)(MAX_EYE_COUNT - 26));
            }
            return Mathf.CeilToInt(time);
        }
        private const float MAX_RADIUS = 1200;
        private const int MAX_EYE_COUNT = 100;

        public static readonly NamespaceID ID = VanillaEffectID.nightmareDarkness;
    }
    [Serializable]
    public class NightmareEyeInfo
    {
        public Vector3 position;
        public float angle;
        public float scale;
        public int time;
    }
}