using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Marble
{

    public class MarbleSpirit : ModProjectile
    {
        public const int maxScreamTimer = 30;
        public int screamTimer = maxScreamTimer;
        public int screamTimes = 0;
        public bool parrying = false;
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 88;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Terraria.Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 1)
            {
                Projectile.width = 32;
                Projectile.damage = 1;
                Projectile.knockBack = 60;
                Projectile.position = (player.position + new Vector2(player.width * 2 * player.direction, -player.height)).Floor();
                parrying = true;
                for (int i = Main.projectile.Length - 1; i >= 0; i--)
                {
                    Projectile selected = Main.projectile[i];
                    if (selected != Projectile)
                    {
                        if (Projectile.Hitbox.Intersects(selected.Hitbox) && Math.Sign(player.Center.DirectionTo(selected.Center).X) == Projectile.spriteDirection)
                        {
                            if (selected.type == ProjectileID.PhantasmalDeathray)
                            {

                            }
                            else
                            {
                                selected.Kill();
                            }
                        }
                    }
                }
            }
            else parrying = false;
            if (Projectile.ai[0] == 2)
            {
                if (screamTimer > 0)
                {
                    screamTimer--;
                }
                else
                {
                    screamTimes++;
                    if (screamTimes > 3)
                    {
                        Projectile.Kill();
                    }
                    int hitbox = Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.Center, new(), ModContent.ProjectileType<MarbleSpiritHitbox>(), 10, 60);
                    Main.projectile[hitbox].ai[0] = 0;
                    screamTimer = 10;
                }
            }
        }
    }
}
