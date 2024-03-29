
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Shadowflame
{
    [AutoloadEquip(EquipType.Body)]
    public class ShadePlate : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 24;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.25f;
            player.GetCritChance<SummonDamageClass>() += 15;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.25f;
            player.maxMinions += 1;
        }
    }
}