using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System.Security.Policy;
using static Terraria.ModLoader.ModContent;

namespace Shade
{
    public static class ShadeUtils
    {
        /// <summary>
        /// Given the position of <paramref name="origin"/>, search in all directions for potential npc targets.
        /// </summary>
        /// <param name="origin">Where to start the search from.</param>
        /// <param name="maxDistToCheck">How far to look in all directions.</param>
        /// <param name="ignoreTiles">Whether or not to look for npcs behind solid tiles.</param>
        /// <returns></returns>
        public static NPC ClosestNPCAt(this Vector2 origin, float maxDistToCheck, bool ignoreTiles = true)
        {
            NPC closestTarget = null;
            float distance = maxDistToCheck;
            for (int index = 0; index < Main.npc.Length; index++)
            {
                if (Main.npc[index].CanBeChasedBy(null, false))
                {
                    float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                    bool canHit = true;
                    if (extraDistance < distance && !ignoreTiles)
                        canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

                    if (Vector2.Distance(origin, Main.npc[index].Center) < (distance + extraDistance) && canHit)
                    {
                        distance = Vector2.Distance(origin, Main.npc[index].Center);
                        closestTarget = Main.npc[index];
                    }
                }
            }
            return closestTarget;
        }
        /// <summary>
        /// i dont know what this is but its awesome
        /// </summary>
        //public static readonly int ؠ = 666;

        public static readonly Color[] OnyxColors = new Color[]
        {
          //ranges from dark to light
          //access these colors by using the index they are in the array
           //ex: OnyxColors[1] would be the 2nd color, new Color(30, 9, 44).
           new Color(31, 0, 52), new Color(30, 9, 44), new Color(136, 0, 201), new Color(159, 36, 239)
        };
        
        //useful for things that shrink or expand
        public static float SineInOut(float value) => (0f - (MathF.Cos((value * MathF.PI)) - 1f)) / 2f;

        /// <summary>
        /// Spawns a projectile from above, with diagonal offset relative to xVariance
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetPos">Whwre to aim towards when spawning</param>
        /// <param name="xLimit">max horizontal offset</param>
        /// <param name="xVariance">how random the x offset is</param>
        /// <param name="yLimitLower">lower vertical limit</param>
        /// <param name="yLimitUpper">upper vertical limit</param>
        /// <param name="projSpeed">how fast the projectile should be</param>
        /// <param name="projType">the projectile to spawn</param>
        /// <param name="damage"></param>
        /// <param name="knockback"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static Projectile ProjectileRain(IEntitySource source, Vector2 targetPos, float xLimit, float xVariance, float yLimitLower, float yLimitUpper, float projSpeed, int projType, int damage, float knockback, int owner)
        {
            float x = targetPos.X + Main.rand.NextFloat(-xLimit, xLimit);
            float y = targetPos.Y - Main.rand.NextFloat(yLimitLower, yLimitUpper);
            Vector2 spawnPosition = new Vector2(x, y);
            Vector2 velocity = targetPos - spawnPosition;
            velocity.X += Main.rand.NextFloat(-xVariance, xVariance);
            float speed = projSpeed;
            float targetDist = velocity.Length();
            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;
            return Projectile.NewProjectileDirect(source, spawnPosition, velocity, projType, damage, knockback, owner);
        }

        
         public static Vector2 SafeDirectionTo(this Entity entity, Vector2 destination, Vector2? fallback = null)
         {
             if (!fallback.HasValue)
                 fallback = Vector2.Zero;
             return (destination - entity.Center).SafeNormalize(fallback.Value);
         }
    }
}
