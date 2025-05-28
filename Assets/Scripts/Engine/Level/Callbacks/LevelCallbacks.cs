﻿using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Callbacks
{
    public struct EmptyCallbackParams
    {
    }
    public struct StringCallbackParams
    {
        public string text;

        public StringCallbackParams(string text)
        {
            this.text = text;
        }
    }
    public struct LevelCallbackParams
    {
        public LevelEngine level;

        public LevelCallbackParams(LevelEngine level)
        {
            this.level = level;
        }
    }
    public struct EntityCallbackParams
    {
        public Entity entity;

        public EntityCallbackParams(Entity entity)
        {
            this.entity = entity;
        }
    }
    public static class LevelCallbacks
    {
        public struct PostEntityContactGroundParams
        {
            public Entity entity;
            public Vector3 velocity;
        }
        public struct PreEntityCollisionParams
        {
            public EntityCollision collision;
        }
        public struct PostEntityCollisionParams
        {
            public EntityCollision collision;
            public int state;
        }
        public struct PostEntityDeathParams
        {
            public Entity entity;
            public DeathInfo deathInfo;
        }
        public struct ArmorParams
        {
            public Entity entity;
            public NamespaceID slot;
            public Armor armor;
            public ArmorDestroyInfo info;
        }
        public struct PostArmorDestroyParams
        {
            public Entity entity;
            public NamespaceID slot;
            public Armor armor;
            public ArmorDestroyInfo info;
        }

        public readonly static CallbackType<EntityCallbackParams> POST_ENTITY_INIT = new();
        public readonly static CallbackType<EntityCallbackParams> POST_ENTITY_UPDATE = new();
        public readonly static CallbackType<EntityCallbackParams> POST_ENTITY_REMOVE = new();

        public readonly static CallbackType<PostEntityContactGroundParams> POST_ENTITY_CONTACT_GROUND = new();
        public readonly static CallbackType<EntityCallbackParams> POST_ENTITY_LEAVE_GROUND = new();

        public readonly static CallbackType<PreEntityCollisionParams> PRE_ENTITY_COLLISION = new();
        public readonly static CallbackType<PostEntityCollisionParams> POST_ENTITY_COLLISION = new();

        public readonly static CallbackType<PostEntityDeathParams> POST_ENTITY_DEATH = new();
        public readonly static CallbackType<EntityCallbackParams> POST_ENTITY_REVIVE = new();

        public readonly static CallbackType<ArmorParams> POST_EQUIP_ARMOR = new();
        public readonly static CallbackType<PostArmorDestroyParams> POST_DESTROY_ARMOR = new();
        public readonly static CallbackType<ArmorParams> POST_REMOVE_ARMOR = new();

        public readonly static CallbackType<EntityCallbackParams> POST_ENEMY_SPAWNED = new();


        public struct PostWaveParams
        {
            public LevelEngine level;
            public int wave;

            public PostWaveParams(LevelEngine level, int wave)
            {
                this.level = level;
                this.wave = wave;
            }
        }
        public struct PostGameOverParams
        {
            public LevelEngine level;
            public int type;
            public Entity killer;
            public string message;
        }

        public readonly static CallbackType<LevelCallbackParams> POST_LEVEL_SETUP = new();
        public readonly static CallbackType<LevelCallbackParams> POST_LEVEL_START = new();
        public readonly static CallbackType<LevelCallbackParams> POST_LEVEL_UPDATE = new();
        public readonly static CallbackType<LevelCallbackParams> POST_LEVEL_CLEAR = new();
        public readonly static CallbackType<LevelCallbackParams> POST_PREPARE_FOR_BATTLE = new();
        public readonly static CallbackType<LevelCallbackParams> POST_HUGE_WAVE_EVENT = new();
        public readonly static CallbackType<LevelCallbackParams> POST_FINAL_WAVE_EVENT = new();

        public readonly static CallbackType<PostWaveParams> POST_WAVE = new();
        public readonly static CallbackType<PostWaveParams> POST_WAVE_FINISHED = new();
        public readonly static CallbackType<PostGameOverParams> POST_GAME_OVER = new();
    }
}
