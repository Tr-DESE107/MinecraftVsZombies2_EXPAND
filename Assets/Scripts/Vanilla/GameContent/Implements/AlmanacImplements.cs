﻿using System.Collections.Generic;
using MVZ2.GameContent.Placements;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Almanacs;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Almanacs;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Implements
{
    public class AlmanacImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LogicCallbacks.GET_ALMANAC_ENTRY_TAGS, GetAlmanacEntryTagsCallback);
        }
        private void GetAlmanacEntryTagsCallback(LogicCallbacks.GetAlmanacEntryTagsParams param, CallbackResult result)
        {
            var category = param.category;
            var entryID = param.entryID;
            var tags = param.tags;

            switch (category)
            {
                case VanillaAlmanacCategories.CONTRAPTIONS:
                    GetContraptionEntryTags(entryID, tags);
                    break;
                case VanillaAlmanacCategories.ENEMIES:
                    GetEnemyEntryTags(entryID, tags);
                    break;
            }
        }
        private void GetEntityAttributeTags(EntityDefinition entityDef, List<AlmanacEntryTagInfo> tags)
        {
            // 发光
            if (entityDef.IsLightSource())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.lightSource));
            }
            // 火焰
            if (entityDef.IsFire())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.fire));
            }
            // 忠诚
            if (entityDef.IsLoyal() && Global.Game.IsUnlocked(VanillaUnlockID.castle1))
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.loyal));
            }
            // 忠诚
            if (!entityDef.CanDeactive())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.controlImmunity));
            }
        }
        private void GetShellAttributeTags(EntityDefinition entityDef, List<AlmanacEntryTagInfo> tags)
        {
            var shell = entityDef.GetShellID();
            tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.shell, shell.ToString()));
        }
        private void GetEnemyShellAttributeTags(EntityDefinition entityDef, List<AlmanacEntryTagInfo> tags)
        {
            var game = Global.Game;
            // 盔甲材质
            var startingArmor = entityDef.GetStartingArmor();
            if (NamespaceID.IsValid(startingArmor))
            {
                var armorDef = game.GetArmorDefinition(startingArmor);
                var shellID = armorDef?.GetShellID();
                if (NamespaceID.IsValid(shellID))
                {
                    tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.shellArmor, shellID.ToString()));
                }
            }
            // 护盾材质
            var startingShield = entityDef.GetStartingShield();
            if (NamespaceID.IsValid(startingShield))
            {
                var armorDef = game.GetArmorDefinition(startingShield);
                var shellID = armorDef?.GetShellID();
                if (NamespaceID.IsValid(shellID))
                {
                    tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.shellShield, shellID.ToString()));
                }
            }
        }
        private void GetMassAttributeTags(EntityDefinition entityDef, List<AlmanacEntryTagInfo> tags)
        {
            var mass = entityDef.GetMass();
            tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.mass, mass.ToString()));
        }
        private void GetContraptionAttributeTags(EntityDefinition entityDef, List<AlmanacEntryTagInfo> tags)
        {
            // 夜用
            if (entityDef.IsNocturnal())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.nocturnal));
            }
            // 防御性
            if (entityDef.IsDefensive())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.defensive));
            }
            // 地面器械
            if (entityDef.IsFloor())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.floorContraption));
            }
            // 高
            if (entityDef.BlocksJump())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.high));
            }
            // 可触发
            if (entityDef.IsTriggerActive())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.canTrigger));
            }
        }
        private void GetEnemyAttributeTags(EntityDefinition entityDef, List<AlmanacEntryTagInfo> tags)
        {
            // 低矮
            if (entityDef.IsLowEnemy())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.low));
            }
            // 飞行
            if (entityDef.IsFlyingEnemy())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.flying));
            }
            // 非亡灵
            if (!entityDef.IsUndead())
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.notUndead));
            }
            // 漂浮
            var waterInteraction = entityDef.GetWaterInteraction();
            if (waterInteraction == WaterInteraction.NONE || waterInteraction == WaterInteraction.FLOAT)
            {
                tags.Add(new AlmanacEntryTagInfo(VanillaAlmanacTagID.drownproof));
            }
        }
        private void GetContraptionEntryTags(NamespaceID id, List<AlmanacEntryTagInfo> tags)
        {
            var game = Global.Game;
            var def = game.GetEntityDefinition(id);
            if (def == null)
                return;

            // 放置类。
            var placement = def.GetPlacementID();
            var placementDef = game.GetPlacementDefinition(placement);
            if (placementDef != null)
            {
                var almanacTag = placementDef.GetAlmanacTag();
                if (NamespaceID.IsValid(almanacTag))
                {
                    tags.Add(new AlmanacEntryTagInfo(almanacTag));
                }
            }

            // 占位类。
            var takenGridLayers = def.GetGridLayersToTake();
            if (takenGridLayers != null)
            {
                foreach (var layer in takenGridLayers)
                {
                    var gridLayerMeta = game.GetGridLayerMeta(layer);
                    if (gridLayerMeta != null && NamespaceID.IsValid(gridLayerMeta.AlmanacTag))
                    {
                        tags.Add(new AlmanacEntryTagInfo(gridLayerMeta.AlmanacTag));
                    }
                }
            }

            // 特性类。
            GetEntityAttributeTags(def, tags);
            GetContraptionAttributeTags(def, tags);

            // 枚举类。
            GetShellAttributeTags(def, tags);
        }
        private void GetEnemyEntryTags(NamespaceID id, List<AlmanacEntryTagInfo> tags)
        {
            var game = Global.Game;
            var def = game.GetEntityDefinition(id);
            if (def == null)
                return;


            // 特性类。
            GetEntityAttributeTags(def, tags);
            GetEnemyAttributeTags(def, tags);

            // 枚举类。
            GetShellAttributeTags(def, tags);
            GetEnemyShellAttributeTags(def, tags);
            GetMassAttributeTags(def, tags);
        }
    }
}
