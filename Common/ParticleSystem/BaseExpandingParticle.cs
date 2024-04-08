using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Shade.Common.ParticleSystem
{
    public abstract class BaseExpandingParticle : BaseParticle
    {
        //contrary to the name, you can also use this as a shrinking particle
        public readonly Vector2 StartScale;

        public readonly Vector2 EndScale;

        public readonly bool UseBloom;

        public override Texture2D MainTexture => CommonBloomTexture;
        public virtual Vector2 DrawScale => Scale * 0.3f;

        public BaseExpandingParticle(Vector2 position, Vector2 velocity, Color drawColor, Vector2 startScale, Vector2 endScale, int lifetime, bool useExtraBloom = false, Color? extraBloomColor = null)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = StartScale = startScale;
            EndScale = endScale;
            MaxLifetime = lifetime;
            UseBloom = useExtraBloom;
            extraBloomColor ??= Color.White;
            BloomColor = extraBloomColor.Value;
        }

        public sealed override void Update()
        {
            Opacity = MathHelper.Lerp(1f, 0f, SineInOut(LifetimeRatio));
            Scale = Vector2.Lerp(StartScale, EndScale, SineInOut(LifetimeRatio));
        }
        public static float SineInOut(float value) => (0f - (MathF.Cos((value * MathF.PI)) - 1f)) / 2f;
        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, DrawColor with { A = 0 } * Opacity, Rotation, MainTexture.Size() * 0.5f, DrawScale, SpriteEffects.None, 0f);

            if (UseBloom)
                spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.4f * Opacity, Rotation, MainTexture.Size() * 0.5f, DrawScale * 0.66f, SpriteEffects.None, 0f);
        }
    }
}
