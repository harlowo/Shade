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
using Shade.Common.Base;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;

namespace Shade.Common.Base
{
    public abstract class BaseSword : ModProjectile
    {
        public override string Texture => "";
        public abstract bool UseRecoil { get; }
        public abstract bool DoSpin { get; }
        public abstract Color BackDarkColor { get; }
        public abstract Color MiddleMediumColor { get; }
        public abstract Color FrontLightColor { get; }
        public abstract bool CenterOnPlayer { get; }
        public abstract float ScaleMulti { get; }
        public abstract float ScaleAdder { get; }
        public abstract bool Rotate45Degrees { get; }
        public abstract int DustType { get; }
        public enum ActionState
        {
            Swinging, //maybe add reciol or smth, idk
        }
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.hide = false;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public ref float AI_State => ref Projectile.ai[0];
        public ref float SwingDirection => ref Projectile.ai[1];//0 is normal, 1 is reversed
        public ref float Rotation => ref Projectile.localAI[1];
        public ref float PlayerDirection => ref Projectile.ai[2];
        public abstract float BaseDistance { get; } //change depending on the dist from the hilt to the guard
        public abstract Color BladeLightingColor { get; }
        float distance = 0;
        float swingAnimationProgress;
        float swingAnimationProgressMax = 10f;
        float startingPosition = 0f;
        double deg;
        /// <summary>
        /// Called in onhitnpc, useful for adding effects and such
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="hitInfo"></param>
        /// <param name="dmgDealt"></param>
        public virtual void OnHiEffects(NPC npc, NPC.HitInfo hitInfo, int dmgDealt) { }
        public override bool PreAI()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (projOwner.dead && !projOwner.active || projOwner.noItems || projOwner.CCed)
            {
                //Disappear when the player dies, or otherwise cannot use items
                Projectile.Kill();
            }
            return true;
        }
        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            Projectile.scale = 1f;
            projOwner.heldProj = Projectile.whoAmI;//The projectile draws in front of the player.
            RotateArms(projOwner);
            Projectile.alpha -= 10;

            switch (AI_State)
            {
                case (float)ActionState.Swinging:
                    SwingAnimation(projOwner);
                    break;
            }

            deg = Rotation;
            double rad = deg * (Math.PI / 180);
            double dist = distance;
            if (CenterOnPlayer)
            {
                Projectile.position.X = projOwner.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
                Projectile.position.Y = projOwner.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            }
            else
            {
                Projectile.position.X = Main.MouseWorld.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
                Projectile.position.Y = Main.MouseWorld.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            }
            OrientSprite(projOwner);
            MathHelper.Clamp(Projectile.alpha, 0, 255);

        }
        private void SwingAnimation(Player projOwner)
        {
            //Remember this is activating every tick.
            if (swingAnimationProgress == 0f)
            {
                startingPosition = MathHelper.ToDegrees((float)Math.Atan2(Main.MouseWorld.Y - projOwner.Center.Y, Main.MouseWorld.X - projOwner.Center.X)) - 180;
                if (UseRecoil)
                {
                    swingAnimationProgressMax = projOwner.itemTimeMax / 2; //Half of the time spent after using the item is this animation.
                }
                else
                {
                    swingAnimationProgressMax = projOwner.itemTimeMax * 0.9f;
                }
                if (SwingDirection == 0)//0 is normal, 1 is reverse
                {
                    int projType = ProjectileType<SwordEffect>();
                    int proj = Projectile.NewProjectile(projOwner.GetSource_FromThis(), projOwner.MountedCenter, new Vector2(projOwner.direction, 0f), projType, 0, 0, projOwner.whoAmI, projOwner.direction, swingAnimationProgressMax, Projectile.scale);
                    SwordEffect projectile = Main.projectile[proj].ModProjectile as SwordEffect;
                    projectile.backDarkColor = BackDarkColor;
                    projectile.middleMediumColor = MiddleMediumColor;
                    projectile.frontLightColor = FrontLightColor;
                    projectile.scaleAdder = ScaleAdder;
                    projectile.scaleMulti = ScaleMulti;
                    projectile.DustType = DustType;
                    if (DoSpin)
                    {
                        projectile.spin = true;
                    }
                    if (!CenterOnPlayer)
                    {
                        projectile.centerOnMouse = true;
                    }
                }
                else
                {
                    int projType = ProjectileType<SwordEffect>();

                    if (PlayerDirection == 1)
                    {
                        int proj = Projectile.NewProjectile(projOwner.GetSource_FromThis(), projOwner.MountedCenter, new Vector2(projOwner.direction, 0f), projType, 0, 0, projOwner.whoAmI, -1, swingAnimationProgressMax, Projectile.scale);
                        SwordEffect projectile = Main.projectile[proj].ModProjectile as SwordEffect;
                        projectile.backDarkColor = BackDarkColor;
                        projectile.middleMediumColor = MiddleMediumColor;
                        projectile.frontLightColor = FrontLightColor;
                        projectile.scaleAdder = ScaleAdder;
                        projectile.scaleMulti = ScaleMulti;
                        projectile.LightingColor = BladeLightingColor;
                        if (DoSpin)
                        {
                            projectile.spin = true;
                        }
                    }
                    else
                    {
                        int proj = Projectile.NewProjectile(projOwner.GetSource_FromThis(), projOwner.MountedCenter, new Vector2(projOwner.direction, 0f), projType, 0, 0, projOwner.whoAmI, 1, swingAnimationProgressMax, Projectile.scale);
                        SwordEffect projectile = Main.projectile[proj].ModProjectile as SwordEffect;
                        projectile.backDarkColor = BackDarkColor;
                        projectile.middleMediumColor = MiddleMediumColor;
                        projectile.frontLightColor = FrontLightColor;
                        projectile.scaleAdder = ScaleAdder;
                        projectile.scaleMulti = ScaleMulti;
                        if (DoSpin)
                        {
                            projectile.spin = true;
                        }
                    }
                }
                distance = BaseDistance;
                Projectile.alpha = 0;
            }
            int bonusMovement = 0;
            if (DoSpin)
            {
                bonusMovement = 360;
            }

            if (PlayerDirection == 1)
            {
                if (SwingDirection == 0)//0 is normal, 1 is reverse
                {
                    Rotation = MathHelper.Lerp(startingPosition - 100, startingPosition + 100 + bonusMovement, EaseHelper.InOutQuad(swingAnimationProgress / swingAnimationProgressMax));

                }
                else
                {
                    Rotation = MathHelper.Lerp(startingPosition + 90, startingPosition - 90 - bonusMovement, EaseHelper.InOutQuad(swingAnimationProgress / swingAnimationProgressMax));
                }
            }
            else
            {
                if (SwingDirection == 0)//0 is normal, 1 is reverse
                {
                    Rotation = MathHelper.Lerp(startingPosition + 100, startingPosition - 100 - bonusMovement, EaseHelper.InOutQuad(swingAnimationProgress / swingAnimationProgressMax));
                }
                else
                {
                    Rotation = MathHelper.Lerp(startingPosition - 100, startingPosition + 100 + bonusMovement, EaseHelper.InOutQuad(swingAnimationProgress / swingAnimationProgressMax));
                }
            }

            if (swingAnimationProgress / swingAnimationProgressMax > 0.4f && !UseRecoil)
            {
                Projectile.alpha += 40;
            }

            if (swingAnimationProgress >= swingAnimationProgressMax && swingAnimationProgress != 0f)
            {
                Projectile.Kill();
            }
            swingAnimationProgress += 1f;
        }
        private void OrientSprite(Player projOwner)
        {
            if (CenterOnPlayer)
            {
                if (Rotate45Degrees)
                {
                    if (PlayerDirection == 1)
                    {
                        if (Projectile.ai[1] == 1)
                        {
                            Projectile.rotation = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-45f);
                        }
                        else
                        {
                            Projectile.rotation = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-135f);
                        }
                    }
                    else
                    {
                        if (Projectile.ai[1] == 1)
                        {
                            Projectile.rotation = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-135f);
                        }
                        else
                        {
                            Projectile.rotation = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-45f);
                        }
                    }


                }
                else
                {
                    Projectile.rotation = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-90f);

                }

            }
            else
            {
                if (Rotate45Degrees)
                {
                    if (Projectile.ai[1] == 1)
                    {
                        Projectile.rotation = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-90f);


                    }
                    else
                    {
                        Projectile.rotation = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-135f);

                    }

                }
                else
                {
                    Projectile.rotation = Vector2.Normalize(Main.MouseWorld - Projectile.Center).ToRotation() + MathHelper.ToRadians(-90f);

                }

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = ((Projectile.spriteDirection > 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            if (PlayerDirection == 1)
            {
                if (Projectile.ai[1] == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    spriteEffects = SpriteEffects.None;
                }
            }
            else
            {
                if (Projectile.ai[1] == 1)
                {
                    spriteEffects = SpriteEffects.None;
                }
                else
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            if (swingAnimationProgress > 2)
            {
                Main.EntitySpriteDraw(texture,
                   Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                   sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            return false;
        }
        private void RotateArms(Player projOwner)
        {
            projOwner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (projOwner.Center - new Vector2(Projectile.Center.X + (projOwner.velocity.X * 0.05f), Projectile.Center.Y + (projOwner.velocity.Y * 0.05f))).ToRotation() + MathHelper.PiOver2);
        }
        public override void OnKill(int timeLeft)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //optionally do smth on hits
            OnHiEffects(target, hit, damageDone);
        }

    }
    public class SwordEffect : ModProjectile
    {
        //example mod code lmao
        public override string Texture => "Shade/Assets/VanillaSlashEffect";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            Main.projFrames[Type] = 4; // This projectile has 4 frames.
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false; //Does no damage as the melee swing will take care of it 
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public Color LightingColor;
        public int DustType;
        float startingRotation;
        public Color backDarkColor = new Color(235, 235, 235, 0); // 
        public Color middleMediumColor = new Color(245, 245, 245, 0); // 
        public Color frontLightColor = new Color(255, 255, 255, 0); // 
        public bool spin = false;
        public bool centerOnMouse = false;
        public float scaleMulti = 0.6f; // Excalibur, Terra Blade, and The Horseman's Blade is 0.6f; True Excalibur is 1f; default is 0.2f 
        public float scaleAdder = 1f; // Excalibur, Terra Blade, and The Horseman's Blade is 1f; True Excalibur is 1.2f; default is 1f 
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0] == 0f)
            {
                startingRotation = MathHelper.ToDegrees((float)Math.Atan2(Main.MouseWorld.Y - player.Center.Y, Main.MouseWorld.X - player.Center.X));

            }
            Projectile.localAI[0]++; // Current time that the projectile has been alive.
            float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
            float direction = Projectile.ai[0];
            float velocityRotation = Projectile.velocity.ToRotation();
            if (spin)
            {
                if (Projectile.ai[0] == 1)
                {
                    Projectile.rotation = MathHelper.ToRadians(MathHelper.Lerp(startingRotation - 115, startingRotation + 45 + 360, EaseHelper.InOutQuad(percentageOfLife))); // Set the rotation to our to the new rotation we calculated.
                }
                else
                {
                    Projectile.rotation = MathHelper.ToRadians(MathHelper.Lerp(startingRotation + 115, startingRotation - 45 - 360, EaseHelper.InOutQuad(percentageOfLife))); // Set the rotation to our to the new rotation we calculated.

                }
            }
            else
            {
                if (Projectile.ai[0] == 1)
                {
                    Projectile.rotation = MathHelper.ToRadians(MathHelper.Lerp(startingRotation - 115, startingRotation + 45, EaseHelper.InOutQuad(percentageOfLife))); // Set the rotation to our to the new rotation we calculated.
                }
                else
                {
                    Projectile.rotation = MathHelper.ToRadians(MathHelper.Lerp(startingRotation + 115, startingRotation - 45, EaseHelper.InOutQuad(percentageOfLife))); // Set the rotation to our to the new rotation we calculated.

                }
            }
            if (centerOnMouse)
            {
                Projectile.Center = Main.MouseWorld - Projectile.velocity;
            }
            else
            {
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;

            }
            Projectile.scale = scaleAdder + percentageOfLife * scaleMulti;
            Lighting.AddLight(Projectile.Center, LightingColor.ToVector3());
            float dustRotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
            Vector2 dustPosition = Projectile.Center + dustRotation.ToRotationVector2() * 84f * Projectile.scale;
            Vector2 dustVelocity = (dustRotation + Projectile.ai[0] * MathHelper.PiOver2).ToRotationVector2();
            if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
            {
                Color dustColor = Color.Lerp(Color.White, Color.White, Main.rand.NextFloat() * 0.3f);
                Dust coloredDust = Dust.NewDustPerfect(Projectile.Center + dustRotation.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), DustType, dustVelocity * 1f, 100, dustColor, 0.4f);
                coloredDust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                coloredDust.noGravity = true;
            }
            Projectile.scale *= Projectile.ai[2]; // Set the scale of the projectile to the scale of the item.
            // If the projectile is as old as the max animation time, kill the projectile.
            if (Projectile.localAI[0] >= Projectile.ai[1])
            {
                Projectile.Kill();
            }

            // This for loop spawns the visuals when using Flasks (weapon imbues)
            for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2)
            {
                Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
                Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // This is how large the circumference is, aka how big the range is. Vanilla uses 94f to match it to the size of the texture.
            float coneLength = 94f * Projectile.scale;
            // This number affects how much the start and end of the collision will be rotated.
            // Bigger Pi numbers will rotate the collision counter clockwise.
            // Smaller Pi numbers will rotate the collision clockwise.
            // (Projectile.ai[0] is the direction)
            float collisionRotation = MathHelper.Pi * 2f / 25f * Projectile.ai[0];
            float maximumAngle = MathHelper.PiOver4; // The maximumAngle is used to limit the rotation to create a dead zone.
            float coneRotation = Projectile.rotation + collisionRotation;

            // Uncomment this line for a visual representation of the cone. The dusts are not perfect, but it gives a general idea.
            // Dust.NewDustPerfect(Projectile.Center + coneRotation.ToRotationVector2() * coneLength, DustID.Pixie, Vector2.Zero);
            // Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, new Vector2((float)Math.Cos(maximumAngle) * Projectile.ai[0], (float)Math.Sin(maximumAngle)) * 5f); // Assumes collisionRotation was not changed

            // First, we check to see if our first cone intersects the target.
            if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle))
            {
                return true;
            }

            // The first cone isn't the entire swinging arc, though, so we need to check a second cone for the back of the arc.
            float backOfTheSwing = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.3f, Projectile.ai[1] * 0.5f, 1f, 0f);
            if (backOfTheSwing > 0f)
            {
                float coneRotation2 = coneRotation - MathHelper.PiOver4 * Projectile.ai[0] * backOfTheSwing;

                // Uncomment this line for a visual representation of the cone. The dusts are not perfect, but it gives a general idea.
                // Dust.NewDustPerfect(Projectile.Center + coneRotation2.ToRotationVector2() * coneLength, DustID.Enchanted_Pink, Vector2.Zero);
                // Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, new Vector2((float)Math.Cos(backOfTheSwing) * -Projectile.ai[0], (float)Math.Sin(backOfTheSwing)) * 5f); // Assumes collisionRotation was not changed

                if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation2, maximumAngle))
                {
                    return true;
                }
            }

            return false;
        }

        public override void CutTiles()
        {
            // Here we calculate where the projectile can destroy grass, pots, Queen Bee Larva, etc.
            Vector2 starting = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
            Vector2 ending = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
            float width = 60f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + starting, Projectile.Center + ending, width, DelegateMethods.CutTiles);
        }
        //THANK YOU EXAMPLE MOD
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = texture.Frame(1, 4); // The sourceRectangle says which frame to use.
            Vector2 origin = sourceRectangle.Size() / 2f;
            float scale = Projectile.scale * 1.1f;
            SpriteEffects spriteEffects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.
            if (Projectile.ai[0] != 1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
            float lerpTime = Utils.Remap(percentageOfLife, 0f, 0.6f, 0f, 1f) * Utils.Remap(percentageOfLife, 0.6f, 1f, 1f, 0f);
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);



            Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
            whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
            Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
            faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
            faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

            // Back part
            Main.EntitySpriteDraw(texture, position, sourceRectangle, backDarkColor * lightingColor * lerpTime, Projectile.rotation + Projectile.ai[0] * MathHelper.PiOver4 * -1f * (1f - percentageOfLife), origin, scale, spriteEffects, 0f);
            // Very faint part affected by the light color
            Main.EntitySpriteDraw(texture, position, sourceRectangle, faintLightingColor * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
            // Middle part
            Main.EntitySpriteDraw(texture, position, sourceRectangle, middleMediumColor * lightingColor * lerpTime * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);
            // Front part
            Main.EntitySpriteDraw(texture, position, sourceRectangle, frontLightColor * lightingColor * lerpTime * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);
            // Thin top line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * lerpTime, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
            // Thin middle line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
            // Thin bottom line (final frame)
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

            // This draws some sparkles around the circumference of the swing.
            for (float i = 0f; i < 8f; i += 1f)
            {
                float edgeRotation = Projectile.rotation + Projectile.ai[0] * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0];
                Vector2 drawpos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 6f) * scale;
                DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0) * lerpTime * (i / 9f), middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(percentageOfLife, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }

            // This draws a large star sparkle at the front of the projectile.
            Vector2 drawpos2 = position + (Projectile.rotation + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0]).ToRotationVector2() * ((float)texture.Width * 0.5f - 4f) * scale;
            DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos2, new Color(255, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(percentageOfLife, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

            // Uncomment this line for a visual representation of the projectile's size.
            // Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, position, sourceRectangle, Color.Orange * 0.75f, 0f, origin, scale, spriteEffects);

            return false;
        }

        // Copied from Main.DrawPrettyStarSparkle() which is private
        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D sparkleTexture = TextureAssets.Extra[98].Value;
            Color bigColor = shineColor * opacity * 0.5f;
            bigColor.A = 0;
            Vector2 origin = sparkleTexture.Size() / 2f;
            Color smallColor = drawColor * 0.5f;
            float lerpValue = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
            Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
            bigColor *= lerpValue;
            smallColor *= lerpValue;
            // Bright, large part
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
            // Dim, small part
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
        }
    }
}
