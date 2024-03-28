using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Text;
using Terraria.Localization;

namespace Shade.Content01.Npcs.OnyxExcavator
{
	public class OnyxExcavatorFlash : ModNPC
	{
		public int damage;

		public int defense;

		public int defDamage;

		public int defDefense;

		public float[] ai = new float[NPC.maxAI];

		public string TypeName;

		public float Visibility = 1;

		public bool MessageSent = false;

		public override void SetStaticDefaults() {
			NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.ImmuneToAllBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults() {
			NPC.width = 200;
			NPC.height = 200;
			NPC.aiStyle = 0;
			NPC.damage = 0;
            NPC.defense = 999999999;
            NPC.lifeMax = 999999999;
            NPC.npcSlots = 10f;
			NPC.HitSound = null;
			NPC.DeathSound = null;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = 0f;
			NPC.knockBackResist = 0f;
			NPC.dontTakeDamage = true;
			NPC.scale = 5;
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source) {
			base.OnSpawn(source);
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Visibility;
        }

        public override void OnKill() {
			base.OnKill();
			NPC.boss = false;
		}

		Color newColor;
		public override void AI() {
            if (Visibility > 0) { Visibility -= 0.1f; }
            else { NPC.life = -1; }
            NPC.position.X = Main.screenPosition.X + 500;
			NPC.position.Y = Main.screenPosition.Y + 1500;
		}
	}
}
