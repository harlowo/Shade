using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Projectiles;

public class OnyxWireWhip : ModProjectile
{
    private float Timer
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

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
        Projectile.WhipSettings.Segments = 50;
        Projectile.WhipSettings.RangeMultiplier = 1.1f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.player[Projectile.owner].ownedProjectileCounts[Type] < 100 && target.type != NPCID.TargetDummy && !target.immortal && !target.SpawnedFromStatue && !NPCID.Sets.CountsAsCritter[target.type] && Projectile.ai[1] != 1f)
        {
            for (int numOfSlashes = 0; numOfSlashes < 4; numOfSlashes++)
            {
                Projectile.ai[1] = 1f;
                Vector2 newV = Main.rand.NextVector2CircularEdge(200f, 300f);
                if (newV.Y < 0f)
                {
                    newV.Y *= -1f;
                }
                newV.Y += 100f;
                Vector2 Vvector = newV.SafeNormalize(Vector2.UnitY) * 6f;
                Projectile.NewProjectile(target.GetSource_OnHit(target), target.position - Vvector * 20f, Vvector * 2f, ModContent.ProjectileType<OnyxBolt>(), (int)(damageDone * 0.5f), 0f, Projectile.owner, target.position.Y, 1f);

            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var list = new List<Vector2>();
        Projectile.FillWhipControlPoints(Projectile, list);

        DrawLine(list);

        //Main.DrawWhip_WhipBland(Projectile, list);
        // The code below is for custom drawing.
        // If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
        // However, you must adhere to how they draw if you do.

        var flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Main.instance.LoadProjectile(Type);
        var texture = TextureAssets.Projectile[Type].Value;

        var pos = list[0];

        for (var i = 0; i < list.Count - 1; i++)
        {
            // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
            // You can change them if they don't!
            var frame = new Rectangle(0, 0, 10, 26); // The size of the Handle (measured in pixels)
            var origin = new Vector2(5, 8); // Offset for where the player's hand will start measured from the top left of the image.
            float scale = 1;

            // These statements determine what part of the spritesheet to draw for the current segment.
            // They can also be changed to suit your sprite.
            if (i == list.Count - 2)
            {
                // This is the head of the whip. You need to measure the sprite to figure out these values.
                frame.Y = 74; // Distance from the top of the sprite to the start of the frame.
                frame.Height = 18; // Height of the frame.

                // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                Projectile.GetWhipSettings(Projectile, out var timeToFlyOut, out var _, out var _);
                var t = Timer / timeToFlyOut;
                scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
            }
            else if (i > 10)
            {
                // Third segment
                frame.Y = 58;
                frame.Height = 16;
            }
            else if (i > 5)
            {
                // Second Segment
                frame.Y = 42;
                frame.Height = 16;
            }
            else if (i > 0)
            {
                // First Segment
                frame.Y = 26;
                frame.Height = 16;
            }

            var element = list[i];
            var diff = list[i + 1] - element;

            var rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
            var color = Lighting.GetColor(element.ToTileCoordinates());

            Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip);

            pos += diff;
        }

        return false;
    }

    // This method draws a line between all points of the whip, in case there's empty space between the sprites.
    private void DrawLine(List<Vector2> list)
    {
        var texture = TextureAssets.FishingLine.Value;
        var frame = texture.Frame();
        var origin = new Vector2(frame.Width / 2, 2);

        var pos = list[0];
        for (var i = 0; i < list.Count - 1; i++)
        {
            var element = list[i];
            var diff = list[i + 1] - element;

            var rotation = diff.ToRotation() - MathHelper.PiOver2;
            var color = Lighting.GetColor(element.ToTileCoordinates(), Color.Purple);
            var scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

            Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None);

            pos += diff;
        }
    }
}