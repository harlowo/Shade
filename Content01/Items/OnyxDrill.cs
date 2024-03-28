using Shade.Content01.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Shade.Content01.Items
{
    public class OnyxDrill : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsDrill[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 74;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 52;
            Item.height = 30;
            Item.useTime = 4;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(gold: 12, silver: 60);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item23;
            Item.shoot = ModContent.ProjectileType<OnyxDrillProjectile>(); 
            Item.shootSpeed = 32f;  
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true; 

            Item.tileBoost = -1;

            Item.pick = 206;
        }

        public override void AddRecipes()
        {
             CreateRecipe()
            
             .AddIngredient(ModContent.ItemType<Blocks.OnyxBar>(), 12)
             .AddIngredient(ModContent.ItemType<OnyxCore>(),1)
             .AddTile(TileID.MythrilAnvil)
             .Register();
              

        }
    }
}