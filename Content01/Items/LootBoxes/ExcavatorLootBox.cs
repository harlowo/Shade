using Shade.Blocks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Items.LootBoxes;


public class ExcavatorLootBox : ModItem
{
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 11));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
        ItemID.Sets.BossBag[Type] = true;
        ItemID.Sets.PreHardmodeLikeBossBag[Type] = false; 

        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 38;
        Item.height = 42;
        Item.rare = ItemRarityID.Purple;
        Item.expert = true; 
    }

    public override bool CanRightClick()
    {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot)
    {

        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxBar>(), minimumDropped: 20, maximumDropped: 40));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxLifeModule>(),1 ));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxOre>(), minimumDropped: 20, maximumDropped: 40));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxFragment>(), minimumDropped: 10, maximumDropped: 40));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxCore>(), minimumDropped: 10, maximumDropped: 40));
        itemLoot.Add(
            ItemDropRule.OneFromOptions(
                1,
                ModContent.ItemType<OnyxEnigmaStaff>(),
                ModContent.ItemType<OnyxRailgun>(),
                ModContent.ItemType<OnyxFang>()
            )
        );
        itemLoot.Add(
            ItemDropRule.OneFromOptions(
                20,
                ModContent.ItemType<OnyxHelmet>(),
                ModContent.ItemType<OnyxChestPlate>(),
                ModContent.ItemType<OnyxLeggings>()
            )
        );
    }
}