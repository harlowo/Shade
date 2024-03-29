using Microsoft.Xna.Framework;
using Shade.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class CurseFire : ModProjectile
    {
        int npcTargetID = -1;
        NPC npcTarget = null;
        float targetDistance;
        Vector2 targetDirection;
        float inertia = 1f;
        float timer = 10f;
        float maxDetectRadius = 400f;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Resize(16, 16);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(39, 30);
        }
        public override void AI()
        {
            // AI
            if (timer >= 0)
            {
                timer--;
            }
            else
            {
                timer = 0f;
                npcTarget = Func.FindClosestNPC(maxDetectRadius, Projectile.Center);
                npcTargetID = npcTarget.whoAmI;
            }

            if (npcTargetID != -1 && npcTarget.active)
            {
                targetDistance = Vector2.Distance(Main.npc[npcTargetID].Center, Projectile.Center);
                targetDirection = Main.npc[npcTargetID].Center - Projectile.Center;
                targetDirection.Normalize();
                Projectile.velocity = (Projectile.velocity * inertia + targetDirection * (targetDistance / 20)) / (inertia + (float)0.32);
            }
            else if (npcTarget != null && !npcTarget.active)
            {
                Projectile.Kill();
            }
        }
    }
}
