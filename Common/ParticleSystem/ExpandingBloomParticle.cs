namespace Shade.Common.ParticleSystem
{
    public class ExpandingBloomParticle : BaseExpandingParticle
    {
        //long ahh
        public ExpandingBloomParticle(Vector2 position, Vector2 velocity, Color drawColor, Vector2 startScale, Vector2 endScale, int lifetime, bool useExtraBloom = false, Color? extraBloomColor = null) : base(position, velocity, drawColor, startScale, endScale, lifetime, useExtraBloom, extraBloomColor)
        {
            //when spawning an instance of this particle, make sure to use BaseParticle particle = new ExpandingBloomParticle(yap, yap, yap, yap, yappity, etc), its much more convienient 
        }
    }
}
