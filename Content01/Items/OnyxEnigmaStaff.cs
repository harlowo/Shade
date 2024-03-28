using Microsoft.Xna.Framework;
using Shade.Content01.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Shade.Blocks;

namespace Shade.Content01.Items
{
    public class OnyxEnigmaStaff : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 11;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 43;
            Item.useAnimation = 43;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.useTurn = false;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<EnigmaProj>();
            Item.shootSpeed = 1f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float[] scans = new float[3];
            float dist = player.Distance(Main.MouseWorld);

            Collision.LaserScan(player.Center, player.DirectionTo(Main.MouseWorld), 0, dist, scans);

            dist = 0;
            foreach (float array in scans)
                dist += array / scans.Length;

            Vector2 spawnpos = player.Center + player.DirectionTo(Main.MouseWorld) * dist;
            var p = Projectile.NewProjectileDirect(source, spawnpos, Vector2.Zero, type, damage, knockback, player.whoAmI);
            p.netUpdate = true;

            for (int k = 0; k < 30; k++)
            {
                Vector2 offset = player.DirectionTo(Main.MouseWorld).RotatedBy(0.1f * -Math.Sign(velocity.X)) * 58;

                int dust = Dust.NewDust(player.Center + offset, 1, 1, DustID.PinkTorch);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
                float dustSpeed = Main.rand.Next(23) / 5;
                Main.dust[dust].velocity = new Vector2(velocity.X * dustSpeed, velocity.Y * dustSpeed).RotatedBy(1.57f * Main.rand.Next(new[] { -1, 0, 1 }));
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<OnyxBar>(22)
            .AddIngredient<OnyxCore>(2)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}