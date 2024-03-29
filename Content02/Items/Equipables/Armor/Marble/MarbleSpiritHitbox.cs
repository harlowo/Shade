using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Marble
{

    public class MarbleSpiritHitbox : ModProjectile
    {
        public Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Marble/MarbleSpiritHitbox", AssetRequestMode.ImmediateLoad).Value;
        public Rectangle source = default;
        public Color drawColor = Color.Gold;
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 122;
            Projectile.height = 122;
            source = new Rectangle(0, 0, Projectile.width, Projectile.height);
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.scale = 0.1f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 4f, 0.2f);
                drawColor.A = 0;
                Main.EntitySpriteDraw(new DrawData(texture,
                    Projectile.Center - Main.screenPosition,
                    source,
                    drawColor,
                    0,
                    texture.Size() / 2f,
                    Projectile.scale,
                    0
                    ));
                if (Projectile.scale >= 3.95f)
                {
                    Projectile.Kill();
                }
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int scaledWidth = (int)(100 * Projectile.scale);
            int scaledHeight = (int)(100 * Projectile.scale);

            int scaledX = projHitbox.X + (100 - scaledWidth) / 2;
            int scaledY = projHitbox.Y + (100 - scaledHeight) / 2;

            Rectangle scaledHitbox = new(scaledX, scaledY, scaledWidth, scaledHeight);

            return scaledHitbox.Intersects(targetHitbox);
        }
    }
}
