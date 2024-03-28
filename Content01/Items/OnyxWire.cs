using Shade.Content01.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content01.Items;

public class OnyxWire : ModItem
{

    public override void SetDefaults()
    {
        Item.DefaultToWhip(ModContent.ProjectileType<OnyxWireWhip>(), 120, 5, 5);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient(ItemID.Wire)
        .AddIngredient(ModContent.ItemType<Blocks.OnyxBar>(), 5)
        .AddIngredient(ModContent.ItemType<OnyxFragment>(), 3)
        .AddIngredient(ModContent.ItemType<OnyxCore>(), 1)
        .AddTile(TileID.MythrilAnvil).
    Register();
    }
    public override bool MeleePrefix()
    {
        return true;
    }
}