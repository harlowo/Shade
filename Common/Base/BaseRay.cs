using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;
using System;
using Terraria.Enums;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.Audio;

namespace Shade.Common.Base
{
    public abstract class BaseRay : ModProjectile
    {
        #region Properties
        //make sure to avoid using these for anything else as it can cause unwanted shenanigans!
        public float RotationalSpeed
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Time
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float LaserLength
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        #endregion

        #region Virtuals

        /// <summary>
        /// Handles the logic for the laser. Can be overridden, but you probably won't need to do that.
        /// </summary>
        public virtual void Behavior()
        {
            // Attach to some arbitrary thing/position optionally. 
            AttachToSomething();

            // Ensure the the velocity is a unit vector
            Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY);

            if (++Time >= Lifetime)
            {
                Projectile.Kill();
                return;
            }

            DetermineScale();

            UpdateLaserMotion();

            float idealLaserLength = DetermineLaserLength();
            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.87f); // quickly approach the ideal laser length

            if (LightCastColor != Color.Transparent)
            {
                DelegateMethods.v3_1 = LightCastColor.ToVector3();
                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.width * Projectile.scale, DelegateMethods.CastLight);
            }
        }

        /// <summary>
        /// Handles movement logic for the laser. By default causes arcing/sweeping motion.
        /// </summary>
        public virtual void UpdateLaserMotion()
        {
            float updatedVelocityDirection = Projectile.velocity.ToRotation() + RotationalSpeed;
            Projectile.rotation = updatedVelocityDirection - MathHelper.PiOver2; // Pretty much all lasers have a vertical sheet. (convention)
            Projectile.velocity = updatedVelocityDirection.ToRotationVector2();
        }

        /// <summary>
        /// Calculates the total scale of the laser. By default uses a clamped sine of <see cref="Time"/>.
        /// </summary>
        public virtual void DetermineScale()
        {
            Projectile.scale = (float)Math.Sin(Time / Lifetime * MathHelper.Pi) * ScaleExpandRate * MaxScale; //grow in size, then shrink when at the midpoint
            if (Projectile.scale > MaxScale)
                Projectile.scale = MaxScale;
        }

        /// <summary>
        /// Handles direct attachment to things. The projectile.ai[1] index is reserved for this. Does nothing by default. Useful for bosses and such.
        /// </summary>
        public virtual void AttachToSomething() { }

        /// <summary>
        /// Calculates the current laser's length. By default does not collide with tiles. <see cref="DetermineLaserLength_CollideWithTiles"/> is a generic laser collision method if you want to use it.
        /// </summary>
        /// <returns>The laser length as a float.</returns>
        public virtual float DetermineLaserLength() => MaxLaserLength;

        /// <summary>
        /// An extra, empty by default method that exists so that you can add custom code after all typical AI logic is done.
        /// </summary>
        public virtual void ExtraBehavior() { }
        #endregion

        #region Helpers

        /// <summary>
        /// Calculates the laser length while taking tiles into account.
        /// </summary>
        /// <param name="samplePointCount">The amount of sample points the ray uses. The higher this is, the more precision, but also more calculations done. 
        /// This can cause performance issues depending on the amount of sampling points used, so be cautious.</param>
        public float DetermineLaserLength_CollideWithTiles(int samplePointCount)
        {
            float[] laserLengthSamplePoints = new float[samplePointCount];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.scale, MaxLaserLength, laserLengthSamplePoints);
            return laserLengthSamplePoints.Average();
        }

        protected internal void DrawBeamWithColor(Color beamColor, float scale, int startFrame = 0, int middleFrame = 0, int endFrame = 0)
        {
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, startFrame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, middleFrame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, endFrame);

            // Start texture drawing. (the 1st frame)
            Main.EntitySpriteDraw(LaserBeginTexture, Projectile.Center - Main.screenPosition, startFrameArea, beamColor, Projectile.rotation, LaserBeginTexture.Size() / 2f, scale, SpriteEffects.None, 0);

            float laserBodyLength = LaserLength;
            laserBodyLength -= (startFrameArea.Height / 2 + endFrameArea.Height) * scale;
            Vector2 centerOnLaser = Projectile.Center;
            centerOnLaser += Projectile.velocity * scale * startFrameArea.Height / 2f;

            // Body drawing. (2nd frame)
            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    Main.EntitySpriteDraw(LaserMiddleTexture, centerOnLaser - Main.screenPosition, middleFrameArea, beamColor, Projectile.rotation, LaserMiddleTexture.Width * 0.5f * Vector2.UnitX, scale, SpriteEffects.None, 0);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                }
            }
            // End texture drawing. (3rd frame)
            if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f)
            {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                Main.EntitySpriteDraw(LaserEndTexture, laserEndCenter, endFrameArea, beamColor, Projectile.rotation, LaserEndTexture.Frame(1, 1, 0, 0).Top(), scale, SpriteEffects.None, 0);
            }
        }
        #endregion

        #region Overrides
        public override void AI() // NEVER override AI in a class inheriting this one
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
            Behavior();
            ExtraBehavior();
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackMelee;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.Size.Length() * Projectile.scale, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case-
            if (Projectile.velocity == Vector2.Zero)
                return false;

            DrawBeamWithColor(LaserOverlayColor, Projectile.scale);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.Size.Length() * Projectile.scale, ref _);
        }

        public override bool ShouldUpdatePosition() => false; //without this, everything falls apart D:
        #endregion

        #region Overridable Properties
        public abstract float Lifetime { get; }
        public abstract float MaxScale { get; }
        public abstract float MaxLaserLength { get; } // Be careful with this, as values above 3000 can cause lag
        public abstract Texture2D LaserBeginTexture { get; }
        public abstract Texture2D LaserMiddleTexture { get; }
        public abstract Texture2D LaserEndTexture { get; }
        public virtual float ScaleExpandRate => 4f;
        public virtual Color LightCastColor => Color.White;
        public virtual Color LaserOverlayColor => Color.White * 0.9f;
        #endregion
    }
}
