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
using System.Collections.Generic;
using Shade.System;

namespace Shade.Content02.Items.Weapons
{
    public class BigSwing : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 54;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.noUseGraphic = true;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.shoot = ModContent.ProjectileType<BigSwingProjectile>();
            Item.shootSpeed = 0;
            Item.UseSound = SoundID.Item1;
            Item.ChangePlayerDirectionOnShoot = true;
            Item.autoReuse = true;
            Item.scale = 1f;
            Item.ArmorPenetration = 20;
            Item.channel = true;
        }
    }
    public class BigSwingProjectile : ModProjectile
    {
        public Terraria.Player player;
        public float lerpCoff;
        public override void SetDefaults()
        {
            Projectile.width = 23;
            Projectile.height = 71;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 1.5f;
        }
        public override void AI()
        {
            if (!player.active || player.HeldItem.type != ModContent.ItemType<BigSwing>())
            {
                Projectile.Kill();
            }
            player = Main.player[Projectile.owner];

            if (player.direction == -1)
            {
                Projectile.rotation = MathHelper.Lerp(-90, 315, lerpCoff);
                Projectile.rotation = MathHelper.ToRadians(Projectile.rotation);
                Projectile.rotation -= MathHelper.PiOver2 * 3f;
                Projectile.spriteDirection = -1;
            }
            else
            {
                Projectile.rotation = MathHelper.Lerp(315, -90, Func.ArcInterp(Projectile.timeLeft / 60f));
                Projectile.rotation = MathHelper.ToRadians(Projectile.rotation);
            }
            Projectile.Center = player.GetFrontHandPosition(0, Projectile.rotation - MathHelper.PiOver2 * 1.5f * player.direction) + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 27f;
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.GetFrontHandPosition(0, Projectile.rotation - MathHelper.PiOver2 * 1.25f), Projectile.Center + (71 * (Projectile.rotation - MathHelper.PiOver2 * 1.25f).ToRotationVector2()), 23, ref point);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>
            {
                Projectile.Center + (71 * (Projectile.rotation - MathHelper.PiOver2 * 1.25f).ToRotationVector2()),
                player.GetFrontHandPosition(0, Projectile.rotation - MathHelper.PiOver2 * 1.5f)
            };
            BarbedWireProjectile.DrawLine(list);
            return true;
        }
    }
    public class BigSwingPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            float rot = 0;
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<BigSwingProjectile>()] > 0)
            {
                for (int i = 0; i < Main.projectile.Length - 1; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<BigSwingProjectile>())
                    {
                        rot = Main.projectile[i].rotation;
                    }
                }
                Player.SetCompositeArmFront(true, 0, rot - MathHelper.PiOver2 * 1.5f * Player.direction);
            }
        }
    }
}
