using Shade.OnyxExcavator.NPCs;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Shade.Content01.Projectiles.OnyxExcavator;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Shade.Content01.Items;
using Shade.Content01.Items.LootBoxes;
using Shade.Blocks;

namespace Shade.Content01.Npcs.OnyxExcavator
{
    [AutoloadBossHead]
    internal class OnyxExcavatorHead : WormHead
	{
        public override int BodyType => ModContent.NPCType<OnyxExcavatorBody>();

		public override int TailType => ModContent.NPCType<OnyxExcavatorTail>();

        public bool Windup = false;

        public bool Drilling = false;

        public bool Inactive = true;

        public float AnimationSpeed = 0.0f;

        public float MoveSpeed = 0.0f;

        public int MovementTimer = 0;

        public int SoundTimer = 0;

        public bool Despawning = false;

        public int StationaryAttackType;

        public int LastAttack = 0;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				CustomTexturePath = "Shade/Content01/Npcs/OnyxExcavator/OnyxExcavator_Bestiary",
				Position = new Vector2(40f, 24f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 12f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
		}

		public override void SetDefaults() 
		{
			NPC.CloneDefaults(NPCID.DiggerHead);
            Main.npcFrameCount[Type] = 4;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
			NPC.noGravity = true;
			NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
			NPC.lifeMax = 60000;
            NPC.boss = true;
            NPC.width = 150;
            NPC.height = 154;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "C"); ;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) { Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), Mod.Find<ModGore>("OnyxExcavatorGore1").Type, 2); }
        }

        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 3;

            int frameSpeed = 1;
            NPC.frameCounter += AnimationSpeed;
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				new FlavorTextBestiaryInfoElement("A drill-like machine created by an unseen mastermind to destroy the foundations of the world.")
			});
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ExcavatorLootBox>()));


            var notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OnyxBar>(), minimumDropped: 15, maximumDropped: 30));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OnyxOre>(), minimumDropped: 35, maximumDropped: 50));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OnyxCore>(), minimumDropped: 15, maximumDropped: 30));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OnyxFragment>(), minimumDropped: 15, maximumDropped: 30));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OnyxFragment>()));

            notExpertRule.OnSuccess(
                ItemDropRule.OneFromOptions(
                    1,
                    ModContent.ItemType<OnyxEnigmaStaff>(),
                    ModContent.ItemType<OnyxRailgun>(),
                    ModContent.ItemType<OnyxFang>()
                )
            );
            notExpertRule.OnSuccess(
                ItemDropRule.OneFromOptions(
                    20,
                    ModContent.ItemType<OnyxHelmet>(),
                    ModContent.ItemType<OnyxChestPlate>(),
                    ModContent.ItemType<OnyxLeggings>()
                ),
                true
            );

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedOnyxBoss, -1);
        }

        public override bool CheckActive()
        {
            if (Main.player[NPC.target].dead) { return true; }
            else { return false; }  
        }

        public override void Init() {
			MinSegmentLength = 25;
			MaxSegmentLength = 25;

			CommonWormInit(this);
		}

		internal static void CommonWormInit(Worm worm) {
			worm.MoveSpeed = 5.5f;
			worm.Acceleration = 0.045f;
		}

		private int attackCounter;
		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(attackCounter);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			attackCounter = reader.ReadInt32();
		}

		public override void AI() 
		{
            if (NPC.position.X < Main.screenPosition.X + 960 - 2000 || NPC.position.X > Main.screenPosition.X + 960 + 2000 || NPC.position.Y < Main.screenPosition.Y + 560 - 2000 || NPC.position.Y > Main.screenPosition.Y + 560 + 2000)
            {
                NPC.position.Y += 999999;
                NPC.life = -1; 
            }
            if (Inactive == false) 
            { 
                NPC.rotation = NPC.velocity.ToRotation() - 17.3f; 
            }
            if (Despawning == false) 
            {
                if (Main.player[NPC.target].dead) { Despawning = true; }
                else if (!Main.player[NPC.target].ZoneDirtLayerHeight & !Main.player[NPC.target].ZoneRockLayerHeight & !Main.player[NPC.target].ZoneUnderworldHeight)
                { Despawning = true; }

                if (Windup == true)
                {
                    if (AnimationSpeed < 1.0f)
                    {
                        AnimationSpeed += 0.01f;
                        if (Main.expertMode) { MoveSpeed += 0.05f; }
                        else { MoveSpeed += 0.03f; }
                    }
                    else
                    {
                        MovementTimer = 500;
                        Windup = false;
                        Drilling = true;
                    }
                }

                if (Inactive == false) {
                    if (NPC.position.X > Main.player[NPC.target].position.X + 40 || NPC.position.X < Main.player[NPC.target].position.X - 40 || NPC.position.Y > Main.player[NPC.target].position.Y + 40 || NPC.position.Y < Main.player[NPC.target].position.Y - 40)
                    {
                        NPC.velocity = (Main.player[NPC.target].Center - NPC.position).SafeNormalize(Vector2.Zero) * MoveSpeed;
                    }
                    else { NPC.velocity *= 0.98f; }
                }
                else { NPC.velocity *= 0.98f; }

                if (Drilling == true)
                {
                    if (MovementTimer > 0) 
                    {
                        if (SoundTimer > 0) { SoundTimer -= 1; }
                        else
                        {
                            SoundStyle DrillStartUp = new SoundStyle("Shade/Content01/Assets/Sounds/OnyxExcavatorDrill");
                            SoundEngine.PlaySound(DrillStartUp, NPC.Center);
                            SoundTimer = 25;
                        }
                        MovementTimer -= 1;
                    }
                    else 
                    {
                        SoundStyle DrillWindown = new SoundStyle("Shade/Content01/Assets/Sounds/OnyxExcavatorDrillWindown");
                        SoundEngine.PlaySound(DrillWindown, NPC.Center);
                        MovementTimer = 650;
                        SoundTimer = 0;
                        StationaryAttackType = Main.rand.Next(1, 4);
                        if (StationaryAttackType == 1 & LastAttack == 1) { StationaryAttackType = 2; }
                        else if (StationaryAttackType == 2 & LastAttack == 2) { StationaryAttackType = 3; }
                        if (StationaryAttackType == 3 & LastAttack == 3) { StationaryAttackType = 1; }
                        LastAttack = StationaryAttackType;
                        Drilling = false;
                        Inactive = true;
                    }
                }

                if (Inactive == true)
                {
                    if (AnimationSpeed > 0)
                    {
                        AnimationSpeed -= 0.01f;
                        MoveSpeed -= 0.05f;
                    }
                    else if (MovementTimer > 0)
                    {
                        if (StationaryAttackType == 1) 
                        {
                            if (MovementTimer == 550)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OnyxExcavatorLightningSignal>(), NPC.whoAmI);
                                SoundStyle ChargeLightning = new SoundStyle("Shade/Content01/Assets/Sounds/OnyxExcavatorChargeLightning");
                                SoundEngine.PlaySound(ChargeLightning, NPC.Center);
                            }
                            if (MovementTimer == 390) 
                            {
                                SoundStyle EmitLightning = new SoundStyle("Shade/Content01/Assets/Sounds/OnyxExcavatorEmitLightning");
                                SoundEngine.PlaySound(EmitLightning, NPC.Center);
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)Main.player[NPC.target].Center.X, (int)Main.player[NPC.target].Center.Y, ModContent.NPCType<OnyxExcavatorFlash>(), NPC.whoAmI);
                            }
                        }

                        if (StationaryAttackType == 2)
                        {
                            if (MovementTimer == 550)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OnyxExcavatorMissileSignal>(), NPC.whoAmI);
                            }
                        }

                        if (StationaryAttackType == 3)
                        {
                            if (MovementTimer == 550)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OnyxExcavatorDroidSignal>(), NPC.whoAmI);
                            }
                        }

                        NPC.velocity *= 0;
                        MovementTimer -= 1;
                    }
                    else 
                    {
                        SoundStyle DrillStartUp = new SoundStyle("Shade/Content01/Assets/Sounds/OnyxExcavatorDrillWindup");
                        SoundEngine.PlaySound(DrillStartUp, NPC.Center);
                        Inactive = false;
                        Windup = true;
                    }
                }
            }
            else
            {
                if (SoundTimer > 0) { SoundTimer -= 1; }
                else
                {
                    SoundStyle DrillStartUp = new SoundStyle("Shade/Content01/Assets/Sounds/OnyxExcavatorDrill");
                    SoundEngine.PlaySound(DrillStartUp, NPC.Center);
                    SoundTimer = 25;
                }
                if (AnimationSpeed < 1.0f)
                {
                    AnimationSpeed += 0.01f;
                }
                Vector2 Direction = NPC.position;
                Direction.Y = -1000;
                MoveSpeed += 0.03f;
                NPC.velocity = (NPC.position - Direction).SafeNormalize(Vector2.Zero) * MoveSpeed;
                Inactive = false;
                NPC.EncourageDespawn(250);
            }
        }
	}

	internal class OnyxExcavatorBody : WormBody
	{
        public int StationaryAttackType;

        public int AttackTimer = 0;

        public float ProjectileDelay = 0;
        public override void SetStaticDefaults() {
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerBody);
			NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.width = 150;
            NPC.height = 120;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) { Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), Mod.Find<ModGore>("OnyxExcavatorGore2").Type, 2); }
        }

        public override bool CheckActive()
        {
			return false;
        }

        public override void Init() {
            OnyxExcavatorHead.CommonWormInit(this);
		}

        public override void AI()
        {
            if (StationaryAttackType == 0) 
            {
                if (NPC.AnyNPCs(ModContent.NPCType<OnyxExcavatorLightningSignal>()))
                {
                    StationaryAttackType = 1;
                    AttackTimer = 550;
                }
                else if (NPC.AnyNPCs(ModContent.NPCType<OnyxExcavatorMissileSignal>()))
                {
                    StationaryAttackType = 2;
                    AttackTimer = Main.rand.Next(1, 550);
                }
                else if (NPC.AnyNPCs(ModContent.NPCType<OnyxExcavatorDroidSignal>()))
                {
                    StationaryAttackType = 3;
                    AttackTimer = Main.rand.Next(1, 550);
                }
            }
            else if (StationaryAttackType == 1)
            {
                if (AttackTimer > 0) 
                {
                    if (AttackTimer < 550 & AttackTimer > 390)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 DustOffset;
                            DustOffset.X = NPC.Center.X + NPC.width / -2;
                            DustOffset.Y = NPC.Center.Y + NPC.height / -2;

                            var velocity = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                            var dust = Dust.NewDustPerfect(DustOffset, DustID.PurpleTorch, velocity, 100, Color.White, Main.rand.NextFloat(1.0f, 2.5f));

                            dust.noLight = false;
                            dust.noGravity = true;
                            dust.fadeIn = Main.rand.NextFloat(0.3f, 0.8f);
                        }
                    }
                    if (AttackTimer == 390)
                    {
                        SoundStyle EmitLightning = new SoundStyle("Shade/Content01/Assets/Sounds/OnyxExcavatorEmitLightning");
                        SoundEngine.PlaySound(EmitLightning, NPC.Center);
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)Main.player[NPC.target].Center.X, (int)Main.player[NPC.target].Center.Y, ModContent.NPCType<OnyxExcavatorFlash>(), NPC.whoAmI);
                    }
                    if (AttackTimer < 390 & AttackTimer > 50)
                    {
                        if (ProjectileDelay > 0) { ProjectileDelay -= 1; }
                        else 
                        {
                            Vector2 ProjectileOffset;
                            ProjectileOffset.X = NPC.Center.X + NPC.width / -2;
                            ProjectileOffset.Y = NPC.Center.Y + NPC.height / -2;

                            Player target = Main.player[NPC.target];
                            Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                            var velocity = direction.RotatedBy(MathHelper.ToRadians(15f * Main.rand.NextFloat(1f, 200f)));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), ProjectileOffset, velocity * 10.0f, ModContent.ProjectileType<OnyxExcavatorLightningBolt>(), 30, 0f, Main.myPlayer);
                            ProjectileDelay = Main.rand.NextFloat(10.0f, 15.0f); ;
                        }
                    }

                    AttackTimer -= 1;
                }
                else { StationaryAttackType = 0; }
            }
            else if (StationaryAttackType == 2)
            {
                if (AttackTimer > 0) { AttackTimer -= 1; }
                else 
                {
                    SoundEngine.PlaySound(SoundID.Item61, NPC.Center);

                    Vector2 ProjectileOffset;
                    ProjectileOffset.X = NPC.Center.X + NPC.width / -2;
                    ProjectileOffset.Y = NPC.Center.Y + NPC.height / -2;

                    Player target = Main.player[NPC.target];
                    Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    var velocity = direction.RotatedBy(MathHelper.ToRadians(15f * Main.rand.NextFloat(1f, 200f)));
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), ProjectileOffset, velocity * 5.0f, ModContent.ProjectileType<OnyxExcavatorMissile>(), 34, 0f, Main.myPlayer);
                    StationaryAttackType = 0;
                }
            }
            else if (StationaryAttackType == 3)
            {
                if (AttackTimer > 0) { AttackTimer -= 1; }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item61, NPC.Center);

                    Vector2 ProjectileOffset;
                    ProjectileOffset.X = NPC.Center.X + NPC.width / -2;
                    ProjectileOffset.Y = NPC.Center.Y + NPC.height / -2;

                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OnyxDroid>(), NPC.whoAmI);
                    StationaryAttackType = 0;
                }
            }
        }
    }

	internal class OnyxExcavatorTail : WormTail
	{
		public override void SetStaticDefaults() {
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerTail);
			NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.width = 150;
            NPC.height = 120;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) { Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), Mod.Find<ModGore>("OnyxExcavatorGore3").Type, 2); }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void Init() {
            OnyxExcavatorHead.CommonWormInit(this);
		}
	}
}
