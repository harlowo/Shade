using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace Shade.Common.ParticleSystem
{
    public class ParticleManager : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawProjectiles += DrawParticles_Projectiles;
            On_Main.DrawDust += DrawParticles_Dust;
            On_Main.DrawInfernoRings += DrawParticles_AfterEverything;
            BaseParticle.ActiveParticles = new();

        }
        
        public override void Unload()
        {
            On_Main.DrawProjectiles -= DrawParticles_Projectiles;
            On_Main.DrawDust -= DrawParticles_Dust;
            On_Main.DrawInfernoRings -= DrawParticles_AfterEverything;
            BaseParticle.ActiveParticles.Clear();
            BaseParticle.ActiveParticles = null;
        }

        private void DrawParticles_Projectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            BaseParticle.DrawParticles(Main.spriteBatch, ParticleLayers.BeforeProjectiles);
            orig(self);
            BaseParticle.DrawParticles(Main.spriteBatch, ParticleLayers.AfterProjectiles);
        }

        private void DrawParticles_Dust(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            BaseParticle.DrawParticles(Main.spriteBatch, ParticleLayers.Dust);
        }

        private void DrawParticles_AfterEverything(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            BaseParticle.DrawParticles(Main.spriteBatch, ParticleLayers.AfterEverything);
        }

        public override void PostUpdateDusts() => BaseParticle.UpdateParticles();

        public override void ClearWorld() => BaseParticle.ActiveParticles?.Clear();
    }
}
