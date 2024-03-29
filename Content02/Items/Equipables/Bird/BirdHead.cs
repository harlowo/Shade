using Shade.Content02.Items.Equipables.Accessories;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Bird
{
    [AutoloadEquip(EquipType.Head)]

    public class BirdHead : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 1;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BirdBody>() && legs.type == ModContent.ItemType<BirdLegs>();
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            player.maxMinions += 2;
            player.setBonus = this.GetLocalization("SetBonus").Value;
            player.GetModPlayer<BirdPlayer>().BirdSet = true;
        }
        public override void AddRecipes()
        {
        }
    }
    public class BirdPlayer : ModPlayer
    {
        public bool BirdSet = false;
        public override void ResetEffects()
        {
            BirdSet = false;
        }
        public override void PostUpdateEquips()
        {
            if (BirdSet)
            {
                Player.wingsLogic = ContentSamples.ItemsByType[ModContent.ItemType<BirdWings>()].wingSlot;
                Player.wings = ContentSamples.ItemsByType[ModContent.ItemType<BirdWings>()].wingSlot;
                Player.wingTimeMax = 60;
                Player.noFallDmg = true;
            }
        }
    }
}

