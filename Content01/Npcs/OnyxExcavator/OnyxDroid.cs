using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System;
using Terraria.Enums;
using Terraria.Utilities;
using Shade.Content01.Projectiles.OnyxExcavator;
using Shade.Content01.Items;
using Terraria.GameContent.ItemDropRules;

namespace Shade.Content01.Npcs.OnyxExcavator
{
    public class OnyxDroid : ModNPC
    {
        public int damage;

        public int defense;

        public int defDamage;

        public int defDefense;

        public bool netAlways;

        public bool despawnEncouraged;

        public float[] ai = new float[NPC.maxAI];

        public string TypeName;

        public float FireCooldown = Main.rand.NextFloat(50f, 100f);

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
            NPC.width = 25;
            NPC.height = 21;
            NPC.aiStyle = 14;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0.0f;
            NPC.value = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.npcSlots = 1;
            NPC.scale = 1.0f;

            AIType = NPCID.Probe;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OnyxCore>()));
        }

        public override void OnKill()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), Mod.Find<ModGore>("OnyxDroidGore1").Type, 1);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), Mod.Find<ModGore>("OnyxDroidGore2").Type, 1);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), Mod.Find<ModGore>("OnyxDroidGore3").Type, 1);
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 1.75f, 0.0f, 2.25f);

            NPC.rotation = NPC.AngleTo(Main.player[NPC.target].Center) + 17.3f;

            if (NPC.Center.X < Main.player[NPC.target].Center.X)
            {
                if (NPC.velocity.X < 10.0f) { NPC.velocity.X += 0.05f; }
            }
            else if (NPC.Center.X > Main.player[NPC.target].Center.X)
            {
                if (NPC.velocity.X > -10.0f) { NPC.velocity.X -= 0.05f; }
            }
            if (NPC.Center.Y > Main.player[NPC.target].Center.Y)
            {
                if (NPC.velocity.Y > -10.0f) { NPC.velocity.Y -= 0.05f; }
            }
            else if (NPC.Center.Y < Main.player[NPC.target].Center.Y)
            {
                if (NPC.velocity.Y < 10.0f) { NPC.velocity.Y += 0.05f; }
            }

            if (FireCooldown > 0) { FireCooldown -= 1; }
            else 
            {
                SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                Player target = Main.player[NPC.target];
                Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 25, ModContent.ProjectileType<OnyxExcavatorLightningBolt>(), 40, 0f, Main.myPlayer);
                FireCooldown = Main.rand.NextFloat(75f, 150f); 
            }
        }
    }
}
