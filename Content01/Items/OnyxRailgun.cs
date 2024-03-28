using Terraria.DataStructures;
using Shade.Blocks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Shade.Content01.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Shade.Content01.Items
{
    public class OnyxRailgun : ModItem
    {
        private int ChargeResourceCost; 


  
        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 20;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9.75f;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<RailGunShot>();
            Item.shootSpeed = 3.5f;
            Item.mana = 50;
        }


		

      
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 35;



        public override Vector2? HoldoutOffset() => new Vector2(5, 0);


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.OnyxBlaster)
                .AddIngredient(ModContent.ItemType<Blocks.OnyxBar>(), 50)
                .AddIngredient(ModContent.ItemType<OnyxFragment>(), 10)
                .AddIngredient(ModContent.ItemType<OnyxCore>(), 16)
                .AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}