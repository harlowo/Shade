using Shade.Content02.Items.Equipables.Accessories;
using Shade.Content02.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Chef
{
    [AutoloadEquip(EquipType.Head)]

    public class ChefsHat : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 8;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ChefsCostume>() && legs.type == ModContent.ItemType<ChefsPants>();
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            player.GetModPlayer<ChefModifier>().ChefSet = true;
            player.setBonus = this.GetLocalization("SetBonus").Value;
            if (player.HasBuff(BuffID.WellFed))
            {
                player.statDefense += 1;
                player.GetDamage(DamageClass.Default) += 0.05f;
                player.lifeRegen += 1;
            }
            if (player.HasBuff(BuffID.WellFed2))
            {
                player.statDefense += 2;
                player.GetDamage(DamageClass.Default) += 0.1f;
                player.lifeRegen += 2;
            }
            if (player.HasBuff(BuffID.WellFed3))
            {
                player.statDefense += 4;
                player.GetDamage(DamageClass.Default) += 0.2f;
                player.lifeRegen += 4;
            }
            if (player.HasBuff(BuffID.Ironskin))
            {
                player.statDefense += 3;
            }
            if (player.HasBuff(BuffID.Regeneration))
            {
                player.lifeRegen += 5;
            }
            if (player.HasBuff(BuffID.Endurance))
            {
                player.GetModPlayer<ChefModifier>().hasEnduranceBuff = true;
            }
            else
            {
                player.GetModPlayer<ChefModifier>().hasEnduranceBuff = false;
            }
            if (!(player.HasBuff(26) && player.HasBuff(206) && player.HasBuff(207) && player.HasBuff(5) && player.HasBuff(2) && player.HasBuff(114)) && player.HasBuff(ModContent.BuffType<ChefBuff>()))
            {
                player.ClearBuff(ModContent.BuffType<ChefBuff>());
            }
            else if (player.HasBuff(26) || player.HasBuff(206) || player.HasBuff(207) || player.HasBuff(5) || player.HasBuff(2) || player.HasBuff(114))
            {
                player.AddBuff(ModContent.BuffType<ChefBuff>(), -1);
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 20);
            recipe.AddIngredient(ModContent.ItemType<SoulOfAnger>(), 10);
            recipe.AddTile(TileID.Loom);
            recipe.Register();
        }
    }
    public class ChefModifier : ModPlayer
    {
        public bool ChefSet = false;
        public bool hasEnduranceBuff = false;
        public override void ModifyHitByNPC(NPC npc, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (ChefSet && hasEnduranceBuff)
            {
                modifiers.FinalDamage *= 0.9f;
            }
        }
    }
    public class ChefBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}