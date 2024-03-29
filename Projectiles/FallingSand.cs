using Microsoft.Xna.Framework;
using Shade.System;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class FallingSand : ModProjectile
    {
        public float accelVal = 0.15f;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Y / 40;

            Projectile.velocity.Y += accelVal;
            if (Projectile.Top.Y > Main.screenPosition.Y+Main.screenHeight)
            {
                Projectile.Kill();
            }
        }
    }
}
