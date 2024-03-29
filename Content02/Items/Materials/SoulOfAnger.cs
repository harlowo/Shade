using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Materials
{
    public class SoulOfAnger : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Registers a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

            ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity

            Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 1000; // Makes the item worth 1 gold.
            Item.rare = ItemRarityID.Orange;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.DarkRed.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }
    }
    public class SoulOfAngerDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(Terraria.NPC npc, NPCLoot npcLoot)
        {
            if (npc.boss || npc.rarity > 0 && npc.type != NPCID.Gnome)
            {
                if (!npc.boss)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulOfAnger>(), 1, 6*npc.rarity, 6*npc.rarity));
                }
                else
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulOfAnger>(), 2, 22, 36));
                }
            }
        }
    }
}