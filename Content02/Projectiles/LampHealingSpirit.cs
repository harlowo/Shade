using Microsoft.Xna.Framework;
using Shade.System;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class LampHealingSpirit : ModProjectile
    {
        public float speed = 20f;
        public int framePause = 3;
        public Vector2 dir;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 36;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = new Color(255, 255, 255, 0);
            return true;
        }
        public override void AI()
        {
            speed += 0.25f;
            Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, Projectile.Center.DirectionTo(Main.player[Projectile.owner].Center) * speed, 0.1f);
            if (Projectile.Center.Distance(Main.player[Projectile.owner].Center) < Func.Diagonal(Projectile.Hitbox))
            {
                Main.player[Projectile.owner].Heal(3);
                Main.player[Projectile.owner].ManaEffect(20);
                Projectile.Kill();
            }

            dir = Projectile.velocity;
            dir.Normalize();
            Projectile.rotation = dir.ToRotation();
            Projectile.spriteDirection = Math.Sign(dir.X);
            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.ToRadians(0f);
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(180f);
            }

            if (framePause > 0)
            {
                framePause--;
            }
            else
            {
                framePause = 3;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            if (Main.screenPosition.Distance(Projectile.Center) > Func.Diagonal(Main.screenWidth, Main.screenHeight) * 2)
            {
                Projectile.Kill();
            }
        }
    }
}
