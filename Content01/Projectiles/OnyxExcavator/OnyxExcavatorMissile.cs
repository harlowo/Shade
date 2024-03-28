using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Projectiles.OnyxExcavator
{
	public class OnyxExcavatorMissile : ModProjectile
	{
        public bool Accelerate = false;

        public int Countdown = 350;

        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 15;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 2.0f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Player target = Main.player[Projectile.owner];
            Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            direction = direction.RotatedBy(MathHelper.ToRadians(15f * Main.rand.NextFloat(1f, 200f))) * 1.0f;
            for (int i = -1; i < 35; i += 2)
            {
                var velocity = direction.RotatedBy(MathHelper.ToRadians(10.0f * i)) * 1.0f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity * 10, ModContent.ProjectileType<OnyxExcavatorLightningBolt>(), 40, 0f, Main.myPlayer);
            }
        }

        public override void AI()
        {
            if (Accelerate == false) 
            { 
                if (Countdown > 0) 
                {
                    Countdown -= 1;
                    Projectile.velocity *= 0.98f; 
                }
                else
                {
                    Projectile.velocity = (Main.player[Projectile.owner].Center - Projectile.position).SafeNormalize(Vector2.Zero) * 1;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
                    Projectile.tileCollide = true;
                    Accelerate = true;
                }
            }
            else { Projectile.velocity *= 1.025f; }
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, -1f), Main.rand.NextFloat(1f, -1f));
                Vector2 offset = Projectile.Center;
                Dust dust = Dust.NewDustPerfect(offset, DustID.PurpleTorch, velocity * 0, 100, Color.White, Main.rand.NextFloat(2.5f, 2.5f));

                dust.noLight = false;
                dust.noGravity = true;
                dust.fadeIn = Main.rand.NextFloat(0.5f, 1.5f);
            }
        }
    }
}
