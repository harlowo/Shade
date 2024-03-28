using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Shade.Content01.Npcs.OnyxExcavator
{
    public class OnyxExcavatorDroidSignal : ModNPC
    {
        public int damage;

        public int defense;

        public int defDamage;

        public int defDefense;

        public float[] ai = new float[NPC.maxAI];

        public string TypeName;

        public float LifeTime = 1;

        public override void SetStaticDefaults()
        {

            NPCID.Sets.ImmuneToAllBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 0;
            NPC.height = 0;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 999999999;
            NPC.npcSlots = 0f;
            NPC.HitSound = null;
            NPC.DeathSound = null;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0f;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.knockBackResist = 0f;

        }

        public override bool CheckActive()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        Color newColor;
        public override void AI()
        {
            if (LifeTime > 0) { LifeTime -= 1; }
            else { NPC.life = -1; }
        }
    }
}
