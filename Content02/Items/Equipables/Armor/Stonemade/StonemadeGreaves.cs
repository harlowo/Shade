using Shade.Content02.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Stonemade
{
    [AutoloadEquip(EquipType.Legs)]
    public class StonemadeGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.OverridesLegs[Item.legSlot] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 5;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 40);
            recipe.AddRecipeGroup("Any Iron Ores", 20);
            recipe.AddIngredient(ModContent.ItemType<SoulOfAnger>(), 7);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
