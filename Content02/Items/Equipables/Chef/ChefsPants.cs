using Shade.Content02.Items.Equipables.Armor;
using Shade.Content02.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Chef
{
    [AutoloadEquip(EquipType.Legs)]

    public class ChefsPants : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 8;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 15);
            recipe.AddIngredient(ModContent.ItemType<SoulOfAnger>(), 15);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }
}
