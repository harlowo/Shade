using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using MonoMod;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace Shade.Content02.Items.Equipables.Armor.AdvancedStealth
{
    [AutoloadEquip(EquipType.Head)]

    public class AdvancedStealthHead : ModItem
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
            Item.defense = 5;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AdvancedStealthBody>() && legs.type == ModContent.ItemType<AdvancedStealthLegs>();
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            AdvancedStealthPlayer mPlayer = player.GetModPlayer<AdvancedStealthPlayer>();
            float dmgBonus = 1f * (mPlayer.timeFromShot / 480f + mPlayer.dmgModifier);
            mPlayer.set = true;
            player.GetDamage(DamageClass.Ranged) += dmgBonus;
            player.GetAttackSpeed(DamageClass.Generic) -= 0.1f;
            player.setBonus = this.GetLocalization("SetBonus").WithFormatArgs((int)(dmgBonus * 100)).Value;
        }
        public override void AddRecipes()
        {
        }
    }
    public class AdvancedStealthPlayer : ModPlayer
    {
        public int timeFromShot;
        public bool set = false;
        public float dmgModifier = 240;
        public override void ResetEffects()
        {
            set = false;
        }
        public override void PostUpdate()
        {
            if (set)
            {
                Vector2 vector = Player.velocity + Player.instantMovementAccumulatedThisFrame;
                if (Player.mount.Active && Player.mount.IsConsideredASlimeMount && Player.velocity.Y != 0f && !Player.SlimeDontHyperJump)
                    vector.Y += Player.velocity.Y;
                int num15 = (int)(1f + vector.Length() * 6f);
                if (num15 > Player.speedSlice.Length)
                    num15 = Player.speedSlice.Length;
                float num16 = 0f;
                for (int num17 = num15 - 1; num17 > 0; num17--)
                {
                    Player.speedSlice[num17] = Player.speedSlice[num17 - 1];
                }

                Player.speedSlice[0] = vector.Length();
                for (int m = 0; m < Player.speedSlice.Length; m++)
                {
                    if (m < num15)
                        num16 += Player.speedSlice[m];
                    else
                        Player.speedSlice[m] = num16 / num15;
                }

                num16 /= num15;
                int num18 = 42240;
                int num19 = 216000;
                float num20 = num16 * num19 / num18;
                if (!Player.merman && !Player.ignoreWater)
                {
                    if (Player.honeyWet)
                        num20 /= 4f;
                    else if (Player.wet)
                        num20 /= 2f;
                }
                dmgModifier = 2.75f / num20;
                if (dmgModifier > 0.5f)
                {
                    dmgModifier = 0.5f;
                }
                if (timeFromShot < 240)
                {
                    timeFromShot++;
                }
            }
        }
    }
    public class AdvancedStealthItem : GlobalItem
    {
        public override bool? UseItem(Item item, Terraria.Player player)
        {
            player.GetModPlayer<AdvancedStealthPlayer>().timeFromShot = 0;
            return null;
        }
    }
}