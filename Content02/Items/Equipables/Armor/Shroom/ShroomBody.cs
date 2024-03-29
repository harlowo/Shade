using Shade.Content02.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Shroom
{
    [AutoloadEquip(EquipType.Body)]
    public class ShroomBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.HidesArms[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesBottomSkin[Item.bodySlot] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 6;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 60);
            recipe.AddRecipeGroup("Any Iron Ores", 30);
            recipe.AddIngredient(ModContent.ItemType<SoulOfAnger>(), 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
