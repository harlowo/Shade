using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class Sawblade : ModProjectile
    {
        public bool orbiting;
        Projectile magnetProj;
        public float maxSpeed = 8; //change this for faster homing to the magnet
        public int trailTimer;
        public float closestProj = 500f;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1)
                Projectile.timeLeft = 12;
            base.OnSpawn(source);
        }
        public override void AI()
        {
            if (Projectile.ai[0] != 1)
            {
                if (magnetProj == null)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i] != null && Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<Magnet>()
                            && Main.projectile[i].whoAmI != Projectile.whoAmI)
                        {
                            if (Vector2.Distance(Projectile.Center, Main.projectile[i].Center) < closestProj)
                            {
                                Projectile.tileCollide = false;
                                closestProj = Vector2.Distance(Projectile.Center, Main.projectile[i].Center);
                                magnetProj = Main.projectile[i];
                            }
                        }
                    }
                }
                if (magnetProj != null && !magnetProj.active)
                {
                    magnetProj = null;
                    closestProj = 500f;
                }

                if (magnetProj != null && magnetProj.active && magnetProj.type == ModContent.ProjectileType<Magnet>())
                {
                    if (Projectile.Center.Distance(magnetProj.Center) < 240f)
                    {
                        Projectile.velocity += Projectile.Center.DirectionTo(magnetProj.Center) * 0.3f;
                        float velX = Projectile.velocity.X;
                        float velY = Projectile.velocity.Y;
                        velX = MathHelper.Clamp(velX, -maxSpeed, maxSpeed);
                        velY = MathHelper.Clamp(velY, -maxSpeed, maxSpeed);
                        Projectile.velocity = new Vector2(velX, velY);
                    }
                }
                trailTimer++;
                if (trailTimer % 4 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.height / 2, Projectile.width / 2), Vector2.Zero, Projectile.type, 0, 0, Projectile.owner, 1);
                }
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.alpha = 156;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}