using Terraria;
using Terraria.ModLoader;

namespace Shade.Content01.Dustz
{
    public class OnyxDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale = 1f;
            dust.alpha = 0;
            dust.velocity = dust.velocity * 0.1f;
        }
        public override bool Update(Dust dust)
        {
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), Color.Violet.ToVector3()); //why was it white lol
            dust.noGravity = true; 
            dust.position += dust.velocity;
            dust.velocity *= 0.92f;
            dust.scale *= 0.98f;
            dust.alpha += 12;
            if (dust.scale < 0.5f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}
