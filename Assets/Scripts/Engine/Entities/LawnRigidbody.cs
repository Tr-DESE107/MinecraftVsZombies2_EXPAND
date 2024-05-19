using System;
using UnityEngine;

namespace PVZEngine
{
    public class LawnRigidbody
    {
        #region 公有方法

        #region 构造方法
        public LawnRigidbody(Game level)
        {
            Level = level;
        }
        #endregion

        #region 体积
        public Vector3 GetBoundsCenter()
        {
            return Pos + GetScaledBoundsOffset() + 0.5f * GetScaledSize().y * Vector3.up;
        }
        public Bounds GetBounds()
        {
            return new Bounds(GetBoundsCenter(), Size);
        }
        public Vector3 GetScaledSize()
        {
            Vector3 size = Size;
            size.Scale(Scale);
            return size;
        }
        public Vector3 GetScaledBoundsOffset()
        {
            Vector3 offset = BoundsOffset;
            offset.Scale(Scale);
            return offset;
        }
        #endregion

        #region 相对高度
        public float GetGroundHeight()
        {
            return Level.GetGroundHeight(Pos.x, Pos.z);
        }
        public float GetRelativeY()
        {
            return Pos.y - GetGroundHeight();
        }
        public void SetRelativeY(float value)
        {
            var pos = Pos;
            pos.y = value + GetGroundHeight();
            Pos = pos;
        }
        #endregion

        public void Update()
        {
            Vector3 nextVelocity = GetNextVelocity();
            Vector3 nextPos = GetNextPosition();
            Pos = nextPos;
            Velocity = nextVelocity;

            float groundHeight = GetGroundHeight();
            float relativeY = nextPos.y - groundHeight;
            bool leavingGround = relativeY > 0 || (relativeY == 0 && nextVelocity.y >= 0);
            if (leavingGround)
            {
                if (isOnGround)
                {
                    OnLeaveGround?.Invoke();
                    isOnGround = false;
                }
            }
            else
            {
                if (!isOnGround)
                {
                    OnContactGround?.Invoke();
                    isOnGround = true;
                }
            }
            if (!CanUnderGround)
            {
                var pos = Pos;
                pos.y = Mathf.Max(GetGroundHeight(), pos.y);
                Pos = pos;

                var vel = Velocity;
                vel.y = Mathf.Max(0, vel.y);
                Velocity = vel;
            }
        }
        #endregion

        public Vector3 GetNextPosition()
        {
            float deltaTime = SimulationSpeed;
            Vector3 velocity = GetNextVelocity();
            return Pos + velocity * deltaTime;
        }


        private Vector3 GetNextVelocity()
        {
            float deltaTime = SimulationSpeed;

            Vector3 velocity = Velocity;

            // Friction.
            float magnitude = velocity.magnitude;
            Vector3 normalized = velocity.normalized;
            velocity = normalized * Math.Max(0, magnitude * (1 - deltaTime * Friction));

            // Gravity.
            velocity.y -= Gravity * deltaTime;

            return velocity;
        }

        #region 事件
        public event Action OnContactGround;
        public event Action OnLeaveGround;
        #endregion

        #region 属性字段
        public Game Level { get; private set; }
        public Vector3 Pos { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Size { get; set; } = Vector3.one;
        public Vector3 Scale { get; set; } = Vector3.one;
        public bool CanUnderGround { get; set; }
        public float Gravity { get; set; }
        public float Friction { get; set; }
        public Vector3 BoundsOffset { get; set; }
        public float SimulationSpeed { get; set; } = 1;
        private bool isOnGround = true;
        #endregion
    }
}