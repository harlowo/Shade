using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content01.Items
{

    [AutoloadEquip(EquipType.Head)]
    public class OnyxHelmet : ModItem
    {
        public static readonly int AdditiveGenericDamageBonus = 30;
        public static readonly int AdditiveGenericDamageBonus2 = 20;

        public static LocalizedText SetBonusText { get; private set; }

        public override void SetStaticDefaults()
        {


            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(AdditiveGenericDamageBonus);
            
        }

        public override void SetDefaults()
        {
            Item.width = 18; 
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 15;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<OnyxChestPlate>() && legs.type == ModContent.ItemType<OnyxLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += AdditiveGenericDamageBonus2 / 100f;
            player.moveSpeed += 0.20f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases dealt damage by 30%";
            player.GetDamage(DamageClass.Generic) += AdditiveGenericDamageBonus / 100f;
            player.moveSpeed += 0.50f;

        }


        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<Blocks.OnyxBar>(), 25)
            .AddIngredient(ModContent.ItemType<OnyxCore>(), 3)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}