using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Shade.Content01.Projectiles;


namespace Shade.Content01.Items
{
    public class OnyxFang : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 88;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.UseSound = SoundID.Item1;

            Item.damage = 87;
            Item.DamageType = DamageClass.MeleeNoSpeed;

            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 8, 0, 0);

            Item.shoot = ModContent.ProjectileType<OnyxFangProjectile>();
            Item.shootSpeed = 9f;

            Item.crit = 15;

            Item.knockBack = 5.5f;

            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (player.itemAnimation == 0)
            {
                float dashSpeed = 22f;
                Vector2 dashVelocity = player.DirectionTo(Main.MouseWorld) * dashSpeed;
                player.velocity = dashVelocity;
                return true;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Blocks.OnyxBar>(), 22)
                .AddIngredient(ModContent.ItemType<OnyxCore>(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
