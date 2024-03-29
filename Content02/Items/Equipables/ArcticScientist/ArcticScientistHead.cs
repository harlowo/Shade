using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using MonoMod;
using Terraria.Localization;
using Shade.System;
using Terraria.GameInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using System.Collections.Generic;
using System;

namespace Shade.Content02.Items.Equipables.Armor.ArcticScientist
{
    [AutoloadEquip(EquipType.Head)]

    public class ArcticScientistHead : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 5;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ArcticScientistBody>() && legs.type == ModContent.ItemType<ArcticScientistLegs>();
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            player.maxMinions += 1;
            player.GetModPlayer<ArcticScientistPlayer>().set = true;
            player.setBonus = this.GetLocalization("SetBonus").WithFormatArgs(Keybinds.SetKeybind.GetAssignedKeys(InputMode.Keyboard)[0], Keybinds.SetKeybind_1.GetAssignedKeys(InputMode.Keyboard)[0], Keybinds.SetKeybind_2.GetAssignedKeys(InputMode.Keyboard)[0], Keybinds.SetKeybind_3.GetAssignedKeys(InputMode.Keyboard)[0]).Value;
        }
        public override void AddRecipes()
        {
        }
    }
    public class ArcticScientistPlayer : ModPlayer
    {
        public bool attackMode = false;
        public bool manaMode = false;
        public bool combinedMode = true;
        public int timeFromHit = 0;
        public int charge = 0;
        public bool set = false;
        public override void ResetEffects()
        {
            set = false;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Keybinds.SetKeybind.JustPressed && set && charge == 300 && Player.ownedProjectileCounts[ModContent.ProjectileType<ScientistProbe>()] < 4)
            {
                charge = 0;
                Player.AddBuff(ModContent.BuffType<ScientistProbeBuff>(), 2);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - (Vector2.UnitY * 20), new(Main.rand.Next(5), Main.rand.Next(5)), ModContent.ProjectileType<ScientistProbe>(), 0, 0, Main.myPlayer);
            }
            if (Keybinds.SetKeybind_1.JustPressed && set)
            {
                attackMode = true;
                manaMode = false;
                combinedMode = false;
            }
            else if (Keybinds.SetKeybind_2.JustPressed && set)
            {
                attackMode = false;
                manaMode = false;
                combinedMode = true;
            }
            else if (Keybinds.SetKeybind_3.JustPressed && set)
            {
                attackMode = false;
                manaMode = true;
                combinedMode = false;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            charge += damageDone;
            timeFromHit = 0;
            if (charge > 300)
            {
                charge = 300;
            }
        }
        public override void PostUpdate()
        {
            if (timeFromHit < 60)
            {
                timeFromHit++;
            }
            else if (charge > 0)
            {
                charge--;
            }
        }
    }
    public class ArcticScientistDrawLayer : PlayerDrawLayer
    {
        float prevCharge = default;
        public Texture2D border = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/DroneResourceBar", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D bar = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/DroneResourceBarProgress", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D bg = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/DroneResourceBarBackground", AssetRequestMode.ImmediateLoad).Value;

        public Texture2D node_0 = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/AttackMode", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D node_1 = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/ManaMode", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D node_2 = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/CombinedMode", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D node_3 = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/DisabledMode", AssetRequestMode.ImmediateLoad).Value;
        public Rectangle nodeSource = new Rectangle(0, 0, 16, 16);

        public ArcticScientistPlayer arcticPlayer;

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.LastVanillaLayer);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            arcticPlayer = drawInfo.drawPlayer.GetModPlayer<ArcticScientistPlayer>();
            if (arcticPlayer.set == true)
            {
                Bar.CreateResourceBar(
                    drawInfo,
                    border,
                    Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 + 40),
                    new Rectangle(0, 0, 76, 36),
                    bar,
                    48,
                    20,
                    new(16, 10),
                    300,
                    arcticPlayer.charge,
                    prevCharge,
                    true,
                    true,
                    bg);
                if (arcticPlayer.attackMode == true)
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                    node_0,
                    new Vector2(Main.screenWidth / 2 - 32, Main.screenHeight / 2 - 30),
                    nodeSource,
                    Color.White,
                    0,
                    nodeSource.Size() / 2f,
                    1,
                    0
                    ));
                }
                else
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                    node_3,
                    new Vector2(Main.screenWidth / 2 - 32, Main.screenHeight / 2 - 30),
                    nodeSource,
                    Color.White,
                    0,
                    nodeSource.Size() / 2f,
                    1,
                    0
                    ));
                }
                if (arcticPlayer.manaMode == true)
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                    node_1,
                    new Vector2(Main.screenWidth / 2 + 32, Main.screenHeight / 2 - 30),
                    nodeSource,
                    Color.White,
                    0,
                    nodeSource.Size() / 2f,
                    1,
                    0
                    ));
                }
                else
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                    node_3,
                    new Vector2(Main.screenWidth / 2 + 32, Main.screenHeight / 2 - 30),
                    nodeSource,
                    Color.White,
                    0,
                    nodeSource.Size() / 2f,
                    1,
                    0
                    ));
                }
                if (arcticPlayer.combinedMode == true)
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                    node_2,
                    new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - 40),
                    nodeSource,
                    Color.White,
                    0,
                    nodeSource.Size() / 2f,
                    1,
                    0
                    ));
                }
                else
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                    node_3,
                    new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 - 40),
                    nodeSource,
                    Color.White,
                    0,
                    nodeSource.Size() / 2f,
                    1,
                    0
                    ));
                }

            }
            prevCharge = arcticPlayer.charge;
        }
    }
    public class ScientistProbeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        }

        public override void Update(Terraria.Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ScientistProbe>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
    public class ScientistProbe : ModProjectile
    {
        public Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Equipables/Armor/ArcticScientist/ManaBeam", AssetRequestMode.ImmediateLoad).Value;
        public bool attackMode;
        public bool manaMode;
        public bool combinedMode;
        public bool healMana;
        public override bool PreDraw(ref Color lightColor)
        {
            if (healMana)
            {
                Vector2 ownerPos = Main.player[Projectile.owner].Center;
                Rectangle source = new(0, 0, 1, 20);
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                List<Vector2> drawPosList = new();
                float size;
                List<float> sizeList = new();
                Vector2 nextStep = Projectile.Center.DirectionTo(ownerPos).RotatedBy(180);
                float laserSteps = 200;
                Color fullColor = new Color(0.5f, 0.5f, 1f);
                Color opaqueColor = new Color(0.5f, 0.5f, 1f, 0.1f);

                for (int i = 0; i < laserSteps; i++)
                {
                    Vector2 step = nextStep;
                    Vector2 directionToPlayer = Projectile.Center.DirectionTo(ownerPos);
                    Vector2 offDirection = directionToPlayer.RotatedBy(MathHelper.ToRadians(90));
                    if (i < 75)
                    {
                        nextStep = Vector2.SmoothStep(directionToPlayer, offDirection, (laserSteps - Projectile.Center.Distance(ownerPos)) / laserSteps);
                    }
                    else
                    {
                        nextStep = Vector2.SmoothStep(nextStep, Vector2.SmoothStep(nextStep, drawPos.DirectionTo(ownerPos - Main.screenPosition), drawPos.Distance(ownerPos) / (laserSteps - i)), 0.1f);
                    }
                    drawPos = drawPos + step;
                    drawPosList.Add(drawPos);
                    if (drawPos.Distance(ownerPos) <= Main.player[Projectile.owner].width * 2)
                    {
                        break;
                    }
                    size = MathHelper.SmoothStep((laserSteps - i) / laserSteps, 1, 0.5f);
                    sizeList.Add(size);

                    Main.EntitySpriteDraw(texture, drawPos, source, fullColor, drawPos.DirectionTo(drawPos + nextStep).ToRotation(), source.Size() / 2f, 2f * size, 0);
                }
                for (int i = drawPosList.ToArray().Length - 1; i > 0; i--)
                {
                    Main.EntitySpriteDraw(texture, drawPosList[i], source, opaqueColor, drawPos.DirectionTo(drawPos + nextStep).ToRotation(), source.Size() / 2f, 1f * sizeList[i], 0);
                }
            }
            return true;
        }
        public override void SetStaticDefaults()
        {
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.DamageType = DamageClass.MagicSummonHybrid; // Declares the damage type (needed for it to deal damage)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Terraria.Player owner = Main.player[Projectile.owner];
            ArcticScientistPlayer arcticPlayer = owner.GetModPlayer<ArcticScientistPlayer>();
            attackMode = arcticPlayer.attackMode;
            manaMode = arcticPlayer.manaMode;
            combinedMode = arcticPlayer.combinedMode;

            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target);
            Movement(distanceToIdlePosition, vectorToIdlePosition, owner, foundTarget, distanceFromTarget, targetCenter, target);
        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Terraria.Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<ScientistProbeBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<ScientistProbeBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Terraria.Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }
        private void SearchForTargets(Terraria.Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;
            target = Main.npc[0];

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                    target = npc;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                            target = npc;
                        }
                    }
                }
            }
        }
        public float manaTimer = 20;
        public float shootTimer = 30;
        public bool attacking;
        private void Movement(float distanceToIdlePosition, Vector2 vectorToIdlePosition, Terraria.Player owner, bool foundTarget, float distanceFromTarget, Vector2 targetCenter, NPC target)
        {
            float speed = 8f;
            float inertia = 20f;

            if (foundTarget && (attackMode || (owner.statMana >= owner.statManaMax / 2 && combinedMode) || (owner.statMana == owner.statManaMax && manaMode)))
            {
                attacking = true;
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget < 1200f)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    Vector2 direction = targetCenter - Projectile.position + (Func.Diagonal(target.Hitbox) * new Vector2(2f * Math.Sign(target.Center.DirectionTo(vectorToIdlePosition).X), -2f));
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    Projectile.rotation = Projectile.Center.DirectionTo(targetCenter).ToRotation();

                    if (shootTimer > 0)
                    {
                        shootTimer--;
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center.DirectionTo(targetCenter) * 10f, ProjectileID.PurpleLaser, 10, 5);
                        shootTimer = 30;
                    }
                }
            }
            else
            {
                attacking = false;
                if (distanceToIdlePosition > 200f)
                {
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    speed = 6f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    vectorToIdlePosition.Normalize();
                    Projectile.rotation = vectorToIdlePosition.ToRotation();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
                if (manaTimer > 0)
                {
                    manaTimer--;
                }
                else if (healMana)
                {
                    manaTimer = 20;
                    owner.statMana += 2;
                    owner.ManaEffect(2);
                }
            }
            if (owner.statMana != owner.statManaMax && !attacking && distanceToIdlePosition < 200f)
            {
                healMana = true;
            }
            else
            {
                healMana = false;
            }
        }
    }
}
