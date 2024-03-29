using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.AdvancedStealth
{
    [AutoloadEquip(EquipType.Body)]
    public class AdvancedStealthBody : ModItem
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
        public override void UpdateEquip(Terraria.Player player)
        {
            player.GetDamage<MagicSummonHybridDamageClass>() += 0.1f;
        }
        public override void AddRecipes()
        {
        }
    }
}
