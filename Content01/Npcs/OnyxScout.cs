using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Shade.NPCs;
using Shade.Content01.Items;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Shade.Content01.Projectiles.OnyxExcavator;

namespace Shade.Content01.Npcs
{
    internal class OnyxScoutHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<OnyxScoutBody>();

        public override int TailType => ModContent.NPCType<OnyxScoutTail>();

        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Shade/Content01/Npcs/OnyxScout_Bestiary",
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {

            NPC.CloneDefaults(NPCID.GiantWormHead);
            NPC.aiStyle = -1;
            NPC.height = 48;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,


				new FlavorTextBestiaryInfoElement("Wormy servants of a group of scientists who dwell in the caves")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.1f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxFragment>()));
        }

        public override void Init()
        {

            MinSegmentLength = 20;
            MaxSegmentLength = 30;

            CommonWormInit(this);
        }


        internal static void CommonWormInit(Worm worm)
        {
            worm.MoveSpeed = 5.5f;
            worm.Acceleration = 0.045f;
        }

        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }

    }

    internal class OnyxScoutBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GiantWormBody);
            NPC.aiStyle = -1;
            NPC.height = 24;
        }

        public override void Init()
        {
            OnyxScoutHead.CommonWormInit(this);
        }
    }

    internal class OnyxScoutTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.GiantWormTail);
            NPC.aiStyle = -1;
            NPC.height = 24;
        }

        public override void Init()
        {
            OnyxScoutHead.CommonWormInit(this);
        }
    }
}
