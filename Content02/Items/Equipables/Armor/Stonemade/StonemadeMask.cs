using Terraria.Audio;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Shade.Content02.Items.Materials;

namespace Shade.Content02.Items.Equipables.Armor.Stonemade
{
    [AutoloadEquip(EquipType.Head)]

    public class StonemadeMask : ModItem
    {
        public bool HellrockSet = false;
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 5;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StonemadeBreastplate>() && legs.type == ModContent.ItemType<StonemadeGreaves>();
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            if (player.GetModPlayer<StonemadeSetModifier>().blocking)
            {
                player.moveSpeed *= 0.5f;
                player.accRunSpeed = 0;
            }
            player.GetModPlayer<StonemadeSetModifier>().StonemadeSet = true;
            player.setBonus = this.GetLocalization("SetBonus").Value;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 30);
            recipe.AddRecipeGroup("Any Iron Ores", 15);
            recipe.AddIngredient(ModContent.ItemType<SoulOfAnger>(), 5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    public class StonemadeSetModifier : ModPlayer
    {
        public bool blocking = false;
        public int blockTime = 0;
        public bool parrying = false;
        public int parryTime = 26;
        public bool parried = false;
        public bool StonemadeSet = false;
        public override void ResetEffects()
        {
            StonemadeSet = false;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (StonemadeSet)
            {
                if (triggersSet.MouseRight && !blocking)
                {
                    blocking = true;
                }
                else if (triggersSet.MouseRight && blocking)
                {
                    blockTime++;
                    if (blockTime >= 13)
                    {
                        blockTime = 13;
                        if (parryTime > 0)
                        {
                            parrying = true;
                            parryTime--;
                        }
                        else
                        {
                            parrying = false;
                            parryTime = 0;
                        }
                    }
                }
                else if (!triggersSet.MouseRight && blocking && blockTime > 0)
                {
                    parryTime = 0;
                    parrying = false;
                    blockTime--;
                }
                else if (!(triggersSet.MouseRight && blockTime > 0) && blocking)
                {
                    parrying = false;
                    parryTime = 26;
                    blockTime = 0;
                    blocking = false;
                }
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (blocking)
            {
                modifiers.FinalDamage /= 2;
                modifiers.Knockback *= 0;
            }
            if (parrying)
            {
                modifiers.FinalDamage *= 0.001f;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (blocking)
            {
                modifiers.FinalDamage /= 2;
                modifiers.Knockback *= 0;
            }
            if (parrying)
            {
                modifiers.FinalDamage *= 0.002f;
            }
        }
        public override void OnHitByNPC(NPC npc, Terraria.Player.HurtInfo hurtInfo)
        {
            if (parrying)
            {
                parrying = false;
                parryTime = 0;
                SoundEngine.PlaySound(SoundID.Item14);
                Player.AddBuff(BuffID.ParryDamageBuff, 180);
            }
        }
        public override void OnHitByProjectile(Projectile proj, Terraria.Player.HurtInfo hurtInfo)
        {
            if (parrying)
            {
                parrying = false;
                parryTime = 0;
                SoundEngine.PlaySound(SoundID.Item14);
                Player.AddBuff(BuffID.ParryDamageBuff, 180);
            }
        }
    }
    public class StonemadeDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Terraria.Player player = drawInfo.drawPlayer;
            StonemadeSetModifier parryInfo = player.GetModPlayer<StonemadeSetModifier>();
            int frame = (int)Math.Floor((decimal)(parryInfo.blockTime / 2.5));

            Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Stonemade/Parry", AssetRequestMode.ImmediateLoad).Value;
            Rectangle source = new(0, 42 * (frame - 1), 22, 42);

            drawInfo.DrawDataCache.Add(new DrawData(texture,
                player.MountedCenter.Floor() + (player.gfxOffY + -2) * Vector2.UnitY - Main.screenPosition,
                source,
                drawInfo.colorArmorBody,
                0,
                source.Size() / 2f,
                1,
                drawInfo.playerEffect
                ));
        }
    }
}
