using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Shade
{
    public abstract class BaseSpearProjectile : ModProjectile, ILocalizedModType
    {
        public enum SpearType
        {
            TypicalSpear,
            GhastlyGlaiveSpear
        }


        public virtual void Behavior()
        {
            if (SpearAiType == SpearType.TypicalSpear)
            {


                Player player = Main.player[Projectile.owner];


                player.ChangeDir(Projectile.direction);
                player.heldProj = Projectile.whoAmI;
                player.itemTime = player.itemAnimation;

                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);


                Projectile.position += Projectile.velocity * Projectile.ai[0];


                if (Projectile.ai[0] == 0f)
                {
                    Projectile.ai[0] = InitialSpeed;
                    Projectile.netUpdate = true;
                }

                if (player.itemAnimation < player.itemAnimationMax / 3)
                {
                    Projectile.ai[0] -= ReelbackSpeed;
                    if (Projectile.localAI[0] == 0f && EffectBeforeReelback != null && Main.myPlayer == Projectile.owner)
                    {
                        Projectile.localAI[0] = 1f;
                        EffectBeforeReelback.Invoke(Projectile);
                    }
                }
                else
                {
                    Projectile.ai[0] += ForwardSpeed;
                }
                if (player.itemAnimation <= 1)
                    Projectile.Kill();

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.PiOver4;
                if (Projectile.spriteDirection == -1)
                    Projectile.rotation -= MathHelper.PiOver2;
            }
            else if (SpearAiType == SpearType.GhastlyGlaiveSpear)
            {
                Player player = Main.player[Projectile.owner];

                Vector2 playerRelativePoint = player.RotatedRelativePoint(player.MountedCenter, true);

                Projectile.direction = player.direction;
                player.heldProj = Projectile.whoAmI;
                Projectile.Center = playerRelativePoint;
                if (player.dead)
                {
                    Projectile.Kill();
                    return;
                }
                if (!player.frozen)
                {
                    if (Main.player[Projectile.owner].itemAnimation < Main.player[Projectile.owner].itemAnimationMax / 3)
                    {
                        if (Projectile.localAI[0] == 0f && EffectBeforeReelback != null && Main.myPlayer == Projectile.owner)
                        {
                            Projectile.localAI[0] = 1f;
                            EffectBeforeReelback.Invoke(Projectile);
                        }
                    }
                    Projectile.spriteDirection = Projectile.direction = player.direction;
                    if (Projectile.alpha > 0)
                    {
                        Projectile.alpha -= 127;
                        if (Projectile.alpha < 0)
                            Projectile.alpha = 0;
                    }
                    if (Projectile.localAI[0] > 0f)
                    {
                        Projectile.localAI[0] -= 1f;
                    }
                    float inverseAnimationCompletion = 1f - (player.itemAnimation / (float)player.itemAnimationMax);
                    float originalVelocityDirection = Projectile.velocity.ToRotation();
                    float originalVelocitySpeed = Projectile.velocity.Length();


                    Vector2 flatVelocity = Vector2.UnitX.RotatedBy(MathHelper.Pi + inverseAnimationCompletion * MathHelper.TwoPi) *
                        new Vector2(originalVelocitySpeed, Projectile.ai[0]);

                    Projectile.position += flatVelocity.RotatedBy(originalVelocityDirection) +
                        new Vector2(originalVelocitySpeed + TravelSpeed, 0f).RotatedBy(originalVelocityDirection);

                    Vector2 destination = playerRelativePoint + flatVelocity.RotatedBy(originalVelocityDirection) + originalVelocityDirection.ToRotationVector2() * (originalVelocitySpeed + TravelSpeed + 40f);
                    Projectile.rotation = player.AngleTo(destination) + MathHelper.PiOver4 * player.direction; //or this

                    if (Projectile.spriteDirection == -1)
                        Projectile.rotation += MathHelper.Pi;
                }

                if (player.itemAnimation == 2)
                {
                    Projectile.Kill();
                    player.reuseDelay = 2;
                }
            }
        }
        public virtual void ExtraBehavior()
        {

        }
        public override void AI()
        {
            Behavior();
            if ((SpearAiType == SpearType.GhastlyGlaiveSpear && !Main.player[Projectile.owner].frozen) ||
                SpearAiType == SpearType.TypicalSpear)
            {
                ExtraBehavior();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (SpearAiType == SpearType.TypicalSpear)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;
                Vector2 origin = Vector2.Zero;
                Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, 0, 0);
                return false;
            }
            return base.PreDraw(ref lightColor);
        }

        #region Virtual Values
        public virtual float InitialSpeed => 3f;
        public virtual float ReelbackSpeed => 1f;
        public virtual float ForwardSpeed => 0.75f;
        public virtual float TravelSpeed => 22f;
        public virtual Action<Projectile> EffectBeforeReelback => null;
        public virtual SpearType SpearAiType => SpearType.TypicalSpear;
        #endregion
    }
}