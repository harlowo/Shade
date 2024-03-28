using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Projectiles.OnyxExcavator
{
	public class OnyxExcavatorLightningBolt : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1.0f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
            Projectile.frame = 1;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4 || ++Projectile.frame < 1) { Projectile.frame = 1;}
            }
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, -1f), Main.rand.NextFloat(1f, -1f));
                Vector2 offset = Projectile.Center;
                offset.X += Main.rand.NextFloat(-10f, 10f);
                offset.Y += Main.rand.NextFloat(-10f, 10f);
                Dust dust = Dust.NewDustPerfect(offset, DustID.PurpleTorch, velocity * 0, 100, Color.White, Main.rand.NextFloat(0.03f, 0.6f));

                dust.noLight = false;
                dust.noGravity = true;
                dust.fadeIn = Main.rand.NextFloat(0.5f, 1.5f);
            }
        }
    }
}
