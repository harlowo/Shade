using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Shade.Content02.Items.Materials;
using Shade.DrawLayers;
using Shade.System;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Shroom
{
    [AutoloadEquip(EquipType.Head)]

    public class ShroomHead : ModItem
    {
        public static Lazy<Asset<Texture2D>> glowmask;
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            if (!Main.dedServ)
            {
                glowmask = new(() => ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Shroom/ShroomHead_Glowmask"));

                DrawLayers.HeadLayer.RegisterData(Item.headSlot, new DrawLayers.DrawLayerData()
                {
                    Texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Shroom/ShroomHead_Head_Glowmask"),
                    Color = (PlayerDrawSet drawInfo) => Color.White * 0.5f
                });
            }
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 5;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ShroomBody>() && legs.type == ModContent.ItemType<ShroomLegs>();
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.setBonus = this.GetLocalization("SetBonus").Value;
            player.GetModPlayer<ShroomPlayer>().ShroomSet = true;
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
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, 0.1f, 0.1f, 0.6f);
        }
        public override void UpdateEquip(Terraria.Player player)
        {
            Lighting.AddLight(player.Top + new Vector2(0, 10), 0.2f, 0.2f, 0.8f);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.BasicInWorldGlowmask(spriteBatch, glowmask.Value.Value, Color.White * 0.25f, rotation, scale);
        }
    }
    public class ShroomPlayer : ModPlayer
    {
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 50; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public const int DashDuration = 35; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public const float DashVelocity = 10f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        // The fields related to the dash accessory
        public bool ShroomSet;
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 0; // frames remaining in the dash

        public override void ResetEffects()
        {
            ShroomSet = false;

            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
            {
                DashDir = DashRight;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = -1;
            }
        }

        public override void PreUpdateMovement()
        {
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return;
                }

                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.GiveImmuneTimeForCollisionAttack(35);
                Player.velocity = newVelocity;
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
            {
                Player.fullRotationOrigin = new Vector2(Player.width / 2, Player.height / 2);
                Player.fullRotation += MathHelper.ToRadians(360f / 35f) * Math.Sign(Player.velocity.X) * 3f;
                DashTimer--;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Player) && Main.npc[i].active && Player.Hitbox.Intersects(Main.npc[i].Hitbox))
                    {
                        Main.npc[i].AddBuff(BuffID.Confused, 240);
                        return;
                    }
                    else if (i == Main.maxNPCs - 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                Player.fullRotation = 0;
            }
        }
        private bool CanUseDash()
        {
            return ShroomSet
                && Player.dashType == DashID.None // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
                && !Player.setSolar // player isn't wearing solar armor
                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }
    }
}
