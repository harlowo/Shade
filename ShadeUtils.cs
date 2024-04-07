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
        public static readonly int ؠ = 666;

        public static readonly Color[] OnyxColors = new Color[]
        {
          //ranges from dark to light
          //access these colors by using the index they are in the array
           //ex: OnyxColors[1] would be the 2nd color, new Color(30, 9, 44).
           new Color(31, 0, 52), new Color(30, 9, 44), new Color(136, 0, 201), new Color(159, 36, 239)
        };
    }
}
