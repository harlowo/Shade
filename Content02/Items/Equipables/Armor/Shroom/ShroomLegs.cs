using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Shade.System;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Shroom
{
    [AutoloadEquip(EquipType.Legs)]
    public class ShroomLegs : ModItem
    {
        public static Lazy<Asset<Texture2D>> glowmask;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.OverridesLegs[Item.legSlot] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            if (!Main.dedServ)
            {
                glowmask = new(() => ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Shroom/ShroomLegs_Glowmask"));

                DrawLayers.LegsLayer.RegisterData(Item.legSlot, new DrawLayers.DrawLayerData()
                {
                    Texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/Shroom/ShroomLegs_Legs_Glowmask"),
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
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.BasicInWorldGlowmask(spriteBatch, glowmask.Value.Value, Color.White * 0.5f, rotation, scale);
        }
    }
}
