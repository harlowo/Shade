using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content01.Projectiles
{
    public class OnyxFangProjectile : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 88;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            if (hit.Crit)
            {
                var source = Projectile.GetSource_FromThis();
                for (int i = 0; i < 3; i++) ;

            }
        }
    }
}