using System;
using System.Collections.Generic;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableEntity
    {
        public long id;
        public int type;
        public int state;
        public long target;
        public long parent;
        public SerializableRNG rng;
        public SerializableRNG dropRng;
        public NamespaceID definitionID;
        public NamespaceID modelID;
        public EntityReferenceChain spawnerReference;
        public SerializableArmor EquipedArmor;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 scale;
        public int collisionMask;
        public Vector3 renderRotation;
        public Vector3 renderScale;
        public bool canUnderGround;
        public Vector3 boundsOffset;
        public int poolCount;
        public int timeout;

        #region 影子
        public bool shadowVisible;
        public float shadowAlpha;
        public Vector3 shadowScale;
        public Vector3 shadowOffset;
        #endregion

        public bool isDead;
        public float health;
        public bool isOnGround;
        public SerializablePropertyDictionary propertyDict;
        public SerializableBuffList buffs;
        public List<long> collisionThisTick;
        public List<long> collisionList;
        public List<long> children;
        public List<int> takenGrids;
    }
}
