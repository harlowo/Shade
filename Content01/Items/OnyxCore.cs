
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Items
{
    public class OnyxCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
            Item.rare = ItemRarityID.Yellow;

        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 90);


        }
    }
}