using Terraria.Audio;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Shade.System;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Shade.Content02.Projectiles;

namespace Shade.Content02.Items.Weapons
{
    public class SpookyLamp : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.width = 30;
            Item.height = 52;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item117;
            Item.shoot = ModContent.ProjectileType<LampProjectile>();
            Item.shootSpeed = 20f;
            Item.shootsEveryUse = false;
            Item.ChangePlayerDirectionOnShoot = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.mana = 5;
            Item.scale = 1f;
            Item.ArmorPenetration = 10;
            Item.channel = true;
        }
        public override void UpdateEquip(Terraria.Player player)
        {
            player.wingsLogic = 10;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
    public class LampNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int hits = 0;
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (hits < 3)
            {
                hits++;
            }
        }
    }

    public class LampPlayer : ModPlayer
    {
        public bool takeMana = false;
        public bool playAnim = false;
        public int animTimer = 30;
        public override bool CanUseItem(Item item)
        {
            return !playAnim;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (triggersSet.MouseRight && Player.HeldItem.type == ModContent.ItemType<SpookyLamp>() && !Player.ItemAnimationActive)
            {
                for (int i = Main.npc.Length - 1; i > 0; i--)
                {
                    if (Main.npc[i].TryGetGlobalNPC(out LampNPC result) && result.hits > 0 && !Main.npc[i].SpawnedFromStatue)
                    {
                        for (int j = result.hits; j > 0; j--)
                        {
                            Main.npc[i].StrikeNPC(Main.npc[i].CalculateHitInfo((int)(20 * result.hits * Player.GetDamage(DamageClass.Magic).Multiplicative), 0, false, 0, DamageClass.Magic, true));
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Main.npc[i].Center + new Vector2(Main.rand.Next(Main.npc[i].width), Main.rand.Next(Main.npc[i].height)), new Vector2(20, 20).RotatedByRandom(360), ModContent.ProjectileType<LampHealingSpirit>(), 0, 0);
                        }
                        SoundEngine.PlaySound(new SoundStyle($"Shade/Content02/Assets/Sound/Abigail_upgrade_0"), Player.Center);
                        result.hits = 0;
                        playAnim = true;
                        takeMana = true;
                    }
                }
                if (takeMana == true)
                {
                    takeMana = false;
                    Player.statMana = 0;
                }
            }
        }
        public override void PostUpdate()
        {
            if (playAnim == true)
            {
                animTimer--;
                if (animTimer > 20)
                {
                    Player.SetCompositeArmFront(true, 0, 135);
                }
                else if (animTimer > 0)
                {
                    Player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.None, 135);
                }
                if (animTimer <= 1)
                {
                    animTimer = 30;
                    playAnim = false;
                }
            }
        }
    }

    public class LampDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);
        public Terraria.Player player;
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            player = drawInfo.drawPlayer;

            Vector2 position = (player.Center + new Vector2(16 * player.direction, 16)).Floor();
            Rectangle source = new Rectangle(0, 0, 30, 52);
            if (player.ItemAnimationActive && player.HeldItem.type == ModContent.ItemType<SpookyLamp>())
            {
                Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Weapons/SpookyLamp", AssetRequestMode.ImmediateLoad).Value;

                drawInfo.DrawDataCache.Add(new DrawData(texture, position - Main.screenPosition,
                    source,
                    drawInfo.itemColor,
                    0,
                    source.Size() / 2f,
                    1,
                    0));
                Lighting.AddLight(position + Vector2.UnitY * 24, 1f, 0.5f, 0f);
            }
        }
    }
}