using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Weapons
{
    public class BarbedWire : ModItem
    {
        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<BarbedWireProjectile>(), 20, 2, 10, 30);
            Item.rare = ItemRarityID.Green;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
    }
    public class BarbedWireProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            Projectile.DefaultToWhip();

            // use these to change from the vanilla defaults
            Projectile.WhipSettings.Segments = 21;
            Projectile.WhipSettings.RangeMultiplier = 0.35f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float ChargeTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        // This example uses PreAI to implement a charging mechanic.
        // If you remove this, also remove Item.channel = true from the item's SetDefaults.
        public override bool PreAI()
        {
            Terraria.Player owner = Main.player[Projectile.owner];

            // Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
            if (!owner.channel || ChargeTime >= 120)
            {
                return true; // Let the vanilla whip AI run.
            }

            if (++ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
                Projectile.WhipSettings.Segments++;

            // Increase range up to 2x for full charge.
            Projectile.WhipSettings.RangeMultiplier += 1f / 120f;

            // Reset the animation and item timer while charging.
            owner.itemAnimation = owner.itemAnimationMax;
            owner.itemTime = owner.itemTimeMax;

            return false; // Prevent the vanilla whip AI from running.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.5f); // Multihit penalty. Decrease the damage the more enemies the whip hits.
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        public static void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }
        public List<Projectile> zapList = new List<Projectile>();
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            //Main.DrawWhip_WhipBland(Projectile, list);
            // The code below is for custom drawing.
            // If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
            // However, you must adhere to how they draw if you do.

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!
                Rectangle frame = new Rectangle(0, 0, 22, 26); // The size of the Handle (measured in pixels)
                Vector2 origin = new Vector2(11, 13); // Offset for where the player's hand will start measured from the top left of the image.
                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.
                if (i == list.Count - 2)
                {
                    // This is the head of the whip. You need to measure the sprite to figure out these values.
                    frame.Y = 86; // Distance from the top of the sprite to the start of the frame.
                    frame.Height = 22; // Height of the frame.

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                    if (scale > 1f)
                    {
                        int zap = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<BarbedWireZap>(), 10, 1);
                        zapList.Add(Main.projectile[zap]);
                        if (zapList.Count > 1)
                        {
                            Main.NewText(ModContent.ProjectileType<BarbedWireZap>());
                            Main.projectile[zap].ai[0] = zapList[zapList.Count - 2].whoAmI;
                            Main.projectile[zap].ai[1] = scale;
                            Main.projectile[zap].ai[2] = Projectile.owner;
                        }
                    }
                }
                else if (i % 3 == 0)
                {
                    // Third segment
                    frame.Y = 66;
                    frame.Height = 20;
                }
                else if (i % 2 == 0)
                {
                    // Second Segment
                    frame.Y = 46;
                    frame.Height = 20;
                }
                else
                {
                    // First Segment
                    frame.Y = 26;
                    frame.Height = 20;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
    public class BarbedWirePlayer : ModPlayer
    {
        public bool rmb = false;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.HeldItem.type == ModContent.ItemType<BarbedWire>() && triggersSet.MouseRight)
            {
                rmb = true;
            }
            else rmb = false;
        }
    }

    public class BarbedWireZap : ModProjectile
    {
        public Vector2 targetPos = Vector2.Zero;
        public int pause = 0;
        public int speed;
        public const int inertia = 80;
        public Vector2 random = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            targetPos = Projectile.Center;
            targetPos -= random;

            if (pause == 0)
            {
                random = new Vector2(Main.rand.Next(20), Main.rand.Next(20));
                Projectile.Center = targetPos + random;
                pause = 5;
            }
            else
            {
                pause--;
            }

            Projectile.velocity = (Projectile.velocity * (Projectile.ai[1] * (inertia - 1)) + Projectile.Center.DirectionTo(Main.MouseWorld) * speed) / (Projectile.ai[1] * inertia);

            if (Main.player[(int)Projectile.ai[2]].GetModPlayer<BarbedWirePlayer>().rmb)
            {
                speed = 20;
            }
            else
            {
                speed = 0;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>("TheExpando/Items/Weapons/GlowCircle", AssetRequestMode.ImmediateLoad).Value;
            if (Main.projectile[(int)Projectile.ai[0]].type == ModContent.ProjectileType<BarbedWireZap>())
            {
                Projectile target = Main.projectile[(int)Projectile.ai[0]];
                Rectangle source = new Rectangle(0, 0, 17, 17);
                for (float i = 0; i < target.Distance(Projectile.Center); i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, i / target.Distance(Projectile.Center));
                    Color color_1 = new Color(0.025f, 0.025f, 0.1f, 0f);
                    color_1 *= Main.dayTime ? 2 - (float)Math.Abs(Math.Sin(MathHelper.ToRadians((float)(Main.time / 300)))) : 2f;
                    Lighting.AddLight((int)pos.X, (int)pos.Y, 1f, 1f, 1f);
                    color_1 = Color.Lerp(color_1, lightColor, 0.05f);
                    Main.EntitySpriteDraw(new DrawData(glowTexture, pos - Main.screenPosition, source, color_1, Main.rand.Next(6265) / 1000, source.Size() / 2f, 3f, 0));
                }
                for (float i = 0; i < target.Distance(Projectile.Center); i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, i / target.Distance(Projectile.Center));
                    Color color = new Color(0.1f, 0.1f, 0.4f, 0f);
                    color *= Main.dayTime ? 2 - (float)Math.Abs(Math.Sin(MathHelper.ToRadians((float)(Main.time / 300)))) : 2f;
                    color = Color.Lerp(color, lightColor, 0.05f);
                    Main.EntitySpriteDraw(new DrawData(glowTexture, pos - Main.screenPosition, source, color, Main.rand.Next(6265) / 1000, source.Size() / 2f, 1f, 0));
                }
            }
            return false;
        }
            public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            {
                if (Main.projectile[(int)Projectile.ai[0]].type == ModContent.ProjectileType<BarbedWireZap>())
                {
                    Projectile target = Main.projectile[(int)Projectile.ai[0]];
                    return Collision.CheckAABBvLineCollision2(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.position, target.BottomRight);
                }
                else return projHitbox.Intersects(targetHitbox);
            }
    }
}
