using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Blocks
{
    public class OnyxBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 63;

        }

        public override void SetDefaults()
        {
           
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OnyxBar>());
            Item.width = 20;
            Item.height = 20;
            Item.value = 950; 
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OnyxOre>(4)
                .AddTile(TileID.AdamantiteForge)
                .Register();

            CreateRecipe()
            .AddIngredient(ItemID.HallowedBar)
            .AddIngredient(ItemID.SoulofNight)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}