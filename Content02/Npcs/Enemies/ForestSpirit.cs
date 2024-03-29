using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Shade.Content02.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Npcs.Enemies
{
    public class ForestSpirit : ModNPC
    {
        public Terraria.Player target;
        public Vector2 direction;
        public float speed = 2.5f;
        public int attackCD = 60;
        public int BamboozleCD = 60;
        public int teleports;
        public int teleportCD;
        public Vector2 teleportPosition;
        public bool doTeleport = true;
        public int preAttackPause;
        public float dustSpeed;
        public Vector2 dustVelocity;
        public Vector2 dustDir;
        public int state = 1;
        public int framePause = 6;
        public int frame = 0;
        public int flameFrame = 0;
        public int flameFramePause = 6;
        private readonly static Texture2D flame = ModContent.Request<Texture2D>("Shade/Content02/Npcs/Enemies/ForestSpiritFlame", AssetRequestMode.ImmediateLoad).Value;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 17;
        }
        public override void SetDefaults()
        {
            NPC.width = 74;
            NPC.height = 62;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
        }
        public override string Texture => "Shade/Content02/Npcs/Enemies/ForestSpiritSheet";
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (flameFramePause > 0)
            {
                flameFramePause--;
            }
            else
            {
                flameFrame++;
                flameFramePause = 6;
            }
            if (flameFrame >= 8)
            {
                flameFrame = 0;
            }
            spriteBatch.Draw(flame, NPC.position - Main.screenPosition, new Rectangle(0, 64 * flameFrame, 74, 62), drawColor);
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (target != null)
            {
                NPC.spriteDirection = -Math.Sign(target.position.X - NPC.position.X);
            }
            if (framePause > 0)
            {
                framePause--;
            }
            else
            {
                framePause = 6;
                frame++;
                if (frame >= 8 && state == 1)
                {
                    frame = 0;
                }
                else if (frame < 8 && state == 2)
                {
                    frame = 8;
                }
                NPC.frame.Y = frameHeight * frame;
            }
        }
        public override void AI()
        {
            // Timers

            if (attackCD > 0)
            {
                attackCD--;
            }
            else
            {
                attackCD = 0;

                if (BamboozleCD > 0)
                {
                    BamboozleCD--;
                }
                else BamboozleCD = 0;
            }

            // Attack Selection

            if (BamboozleCD == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                state = 2;
            }

            // Player Detection

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Terraria.Player player = Main.player[NPC.target];
            target = player;
            direction = player.Center - NPC.Center;
            direction.Normalize();

            // AI

            if (state == 1)
            {
                NPC.velocity = speed * direction;
            }
            else if (state == 2)
            {
                if (frame >= 16)
                {
                    framePause = 6;
                    frame = 16;
                    if (teleportCD > 0)
                    {
                        teleportCD--;
                    }
                    else
                    {
                        if (doTeleport == true)
                        {
                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            preAttackPause = 20;
                            NPC.position = teleportPosition;
                            doTeleport = false;
                        }
                        else if (preAttackPause > 0)
                        {
                            preAttackPause--;
                        }
                        else if (preAttackPause <= 0)
                        {
                            teleports++;
                            teleportCD = 10;
                            direction = player.Center - NPC.Center;
                            direction.Normalize();
                            doTeleport = true;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 2, ModContent.ProjectileType<ForestSpiritProj>(), 5, 1);
                            if (teleports < 6)
                            {
                                teleportPosition = player.Center + Main.rand.Next(100, 600) * MathHelper.ToRadians(Main.rand.Next(0, 360)).ToRotationVector2();
                                for (int i = 0; i < 30; i++)
                                {
                                    dustSpeed = Main.rand.Next(4, 10);
                                    dustDir = (MathHelper.ToRadians(i * 40) + (MathHelper.ToRadians(Main.rand.Next(0, 40)) * Main.rand.Next(-1, 1))).ToRotationVector2();
                                    dustVelocity = dustDir * dustSpeed;

                                    int dust = Dust.NewDust(teleportPosition, 16, 16, DustID.Grass, dustVelocity.X, dustVelocity.Y, 0, Color.White, 1f);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].scale = Main.rand.Next(1, 30) / 10;
                                    Main.dust[dust].rotation = dustSpeed * 2.25f;
                                }
                            }
                        }
                    }
                    if (teleports >= 6)
                    {
                        BamboozleCD = 60;
                        attackCD = 60;
                        teleportCD = 0;
                        teleports = 0;
                        state = 1;
                    }
                }
                else
                {
                    if (frame == 8)
                    {
                        teleportPosition = player.Center + Main.rand.Next(100, 600) * MathHelper.ToRadians(Main.rand.Next(0, 360)).ToRotationVector2();
                    }
                    if (frame == 15 && framePause == 0)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            dustSpeed = Main.rand.Next(4, 10);
                            dustDir = (MathHelper.ToRadians(i * 40) + (MathHelper.ToRadians(Main.rand.Next(0, 40)) * Main.rand.Next(-1, 1))).ToRotationVector2();
                            dustVelocity = dustDir * dustSpeed;

                            int dust = Dust.NewDust(teleportPosition, 16, 16, DustID.Grass, dustVelocity.X, dustVelocity.Y, 0, Color.White, 1f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].scale = Main.rand.Next(1, 30) / 10;
                            Main.dust[dust].rotation = dustSpeed * 2.25f;
                        }
                    }
                }
            }
        }
    }
}
