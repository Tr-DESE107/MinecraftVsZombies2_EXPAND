using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine.Serialization
{
    public class SerializableEntity
    {
        public int id;
        public int type;
        public int state;
        public int target;
        public SerializableRNG rng;
        public NamespaceID definitionID;
        public NamespaceID modelID;
        public EntityReference spawnerReference;
        public int parent;
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

        #region Warp Lane
        public bool isWarpingLane;
        public int warpTargetLane;
        public int warpFromLane;
        public float warpLaneSpeed;
        #endregion

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
        public List<SerializableBuff> buffs;
        public List<int> collisionThisTick;
        public List<int> collisionList;
        public List<int> children;
        public List<int> takenGrids;
    }
}
