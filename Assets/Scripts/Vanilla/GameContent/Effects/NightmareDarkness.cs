using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;
using static MVZ2.GameContent.Buffs.VanillaBuffNames;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.nightmareDarkness)]
    public class NightmareDarkness : EffectBehaviour
    {
        public NightmareDarkness(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Position = Vector3.zero;
            entity.SetSortingLayer(SortingLayers.shadow);
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
                entity.Level.ShakeScreen(0, 50, 30);
            }
            if (entity.Timeout == 0)
            {
                var level = entity.Level;
                Vector3 pos = new Vector3(level.GetEntityColumnX(4), 0, level.GetEntityLaneZ(2));
                var boss = entity.Spawn(VanillaBossID.slenderman, pos);
                boss.Velocity = Vector3.up * 5;
                boss.PlaySound(VanillaSoundID.splashBig);
                boss.PlaySound(VanillaSoundID.glassBreakBig);
                level.ShakeScreen(30, 0, 30);
                boss.Spawn(VanillaEffectID.nightmareaperSplash, pos);

                level.SetProgressBarToBoss(VanillaProgressBarID.nightmare);
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