
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Items
{
    public class OnyxFragment : ModItem
    {


        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 15;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;

            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 70);


        }
    }
}