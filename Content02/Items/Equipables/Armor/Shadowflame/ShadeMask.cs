
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Shade.Content02.Projectiles;

namespace Shade.Content02.Items.Equipables.Armor.Shadowflame
{
    [AutoloadEquip(EquipType.Head)]

    public class ShadeMask : ModItem
    {
        public override void SetStaticDefaults()
        { 
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

        }
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 26;
            Item.defense = 21;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ShadePlate>() && legs.type == ModContent.ItemType<ShadeGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowLokis = true;
            player.armorEffectDrawOutlinesForbidden = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.maxMinions += 2;
            Player.jumpHeight += 6;
            player.jumpSpeedBoost += 0.6f;


        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
            player.GetCritChance<SummonDamageClass>() += 15;
        }
    }
}