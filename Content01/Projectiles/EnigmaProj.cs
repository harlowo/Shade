using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Projectiles
{
    public class EnigmaProj : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.alpha = 255;
            Projectile.timeLeft = 20;
            Projectile.tileCollide = false;

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; ++i)
            {
                Vector2 targetDir = ((((float)Math.PI * 4) / 16) * i).ToRotationVector2();
                targetDir.Normalize();
                targetDir *= 5;
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, targetDir.X, targetDir.Y, ProjectileID.BlackBolt, Projectile.damage, Projectile.knockBack, Main.myPlayer);
            }
        }

    }
}
