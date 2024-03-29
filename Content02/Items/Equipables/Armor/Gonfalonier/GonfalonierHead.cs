using Terraria.Audio;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameInput;
using Shade.System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Shade.Content02.Projectiles;

namespace Shade.Content02.Items.Equipables.Armor.Gonfalonier
{
    [AutoloadEquip(EquipType.Head)]
    public class GonfalonierHead : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 3;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GonfalonierBody>() && legs.type == ModContent.ItemType<GonfalonierLegs>();
        }
        public override void UpdateEquip(Terraria.Player player)
        {
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            if (player.GetModPlayer<GonfalonierPlayer>().WarbannerBonus == true)
            {
                player.statDefense += 5;
                player.lifeRegen += 10;
                player.GetDamage(DamageClass.Generic) += 0.1f;
                player.GetAttackSpeed(DamageClass.Generic) += 0.1f;
                player.moveSpeed += 0.3f;
                player.AddBuff(ModContent.BuffType<WarbannerBuff>(), 18000);
            }
            else if (player.HasBuff(ModContent.BuffType<WarbannerBuff>()))
            {
                player.ClearBuff(ModContent.BuffType<WarbannerBuff>());
            }
            player.GetModPlayer<GonfalonierPlayer>().GonfalonierSet = true;
            player.setBonus = this.GetLocalization("SetBonus").Value;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
    public class GonfalonierPlayer : ModPlayer
    {
        public int Cooldown;
        public bool GonfalonierSet = false;
        public bool WarbannerBonus = false;
        public bool RemoveBonus = true;
        public bool PlaySound = true;
        public override void ResetEffects()
        {
            GonfalonierSet = false;
        }
        public override void PreUpdate()
        {
            if (Cooldown > 0)
            {
                Cooldown--;
            }
            else
            {
                Cooldown = 0;
                if (PlaySound)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Shade/Content02/Assets/Sound/CooldownDone"));
                    PlaySound = false;
                }
            }
            RemoveBonus = true;
            WarbannerBonus = false;
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<Warbanner>()] > 0 && Main.projectile.Length - 1 > 1)
            {
                for (int i = Main.projectile.Length - 1; i > -1; i--)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<Warbanner>() && WarbannerBonus == false && Vector2.Distance(Main.projectile[i].Center, Player.Center) < 480)
                    {
                        RemoveBonus = false;
                        WarbannerBonus = true;
                    }
                }
            }
            if (RemoveBonus)
            {
                RemoveBonus = false;
                WarbannerBonus = false;
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Keybinds.SetKeybind.JustReleased && Cooldown == 0 && GonfalonierSet && Main.myPlayer == Player.whoAmI)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<Warbanner>()] > 0 && Main.projectile.Length - 1 > 1)
                {
                    for (int i = Main.projectile.Length - 1; i > -1; i--)
                    {
                        if (Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<Warbanner>())
                        {
                            Main.projectile[i].Kill();
                            break;
                        }
                    }
                }
                PlaySound = true;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - (Vector2.UnitY * 20), new(0, 0), ModContent.ProjectileType<Warbanner>(), 0, 0, Main.myPlayer);
                Cooldown = 720;
            }
        }
    }
    public class DrawWarbannerBar : PlayerDrawLayer
    {
        public Terraria.Player player;
        public Texture2D border = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Gonfalonier/WarbannerResourceBar", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D bar = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Gonfalonier/WarbannerResourceBarProgress", AssetRequestMode.ImmediateLoad).Value;
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            player = drawInfo.drawPlayer;
            if (player.GetModPlayer<GonfalonierPlayer>().GonfalonierSet == true)
            {
                Bar.CreateCooldownBar(drawInfo, 
                    border,
                    Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 + 80), 
                    new Rectangle(0, 0, 76, 36), 
                    bar, 
                    48, 
                    12, 
                    new Vector2(14, 22), 
                    720, 
                    player.GetModPlayer<GonfalonierPlayer>().Cooldown, 
                    true);
            }
        }
    }
    public class WarbannerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Terraria.Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
        }
    }
}