using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Shade.Trophies;

public class OnyxExcavatorTrophy : ModItem
{
    public override void SetDefaults()
    {
        // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
        Item.DefaultToPlaceableTile(ModContent.TileType<OnyxExcavatorTrophyTile>());

        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(0, 1);
    }
}

public class OnyxExcavatorTrophyTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
        DustType = 7;
    }
}