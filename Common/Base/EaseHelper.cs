using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using static Microsoft.Xna.Framework.MathHelper;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Shade.Common.Base
{
    internal static class EaseHelper
    {
        public static float InQuad(float t) => t * t;
        public static float OutQuad(float t) => 1 - InQuad(1 - t);
        public static float InOutQuad(float t)
        {
            if (t < 0.5) return InQuad(t * 2) / 2;
            return 1 - InQuad((1 - t) * 2) / 2;
        }
        public static float Flip(float x)
        {
            return -1 * x;
        }
    }
}
