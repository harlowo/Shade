using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content01.Items
{
    [AutoloadEquip(EquipType.Legs)]
    public class OnyxLeggings : ModItem
    {
        public static readonly int MoveSpeedBonus = 5;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus);

        public override void SetDefaults()
        {
            Item.width = 18; 
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1); 
            Item.rare = ItemRarityID.Yellow; 
            Item.defense = 17; 
        }
        public override void SetStaticDefaults()
        {

        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeedBonus / 100f; 
        }

      
        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<Blocks.OnyxBar>(), 20)
            .AddIngredient(ModContent.ItemType<OnyxCore>(), 3)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}