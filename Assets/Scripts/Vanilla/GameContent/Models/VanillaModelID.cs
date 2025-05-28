﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Models
{
    public static class VanillaModelID
    {
        public const string TYPE_HELD_ITEM = "held";
        public const string TYPE_ARMOR = "armor";
        public const string TYPE_ICON = "icon";
        public const string TYPE_UI = "ui";

        public static readonly NamespaceID zombie = Get("zombie", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID moneyChest = Get("money_chest", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID blueprintPickup = Get("blueprint_pickup", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID mapPickup = Get("map_pickup", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID emerald = Get("emerald", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID ruby = Get("ruby", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID sapphire = Get("sapphire", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID diamond = Get("diamond", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID pistonPalm = Get("piston_palm", EngineModelID.TYPE_ENTITY);
        public static readonly NamespaceID dullahanMain = Get("dullahan_main", EngineModelID.TYPE_ENTITY);

        public static readonly NamespaceID boatItem = Get("boat_item", TYPE_ARMOR);

        public static readonly NamespaceID pickaxeHeldItem = Get("pickaxe", TYPE_HELD_ITEM);
        public static readonly NamespaceID triggerHeldItem = Get("trigger", TYPE_HELD_ITEM);
        public static readonly NamespaceID swordHeldItem = Get("sword", TYPE_HELD_ITEM);
        public static readonly NamespaceID defaultStartShardHeldItem = Get("starshard.default", TYPE_HELD_ITEM);
        public static readonly NamespaceID targetHeldItem = Get("target", TYPE_HELD_ITEM);

        public static readonly NamespaceID shortCircuit = Get("short_circuit", TYPE_ICON);
        public static readonly NamespaceID nocturnal = Get("nocturnal", TYPE_ICON);
        public static readonly NamespaceID staticParticles = Get("static_particles", TYPE_ICON);
        public static readonly NamespaceID dreamKeyShield = Get("dream_key_shield", TYPE_ICON);
        public static readonly NamespaceID terrorParasitized = Get("terror_parasitized", TYPE_ICON);
        public static readonly NamespaceID weaknessParticles = Get("weakness_particles", TYPE_ICON);
        public static readonly NamespaceID witherParticles = Get("wither_particles", TYPE_ICON);

        public static readonly NamespaceID RegenerationParticles = Get("RegenerationParticles", TYPE_ICON);
        public static readonly NamespaceID CorropoisonParticles = Get("CorropoisonParticles", TYPE_ICON);

        public static readonly NamespaceID dreamAlarm = Get("dream_alarm", TYPE_ICON);
        public static readonly NamespaceID parabotInsected = Get("parabot_insected", TYPE_ICON);
        public static readonly NamespaceID knockbackWave = Get("knockback_wave", TYPE_ICON);
        public static readonly NamespaceID divineShield = Get("divine_shield", TYPE_ICON);

        public static readonly NamespaceID mindSwap = Get("mind_swap", TYPE_UI);
        public static NamespaceID GetStarshardHeldItem(NamespaceID areaID)
        {
            return new NamespaceID(areaID.SpaceName, $"starshard.{areaID.Path}").ToModelID(TYPE_HELD_ITEM);
        }
        private static NamespaceID Get(string name, string type)
        {
            return new NamespaceID(VanillaMod.spaceName, name).ToModelID(type);
        }
    }
}
