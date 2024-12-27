using System;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaProjectileExt
    {
        public static Entity ShootProjectile(this Entity entity)
        {
            return entity.ShootProjectile(entity.GetProjectileID());
        }
        public static Entity ShootProjectile(this Entity entity, NamespaceID projectileID)
        {
            var velocity = entity.GetShotVelocity();
            if (entity.IsFacingLeft())
            {
                velocity.x *= -1;
            }
            return entity.ShootProjectile(projectileID, velocity);
        }
        public static Vector3 GetShootPoint(this Entity entity)
        {
            Vector3 offset = entity.GetShotOffset();
            if (entity.IsFacingLeft())
            {
                offset.x *= -1;
            }
            return entity.Position + offset;
        }
        public static Entity ShootProjectile(this Entity entity, NamespaceID projectileID, Vector3 velocity)
        {
            var shootParams = entity.GetShootParams();
            shootParams.projectileID = projectileID;
            shootParams.velocity = velocity;
            return entity.ShootProjectile(shootParams);
        }
        public static Entity ShootProjectile(this Entity entity, ShootParams parameters)
        {
            var game = entity.Level;
            entity.PlaySound(parameters.soundID);

            var velocity = entity.ModifyProjectileVelocity(parameters.velocity);

            var projectile = game.Spawn(parameters.projectileID, parameters.position, entity);
            projectile.SetDamage(parameters.damage);
            projectile.SetFaction(parameters.faction);
            projectile.Velocity = velocity;
            projectile.UpdatePointTowardsDirection();
            return projectile;
        }
        public static ShootParams GetShootParams(this Entity entity)
        {
            return new ShootParams()
            {
                projectileID = entity.GetProjectileID(),
                position = entity.GetShootPoint(),
                faction = entity.GetFaction(),
                damage = entity.GetDamage(),
                soundID = entity.GetShootSound(),
                velocity = entity.GetShotVelocity()
            };
        }
        public static void UpdatePointTowardsDirection(this Entity entity)
        {
            if (entity.PointsTowardDirection())
            {
                var vel = new Vector2(entity.Velocity.x, entity.Velocity.y + entity.Velocity.z);
                entity.RenderRotation = Vector3.forward * Vector2.SignedAngle(Vector2.right, vel);
            }
        }
        public static Vector3 GetLobVelocity(Vector3 source, Vector3 target, float maxY, float gravity, bool passesMaxY = true)
        {
            //问题：已知y = a[(x - h)]^2 + k经过原点，求h。

            //其中，x为点的x值，y为点的y值，a为偏曲率，h为抛物线对称轴，k为最大高度。
            //因为抛物线经过原点，故得：
            //0 = a(-h) ^ 2 + k
            //0 = ah ^ 2 + k
            //a = -k / h ^ 2

            //将a = -k / h ^ 2 代入y = a[(x - h)]^2 + k，得：
            //y = (-k/h^2) * (x-h)^2 + k
            //y = (-k/h^2) * (x^2 - 2xh + h^2) + k
            //y = -k * (x^2 - 2xh + h^2) / h^2 + k
            //y = (-kx^2 + 2kxh - kh^2) / h^2 + k
            //y = -kx^2/h^2 + 2kx/h - k + k
            //y = -kx^2/h^2 + 2kx/h
            //y = (-kx^2 + 2kxh)/
            //yh^2 = -kx^2 + 2kxh
            //yh^2 - 2kxh + kx^2 = 0

            //通过二次函数求根公式x = (-b±√(b^2 - 4ac))/ (2a)可得：
            //h = (2kx±√(4k^2x^2 - 4kx^2y))/ 2y
            //h = (kx±√(x^2 * (k^2 - k*y)))/ y

            //当y = 0时，h = x / 2
            //其中，如果点(x, y)在靠近Y轴一侧，则
            //h = (kx+√(x^2 * (k^2 - k*y)))/ y
            //如果点在(x, y)在远离Y轴一侧，则
            //h = (kx-√(x^2 * (k^2 - k*y)))/ y

            //当k > 0时，函数向下开口，y≤k
            //当k < 0时，函数向上开口，y≥k
            //当k = 0时，h不存在
            //当x = 0时，函数在x = 0上有无数个值

            var relativePosition = target - source;
            var k = maxY - source.y;
            if (k > 0)
            {
                if (relativePosition.y > k)
                    throw new InvalidOperationException("If maxY is positive, it must be greater than target.y.");
                if (gravity < 0)
                    throw new InvalidOperationException("If maxY is positive, the gravity must be positive.");
            }
            else if (k < 0)
            {
                if (relativePosition.y > k)
                    throw new InvalidOperationException("If maxY is negative, it must be less than target.y.");
                if (gravity < 0)
                    throw new InvalidOperationException("If maxY is negative, the gravity must be negative.");
            }
            else
            {
                throw new InvalidOperationException("MaxY cannot be equal to source.y.");
            }

            var horiVector = new Vector2(relativePosition.x, relativePosition.z);
            var horiDirection = horiVector.normalized;
            var length = horiVector.magnitude;
            float highestLength;
            var sign = passesMaxY ? -1 : 1;
            if (Mathf.Approximately(relativePosition.y, 0))
            {
                highestLength = length * 0.5f;
            }
            else
            {
                var sqrMaxY = Mathf.Pow(k, 2);
                var sqrLength = Mathf.Pow(length, 2);
                var factor = Mathf.Sqrt(sqrLength * (sqrMaxY - k * relativePosition.y));
                highestLength = (k * length + sign * factor) / relativePosition.y;
            }
            // 到达最高点t帧过后的y速度y1 = -gt
            // 到达最高点t帧过后的y位置 = k + y1 * (1 + t) / 2
            // 按照目标的最终y位置求出到达最高点后的时间。
            var a = -gravity / 2;
            var b = 0;
            var c = k - relativePosition.y;
            var fallTime = (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            // 根据后半段时间求出总时间。
            var fallPercent = (length - highestLength) / length;
            var totalTime = fallTime / fallPercent;
            var ascendTime = totalTime - fallTime;

            // 根据总时间获取水平移速。
            var velHori = length / totalTime;

            // 到达最高点的y位置 k = y1 + g * (1 + t) / 2
            // 所以 y1 = k - g * (1 + t) / 2
            // 获取垂直移速y1。
            var velVert = gravity * ascendTime;

            var hori = horiDirection * velHori;
            return new Vector3(hori.x, velVert, hori.y);
        }
    }
    public struct ShootParams
    {
        public NamespaceID projectileID;
        public Vector3 position;
        public Vector3 velocity;
        public NamespaceID soundID;
        public float damage;
        public int faction;
    }
}
