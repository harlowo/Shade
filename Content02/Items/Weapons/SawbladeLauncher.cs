using Shade.Content02.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Weapons
{
	public class SawbladeLauncher : ModItem
	{

		public override void SetDefaults()
		{
			Item.damage = 54;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 70;
			Item.height = 36;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item108;
			Item.autoReuse = true;
			Item.shootSpeed = 8;
			Item.shoot = ModContent.ProjectileType<Sawblade>();
		}
        public override bool AltFunctionUse(Player player)
        {
			return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse == 2)
				type = ModContent.ProjectileType<Magnet>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.altFunctionUse == 2)
			{
				if (player.ownedProjectileCounts[ModContent.ProjectileType<Magnet>()] < 3)
					return true;
				else
					return false;
			}
			return true;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.NailGun);
            recipe.AddIngredient(ItemID.HallowedRepeater);
            recipe.AddIngredient(ItemID.Cog, 12);
            recipe.AddIngredient(ItemID.IronBar, 2);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}