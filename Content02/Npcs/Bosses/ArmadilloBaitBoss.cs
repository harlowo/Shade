using Microsoft.Xna.Framework;
using Shade.System;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Npcs.Bosses
{
    public class ArmadilloBaitBoss : ModNPC
    {
        public int animType = 0;
        public float xSpeed = 0f;
        public float xAccel = 0.2f;
        public double frameY = 0;
        public int animPause = 0;
        Vector2 direction;
        public Terraria.Player target;
        public float jumpCD = 1f;
        public float jumpTimer;
        public bool jump;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 17;
        }
        public override void SetDefaults()
        {
            NPC.width = 102;
            NPC.height = 62;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.lifeMax = 800;
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
        }
        public override string Texture => "Shade/Content02/Npcs/Bosses/ArmadilloBaitBossSheet";
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 102;
            NPC.spriteDirection = Math.Sign(direction.X);
            NPC.frame.Y = (int)Math.Round(frameY) * frameHeight;
            if (animType == 1)
            {
                NPC.frame.Y += 9 * frameHeight;
            }
            frameY += 0.25;
            if ((frameY > 8.5 && animType == 0))
            {
                frameY = 0;
            }
            if (animType == 1 && frameY > 6.5)
            {
                frameY = 6;
            }
        }
        public override bool? CanFallThroughPlatforms()
        {
            if (NPC.Bottom.Y < target.Top.Y)
            {
                if (NPC.velocity.Y > 0)
                {
                    animType = 1;
                }
                return true;
            }
            return false;
        }
        public override void AI()
        {
            // Timers

            if (jumpCD > 0)
            {
                jumpCD -= 0.016f;
            }
            else jumpCD = 0;

            if (jumpTimer > 0)
            {
                animType = 1;
                frameY = 0;
                NPC.velocity.X = 0;
                jumpTimer -= 0.016f;
            }
            else jumpTimer = 0;

            // Player Detection

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Terraria.Player player = Main.player[NPC.target];
            target = player;
            if (!player.dead)
            {
                direction = player.Center - NPC.Center;
            }
            else
            {
                NPC.EncourageDespawn(10);
            }
            direction.Normalize();

            //Horisontal movement code

            NPC.velocity.X = Func.lerp(NPC.velocity.X, xSpeed, 0.1f);
            xSpeed += xAccel * direction.X;
            if (Math.Abs(xSpeed) > 15f)
            {
                xSpeed = 15f * Math.Sign(xSpeed);
            }

            //Jump code

            if (NPC.collideY)
            {
                animType = 0;
                if (NPC.Top.Y > player.Bottom.Y && NPC.collideY == true && jumpCD == 0 && jump == false)
                {
                    jumpTimer = 0.25f;
                    jump = true;
                }
                if (jump == true && jumpTimer == 0)
                {
                    xSpeed = Math.Abs(xSpeed) * direction.X;
                    NPC.velocity.Y = -10f;
                    jumpCD = 1;
                    jump = false;
                    animType = 1;
                }
            }
        }
    }
}
