using Shade.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content01.Items
{
    
    [AutoloadEquip(EquipType.Body)]
    public class OnyxChestPlate : ModItem
    {
        public static int MaxlifeIncrease = 20;
        public static readonly int AdditiveGenericDamageBonus = 10;





        public override void SetDefaults()
        {
            Item.width = 18; 
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 5); 
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 26; 
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += MaxlifeIncrease;
            player.GetDamage(DamageClass.Generic) += AdditiveGenericDamageBonus / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<Blocks.OnyxBar>(), 22)
            .AddIngredient(ModContent.ItemType<OnyxCore>(), 3)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}