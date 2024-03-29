using Shade.Content02.Items.Equipables.Accessories;
using Shade.Content02.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Chef
{
    [AutoloadEquip(EquipType.Body)]
    public class ChefsCostume : ModItem
    {

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 9;
        }
        public override void UpdateEquip(Terraria.Player player)
        {
            player.moveSpeed *= 0.8f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 25);
            recipe.AddIngredient(ItemID.MonsterLasagna);
            recipe.AddIngredient(ModContent.ItemType<SoulOfAnger>(), 20);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }
}
