using Terraria.Audio;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using ReLogic.Content;
using System;

namespace Shade.Content02.Items.Weapons
{
    public class Autumn : ModItem
    {
        public override void SetDefaults()
        {
            Item.channel = true;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 64;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.useTurn = true;
            Item.ChangePlayerDirectionOnShoot = true;
            Item.autoReuse = true;
            Item.shootSpeed = 0f;
            Item.scale = 1f;
            Item.ArmorPenetration = 10;
            Item.noUseGraphic = true;
        }
    }
    public class AutumnPlayer : ModPlayer
    {
        public bool firstArrow = true;
        public bool startSpam = false;
        public float timer = 0;
        public float holdTime;
        public int arrowAmt;
        public Vector2 randomOffset;
        public int handExtend;
        public float handAngle;
        public float angle;
        public int projToShoot;
        public float speed;
        public int damage;
        public float knockback;
        public int usedAmmoID;
        public bool shouldShoot = true;
        public float barrageCD = 60;
        public bool playSound = false;
        public bool shooting = true;
        public override bool CanUseItem(Item item)
        {
            if (item.type == ModContent.ItemType<Autumn>() && barrageCD != 0)
            {
                return false;
            }
            else return true;
        }
        public override void PostUpdate()
        {
            if ((Player.channel || holdTime > 0) && Player.HeldItem.type == ModContent.ItemType<Autumn>() && barrageCD == 0)
            {
                playSound = true;
                if (Player.channel && holdTime > 30)
                {
                    handAngle = Player.MountedCenter.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(holdTime / 20)).ToRotation();
                }
                else
                {
                    handAngle = Player.MountedCenter.DirectionTo(Main.MouseWorld).ToRotation();
                }
                if (handExtend == 0)
                {
                    Player.SetCompositeArmFront(true, 0, handAngle - MathHelper.ToRadians(90f));
                }
                else if (handExtend == 1)
                {
                    Player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.ThreeQuarters, handAngle - MathHelper.ToRadians(90f));
                }
                else if (handExtend == 2)
                {
                    Player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Quarter, handAngle - MathHelper.ToRadians(90f));
                }
                else if (handExtend == 3 || (!Player.channel && arrowAmt > 0))
                {
                    Player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.None, handAngle - MathHelper.ToRadians(90f));
                }
                Player.SetCompositeArmBack(true, 0, handAngle - MathHelper.ToRadians(90f));
                Player.direction = Math.Sign(Player.MountedCenter.DirectionTo(Main.MouseWorld).X);
                if ((!Player.channel || shooting == true) && arrowAmt > 0)
                {
                    shooting = true;
                    handExtend = 3;
                    if (timer == 0)
                    {
                        randomOffset = new Vector2(0, Main.rand.Next(-20, 20));
                        if (!Main.rand.NextBool(10))
                        {
                            shouldShoot = Player.PickAmmo(new Item(ItemID.PlatinumBow), out projToShoot, out speed, out damage, out knockback, out usedAmmoID, true);
                        }
                        else
                        {
                            shouldShoot = Player.PickAmmo(new Item(ItemID.DemonBow), out projToShoot, out speed, out damage, out knockback, out usedAmmoID, false);
                        }
                        if (shouldShoot)
                        {
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.MountedCenter + randomOffset + (32 * handAngle.ToRotationVector2()), (Player.MountedCenter + randomOffset).DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(10) + MathHelper.ToRadians((holdTime - 60) / 4)) * 20f, projToShoot, 20, 5);
                        }
                        timer = 1;
                        arrowAmt--;
                        if (arrowAmt <= 0)
                        {
                            shooting = false;
                            barrageCD = 60;
                            holdTime = 0;
                            timer = 0;
                        }
                    }
                    else
                    {
                        timer -= 1;
                    }
                }
                else
                {
                    if (holdTime <= 120)
                    {
                        holdTime++;
                    }
                    if (holdTime <= 60)
                    {
                        handExtend = (int)Math.Floor((decimal)holdTime / 20);
                        arrowAmt = (int)Math.Floor((decimal)holdTime / 5);
                    }
                }
            }
            else
            {
                holdTime = 0;
                if (playSound == true && barrageCD == 0)
                {
                    SoundEngine.PlaySound(SoundID.ResearchComplete);
                    playSound = false;
                }
            }
            if (barrageCD > 0)
            {
                barrageCD--;
            }
            else
            {
                barrageCD = 0;
            }
        }
    }
    public class AutumnDrawPlayer : PlayerDrawLayer
    {
        public Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Weapons/AutumnSheet", AssetRequestMode.ImmediateLoad).Value;
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Terraria.Player player = drawInfo.drawPlayer;
            Rectangle source = new();
            if (player.GetModPlayer<AutumnPlayer>().shooting == true)
            {
                source = new Rectangle(0, 66, 32, 66);
            }
            else
            {
                source = new Rectangle(0, 66 * player.GetModPlayer<AutumnPlayer>().handExtend, 32, 66);
            }
            if (player.HeldItem.type == ModContent.ItemType<Autumn>() && (player.channel || player.GetModPlayer<AutumnPlayer>().holdTime > 0) && player.GetModPlayer<AutumnPlayer>().barrageCD == 0 /*&& player.GetModPlayer<ApolloPlayer>().*/)
            {
                drawInfo.DrawDataCache.Add(new DrawData(texture,
                    player.MountedCenter + (16 * player.GetModPlayer<AutumnPlayer>().handAngle.ToRotationVector2()) - Main.screenPosition,
                    source,
                    Color.White,
                    player.GetModPlayer<AutumnPlayer>().handAngle,
                    source.Size() / 2f,
                    1,
                    0));
            }
        }
    }
}
