using MVZ2.GameContent.Projectiles;
using MVZ2.GameContent.Shells;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Boss.frankensteinSteel)]
    public class FrankensteinSteelBuff : BuffDefinition
    {
        public FrankensteinSteelBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(EngineEntityProps.SHELL, VanillaShellID.metal));
            AddTrigger(VanillaLevelCallbacks.PRE_PROJECTILE_HIT, PreProjectileHitCallback, filter: VanillaProjectileID.knife);
        }
        private void PreProjectileHitCallback(ProjectileHitInput hitInput, DamageInput damage)
        {
            var other = hitInput.Other;
            if (!other.HasBuff<FrankensteinSteelBuff>())
                return;
            var knife = hitInput.Projectile;
            knife.SetFaction(other.GetFaction());

            DeflectKnife(other, knife);
            knife.PlaySound(VanillaSoundID.shieldHit);

            hitInput.Cancel();
        }
        private void DeflectKnife(Entity self, Entity knife)
        {
            var size = self.GetScaledSize();
            float thisX = self.Position.x;
            float thisZ = self.Position.z;
            float xExtent = size.x / 2;
            float zExtent = size.z / 2;
            float knifeX = knife.Position.x;
            float knifeZ = knife.Position.z;
            float knifeXSpeed = knife.Velocity.x;
            float knifeZSpeed = knife.Velocity.z;

            Vector2 leftUp = new Vector2(thisX - xExtent, thisZ + zExtent);
            Vector2 leftDown = new Vector2(thisX - xExtent, thisZ - zExtent);
            Vector2 rightUp = new Vector2(thisX + xExtent, thisZ + zExtent);
            Vector2 rightDown = new Vector2(thisX + xExtent, thisZ - zExtent);

            Vector2 otherDirection2D = new Vector2(knifeXSpeed, knifeZSpeed).normalized;
            Vector2 longRange = otherDirection2D * 100;
            Vector2 otherLastPos2D = new Vector2(knifeX, knifeZ) - longRange;
            Vector2 otherPos2D = new Vector2(knifeX, knifeZ) + longRange;

            Vector3 normal = Vector3.zero;

            if (knifeZSpeed > 0 && knifeZ <= self.Position.z)
            {
                if (MathTool.DoLinesIntersect(otherLastPos2D, otherPos2D, leftDown, rightDown))
                {
                    normal = Vector3.back;
                }
            }
            else if (knifeZSpeed < 0 && knifeZ >= self.Position.z)
            {
                if (MathTool.DoLinesIntersect(otherLastPos2D, otherPos2D, leftUp, rightUp))
                {
                    normal = Vector3.forward;
                }
            }

            if (knifeXSpeed > 0 && knifeX <= self.Position.x)
            {
                if (MathTool.DoLinesIntersect(otherLastPos2D, otherPos2D, leftUp, leftDown))
                {
                    normal = Vector3.left;
                }
            }
            else if (knifeXSpeed < 0 && knifeX >= self.Position.x)
            {
                if (MathTool.DoLinesIntersect(otherLastPos2D, otherPos2D, rightUp, rightDown))
                {
                    normal = Vector3.right;
                }
            }
            knife.Velocity = Vector3.Reflect(knife.Velocity, normal.normalized);
        }
    }
}
