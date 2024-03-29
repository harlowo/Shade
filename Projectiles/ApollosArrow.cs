using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class ApollosArrow : ModProjectile
    {
        public float topToCenterDist;
        public override void SetDefaults()
        {
            Projectile.rotation = 0;
            topToCenterDist = Projectile.Center.Y - Projectile.Top.Y;
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            Projectile.ArmorPenetration = 100;
            Projectile.penetrate = -1;
        }
        public override bool PreAI()
        {
            Lighting.AddLight(Projectile.Center + (topToCenterDist * Projectile.velocity.ToRotation().ToRotationVector2()), 0.25f, 1f, 0.25f);
            return true;
        }
    }
}
