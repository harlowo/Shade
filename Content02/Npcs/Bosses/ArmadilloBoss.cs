using Microsoft.Xna.Framework;
using Shade.Content02.Projectiles;
using Shade.System;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Npcs.Bosses
{
    [AutoloadBossHead]
    public class ArmadilloBoss : ModNPC
    {
        public int phase = 1;
        public float xSpeed;
        public float xAccel = 20f;
        Vector2 direction;
        public Terraria.Player target;
        public int state = 1;
        public float rollCD = 0;
        public float sandfallCD = 0;
        Vector2 rollDir;
        public Vector2 prevPlayerVelocity;
        public float sandfallTimer = 60f;
        public Vector2 sandfallMarker = new(Main.screenPosition.X, Main.screenPosition.Y);
        public float sandfallTime = 539f;
        public bool jump = false;
        public float jumpCD = 60;
        public float lerpCofficient = 0.1f;
        public float jumpTimer = 20f;
        public Vector2 targetCoordinate = new(0, 0);
        public Vector2 targetDir = new(0, 0);
        public bool prevCollision = false;
        public int exitAmt = 0;
        public float sandAngle = 0;
        public float digCD = 240;
        public float maxHealth = 0;
        public float rageCoff = 1;
        public float slamX;
        public float slamY;
        public Vector2 slamDir;
        public float slamRadians;
        public bool slamming = false;
        public float attackCD = 120;
        public float sandSpeed = 5;
        public bool babySpawned = false;
        public override void SetStaticDefaults()
        {
            //Main.npcFrameCount[Type] = frames;
        }
        public override void SetDefaults()
        {
            NPC.width = 154;
            NPC.height = 142;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 3600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = false;
            if (maxHealth == 0)
            {
                maxHealth = NPC.realLife;
            }
        }
        public override bool? CanFallThroughPlatforms()
        {
            if (NPC.Bottom.Y < target.Top.Y)
            {
                return true;
            }
            return false;
        }
        public override void AI()
        {
            //Debugging

            //Timers

            if (attackCD > 0)
            {
                attackCD -= rageCoff;
            }
            else attackCD = 0;

            if (attackCD == 0)
            {
                if (rollCD > 0)
                {
                    rollCD -= rageCoff * phase;
                }
                else rollCD = 0f;

                if (sandfallCD > 0)
                {
                    sandfallCD -= 1;
                }
                else sandfallCD = 0f;

                if (jumpCD > 0)
                {
                    jumpCD -= rageCoff;
                }
                else jumpCD = 0;

                if (jumpTimer > 0)
                {
                    jumpTimer -= rageCoff;
                }
                else jumpTimer = 0;

                if (digCD > 0)
                {
                    digCD -= 1;
                }
                else digCD = 0f;
            }

            //Player Detection

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Terraria.Player player = Main.player[NPC.target];
            target = player;
            if (!player.dead && player.ZoneDesert)
            {
                direction = player.Center - NPC.Center;
            }
            else
            {
                NPC.rotation = 0;
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                state = 1;
                NPC.EncourageDespawn(10);
            }
            direction.Normalize();

            // Attack Choice

            if (!player.dead && player.ZoneDesert && state == 1 && attackCD == 0)
            {
                // Jump

                if (((NPC.Top.Y > player.Bottom.Y && jumpCD - (jumpCD / 2) <= 0 && Math.Abs(NPC.Center.X - player.Center.X) > 150) || (jumpCD == 0)) && NPC.collideY == true)
                {
                    slamX = 0;
                    slamY = 0;
                    NPC.velocity.X = 0;
                    xSpeed = 0;
                    NPC.velocity.Y = -20;
                    state = 5;
                }

                // Sandfall

                if (NPC.collideY && sandfallCD == 0)
                {
                    lerpCofficient = 0.5f;
                    xSpeed = 0;
                    state = 3;
                }

                // Dig

                if (digCD == 0)
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 10f;
                    state = 4;
                }

                // Roll

                if (rollCD == 0 && !(player.Bottom.Y < NPC.Top.Y) && NPC.collideY)
                {
                    rollDir = direction;
                    rollDir.Normalize();
                    state = 2;
                    NPC.velocity.Y = -7;
                    NPC.velocity.X = 1 * Math.Sign(rollDir.X);
                }
            }
            else if (state != 1)
            {
                attackCD = 60;
            }

            // Movement

            if (state == 1)
            {
                lerpCofficient = 0.1f;
                if (Math.Abs(xSpeed) < 7 || Math.Sign(xSpeed) != Math.Sign(direction.X))
                {
                    xSpeed += xAccel * Math.Sign(direction.X);
                }
                if (player.dead || !player.ZoneDesert)
                {
                    xAccel = 1f;
                }
                else xAccel = 0.5f;
                if (Collision.SolidCollision(NPC.TopLeft + new Vector2(154 * direction.X, 0), 154, 140) && NPC.collideY)
                {
                    NPC.velocity.Y = -10f;
                }
            }
            else if (state == 2)
            {
                if (phase == 1)
                {
                    lerpCofficient = 0.01f;
                }
                else
                {
                    lerpCofficient = 0.01f / 1.5f;
                }
                xSpeed = 40f * Math.Sign(rollDir.X) * phase;


                if (NPC.velocity.X == 0 || Math.Sign(rollDir.X) != Math.Sign(direction.X))
                {
                    if (NPC.velocity.X == 0)
                    {
                        xSpeed = 0;
                        NPC.velocity.X = -5 * rollDir.X;
                        NPC.velocity.Y = -10f;
                    }
                    rollCD = 240f;
                    xSpeed /= 2;
                    attackCD = 240f;
                    state = 1;
                }
            }
            else if (state == 3)
            {
                if (player.active == true)
                {
                    //Sandstorm

                    Main.windSpeedTarget = 0.6f;
                    player.AddBuff(BuffID.WindPushed, -1);
                    Sandstorm.StartSandstorm();


                    if (sandfallTimer <= 0)
                    {
                        sandSpeed *= -1;
                        sandfallMarker = new Vector2(Main.screenPosition.X - (Main.screenWidth / 18f * (10 * (phase - 1))), Main.screenPosition.Y);

                        for (int i = -1; i < 20 + 20 * (phase - 1); i++)
                        {
                            if (phase == 1)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), sandfallMarker, new Vector2(0, 0), ModContent.ProjectileType<FallingSand>(), 15, 2);
                            }
                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), sandfallMarker, new Vector2(sandSpeed, 0), ModContent.ProjectileType<FallingSand>(), 15, 2);
                            }
                            if (phase == 1)
                            {
                                sandfallMarker.X += Main.screenWidth / 18f;
                            }
                            else
                            {
                                if (babySpawned) sandfallMarker.X += Main.screenWidth / 14f;
                                else sandfallMarker.X += Main.screenWidth / 18f;
                            }
                            sandfallTimer = 60;
                        }
                    }
                    else
                    {
                        sandfallTimer -= 1;
                    }
                    sandfallTime--;
                    if (sandfallTime <= 0)
                    {
                        Main.windSpeedTarget = 0;
                        Sandstorm.StopSandstorm();
                        sandfallTimer = 0;
                        sandfallCD = 240;
                        sandfallTime = 539;
                        state = 1;
                    }
                }
            }
            else if (state == 4)
            {
                xSpeed = 0;
                NPC.width = 120;
                NPC.height = 120;
                NPC.noTileCollide = true;
                targetDir.X = (player.Center.X - NPC.Center.X);
                if (!Collision.SolidCollision(NPC.position, 77, 71) && !(NPC.velocity.Y > -2))
                {
                    targetDir.Y += 50f;
                }
                else targetDir.Y = (player.Center.Y - NPC.Center.Y);
                targetDir = targetDir.SafeNormalize(Vector2.UnitX);
                NPC.rotation = Func.lerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                NPC.velocity.X = Func.lerp(NPC.velocity.X, 50f * phase * targetDir.X, 0.005f);
                NPC.noGravity = true;
                if (!Collision.SolidCollision(NPC.position, 77, 71))
                {
                    NPC.velocity.Y += 0.40f;
                    if (prevCollision == true && NPC.velocity.Y < -2f)
                    {
                        sandAngle = NPC.rotation;
                        for (int i = 0; i < 4; i++)
                        {
                            sandAngle += 28;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, sandAngle.ToRotationVector2() * 10f, ModContent.ProjectileType<FallingSand>(), 15, 2);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (360 - sandAngle).ToRotationVector2() * 10f, ModContent.ProjectileType<FallingSand>(), 15, 2);
                        }
                        exitAmt++;
                        prevCollision = false;
                    }
                    if (exitAmt >= 3 && Math.Round(NPC.velocity.Y) == 0)
                    {
                        exitAmt = 0;
                        prevCollision = false;
                        NPC.rotation = 0;
                        NPC.width = 154;
                        NPC.height = 142;
                        NPC.noGravity = false;
                        NPC.noTileCollide = false;
                        digCD = 240;
                        state = 1;
                    }
                }
                else
                {
                    NPC.velocity.Y += 0.125f * phase;
                    prevCollision = true;
                    NPC.velocity.Y = Func.lerp(NPC.velocity.Y, 50f * phase * targetDir.Y, 0.005f);
                }
            }
            else if (state == 5)
            {
                xSpeed = 0;
                if (NPC.velocity.Y > -8 || slamming == true)
                {
                    NPC.noGravity = true;

                    if (slamming == false)
                    {
                        slamX = player.position.X;
                        slamY = player.position.Y;
                        if (slamY < NPC.position.Y)
                        {
                            slamY += 100;
                        }
                        if (NPC.Center.Y > slamY)
                        {
                            slamY -= 320;
                        }
                        NPC.velocity = NPC.Center.DirectionTo(new Vector2(slamX, slamY)) * 20f;
                    }
                    slamming = true;
                    if (NPC.velocity.Y == 0)
                    {
                        for (int i = 0; i < 5 * phase; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(-20 * (i + 1), 0), new Vector2(-1f * i, -10f), ModContent.ProjectileType<FallingSand>(), 15, 2);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(20 * (i + 1), 0), new Vector2(1f * i, -10f), ModContent.ProjectileType<FallingSand>(), 15, 2);
                        }
                        NPC.velocity = new Vector2(0, 0);
                        slamming = false;
                        jumpCD = 120;
                        NPC.noGravity = false;
                        state = 1;
                    }
                }
                else
                {
                    NPC.noGravity = false;
                }
            }

            if (state != 4 && state != 5) NPC.velocity.X = Func.lerp(NPC.velocity.X, xSpeed, lerpCofficient);

            // Other

            rageCoff = 1 / (NPC.GetLifePercent() * 3);
            if (rageCoff < 1)
            {
                rageCoff = 1;
            }

            if (NPC.GetLifePercent() < 0.5 && Main.expertMode)
            {
                phase = 2;
            }
        }
        public override void OnKill()
        {
            Main.windSpeedTarget = 0;
            Sandstorm.StopSandstorm();
        }
    }
}
