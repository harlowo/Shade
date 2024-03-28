using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Shade.Content01.Npcs.OnyxExcavator;

namespace Shade.Content01.Items
{
    public class OnyxExcavatorSummon : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 9999;
			Item.value = 0;
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
		}

		public override bool CanUseItem(Player player) {
			if (!NPC.AnyNPCs(ModContent.NPCType<OnyxExcavatorHead>())) { return Main.player[Item.holdStyle].ZoneDirtLayerHeight || Main.player[Item.holdStyle].ZoneRockLayerHeight || Main.player[Item.holdStyle].ZoneUnderworldHeight; }
			else { return false; }
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				if (!Main.getGoodWorld) {
					SoundEngine.PlaySound(SoundID.Roar, player.position);
				}

				int type = ModContent.NPCType<OnyxExcavatorHead>();

				if (Main.netMode != NetmodeID.MultiplayerClient) {
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else {
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddTile(TileID.MythrilAnvil)
                .AddIngredient(ModContent.ItemType<OnyxFragment>(), 3)
				.AddIngredient(ItemID.MeteoriteBar, 10)
                .Register();

			CreateRecipe()
				.AddIngredient(ItemID.SoulofFright, 5)
				.AddTile(TileID.MythrilAnvil)
				.AddIngredient(ModContent.ItemType<OnyxFragment>(), 3)
				.AddIngredient(ItemID.MeteoriteBar, 10)
			.Register();

            CreateRecipe()
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.MythrilAnvil)
                .AddIngredient(ModContent.ItemType<OnyxFragment>(), 3)
                .AddIngredient(ItemID.MeteoriteBar, 10)
            .Register();
        }
    }
}