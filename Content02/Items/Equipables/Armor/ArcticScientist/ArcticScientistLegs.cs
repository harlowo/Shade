using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using MonoMod;
using Terraria.Localization;

namespace Shade.Content02.Items.Equipables.Armor.ArcticScientist
{
    [AutoloadEquip(EquipType.Legs)]
    public class ArcticScientistLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.OverridesLegs[Item.legSlot] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 6;
        }
        public override void AddRecipes()
        {
        }
    }
}
