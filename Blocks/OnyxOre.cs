using Shade.Blocks.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Blocks
{
    public class OnyxOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;


        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OnyxOre>());
            Item.width = 12;
            Item.height = 12;
            Item.value = 30000;
        }
    }
}