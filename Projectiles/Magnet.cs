using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class Magnet : ModProjectile
    {
        public NPC stuckTo;
        public bool stuckToTile;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.damage = 1;
        }
        public override void AI()
        {
            if (stuckTo != null && stuckTo.active)
            {
                Projectile.Center = stuckTo.Center;
            }
            else if (stuckToTile)
            {
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.velocity.Y += 0.1f; //Gravity
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            base.AI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.active)
            {
                stuckTo = target;
                Projectile.timeLeft = 240;
                Projectile.velocity = Vector2.Zero;
                Projectile.damage = 0;
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            stuckToTile = true;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0;
            Projectile.tileCollide = false;

            return false;
        }
    }
}