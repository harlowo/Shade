using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class ForestSpiritProj : ModProjectile
    {
        public Vector2 dir = new(0, 0);
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (dir == new Vector2(0, 0))
            {
                dir = Projectile.velocity / 2;
            }
            Projectile.velocity += dir / 2.5f;

            // Dust

            int dust = Dust.NewDust(Projectile.Center, 12, 12, DustID.Grass);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity = Projectile.velocity;
            Main.dust[dust].rotation = Projectile.velocity.X / dir.X / 10;

            int dust_2 = Dust.NewDust(Projectile.Center, 12, 12, DustID.Grass);
            Main.dust[dust_2].noGravity = true;
            Main.dust[dust_2].scale = Main.rand.Next(2, 10) / 10f;
            Main.dust[dust_2].velocity = Projectile.velocity.RotatedByRandom(40) / Main.rand.Next(1, 7) / 10f;
        }
    }
}
