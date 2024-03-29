using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class Warbanner : ModProjectile
    {
        public int framePause = 12;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 60;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.timeLeft = 60;
            if (framePause > 0)
            {
                framePause--;
            }
            else
            {
                framePause = 12;
                Projectile.frame++;
                if (Projectile.frame == Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = default;
            return false;
        }
    }
}
