
using Terraria.ModLoader;
using Shade.Content02.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Shade.Content02.Items.Equipables.Armor.Druid
{
    [AutoloadEquip(EquipType.Legs)]
    public class DruidLegs : ModItem
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
            Item.defense = 3;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 12);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ModContent.ItemType<SoulOfEvergrowth>(), 3);
            recipe.AddTile(TileID.LivingLoom);
            recipe.Register();
        }
        public override void UpdateEquip(Terraria.Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 3;
        }
    }
}
