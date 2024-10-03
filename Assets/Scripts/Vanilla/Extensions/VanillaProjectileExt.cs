using System;
using MVZ2.Extensions;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaProjectileExt
    {
        public static Entity ShootProjectile(this Entity entity)
        {
            return entity.ShootProjectile(entity.GetProjectileID());
        }
        public static Entity ShootProjectile(this Entity entity, NamespaceID projectileID)
        {
            return entity.ShootProjectile(projectileID, entity.GetShotVelocity());
        }
        public static Entity ShootProjectile(this Entity entity, NamespaceID projectileID, Vector3 velocity)
        {
            var game = entity.Level;
            entity.PlaySound(entity.GetShootSound());

            Vector3 offset = entity.GetShotOffset();
            if (entity.IsFacingLeft())
            {
                offset.x *= -1;
                velocity.x *= -1;
            }
            velocity = entity.ModifyProjectileVelocity(velocity);

            var projectile = game.Spawn(projectileID, entity.Position + offset, entity);
            projectile.SetDamage(entity.GetDamage());
            projectile.SetFaction(entity.GetFaction());
            projectile.Velocity = velocity;
            return projectile;
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
}
