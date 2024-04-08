using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent;
using System;
using Terraria.Enums;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Shade.Common.Base;
using Shade.Common.ParticleSystem;
using Shade.Common.Players;
//really inconvenient namespace, i highly suggest merging the 2 content folders, and putting assets into common
//note that this will cause a drastic need for file refactorization, and multiple things will break
//however, this will make the place much more readable, and less painful to find stuff
namespace Shade.Content02.Items.Weapons.OnyxRailgun
{
    public class OnyxRailgun : ModItem //reworked and sexier than ever!
    {
        public override void SetDefaults()
        {
            Item.damage = 237;
            Item.width = 154;
            Item.height = 44;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.knockBack = 7;
            Item.useTurn = false;
            Item.rare = ItemRarityID.LightPurple; //making a custom rarity for all onyx stuffs soon, maybe?!
            Item.autoReuse = true;
            Item.shoot = ProjectileType<OnyxRailgunHoldout>();
            Item.shootSpeed = 0.1f;
            Item.noUseGraphic = true;
            Item.channel = true;
            //Item.useAmmo = AmmoID.Bullet;
            Item.value = Item.sellPrice(gold: 60);
        }
        public override bool AltFunctionUse(Player player) => true;
        //public override bool CanConsumeAmmo(Item ammo, Player player) => false; //so spawning the holdout doesnt use ammo (does this use ammo?)
        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<OnyxRailgunHoldout>());
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
                Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<OnyxRailgunHoldout>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<OnyxRailgunHoldout>(), damage, knockback, player.whoAmI, ai2: 1);
            return false;
        }
    }
    public class OnyxRailgunHoldout : ModProjectile
    {
        public override string Texture => "Shade/Content02/Items/Weapons/OnyxRailgun/OnyxRailgun";
        public ref float Charge => ref Projectile.ai[0];
        public ref float KillTimer => ref Projectile.ai[1];
        public bool SpawnedByRightClick => Projectile.ai[2] == 1; //spawn the proj with an ai[2] value of 1 in shoot when right clicking!
        public bool CanBeUsed = true;
        public bool ShouldChange = true;
        public bool playedFullyChargedSound;
        public override void SetDefaults()
        {
            Projectile.width = 154;
            Projectile.height = 44;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.netImportant = true;

        }
        private Player Owner => Main.player[Projectile.owner];
        public override void AI()
        {
            if (Owner.dead || !Owner.active || Owner.noItems || Owner.CCed)
                Projectile.Kill();

            if (Owner.HeldItem.type == ModContent.ItemType<OnyxRailgun>())
            {
                Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);
                Projectile.CritChance = Owner.GetWeaponCrit(Owner.HeldItem);
                Projectile.knockBack = Owner.GetWeaponKnockback(Owner.HeldItem, Owner.HeldItem.knockBack);
            }
            Vector2 armPos = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPos = armPos + Projectile.velocity * Projectile.width * 0.75f + new Vector2(0, -5f);
            Vector2 FiringVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 35; //fast as hell
            ControlsPlayer cPlaya = Owner.GetModPlayer<ControlsPlayer>(); //handles right click channeling!
            if (Projectile.owner == Main.myPlayer)
            {
                #region heldProj stuff that is dumb but needed
                if (Main.myPlayer == Projectile.owner)
                {
                    float interpolant = Utils.GetLerpValue(5f, 55f, Projectile.Distance(Main.MouseWorld), true);
                    Vector2 oldVelocity = Projectile.velocity;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                    if (Projectile.velocity != oldVelocity)
                    {
                        Projectile.netSpam = 0;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.Center = armPos + Projectile.velocity * MathHelper.Clamp(32f, 0f, 32f) + new Vector2(0, 5);
                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
                Projectile.spriteDirection = Projectile.direction;
                Owner.ChangeDir(Projectile.direction);
                Owner.heldProj = Projectile.whoAmI;
                Owner.itemTime = 2;
                Owner.itemAnimation = 2;
                Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
                #endregion
                if (SpawnedByRightClick)
                {
                    if (cPlaya.mouseRight)
                    {
                        if (++Charge >= 45)
                        {
                            SoundEngine.PlaySound(SoundID.Item33, tipPos);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPos, (FiringVelocity * 0.65f).RotatedByRandom(MathHelper.ToRadians(5f)), ProjectileType<OnyxShot>(), (int)(Projectile.damage * .75f), 2f, Projectile.owner);
                            Charge = 0;
                        }
                        else
                        {
                            Vector2 vector = new Vector2(Main.rand.Next(-9, 9) * (0.01f - (46 / (Charge + 1))), Main.rand.Next(-9, 9) * (0.01f - (46 / (Charge + 1))));
                            Dust d = Main.dust[Dust.NewDust(tipPos + vector, 1, 1, DustType<OnyxDust>(), 0, 0, 0, default, 0.8f)];
                            d.velocity = -vector / 12;
                            d.velocity -= Owner.velocity / 8;
                            d.noLight = true;
                            d.noGravity = true;
                        }
                    }
                    if (!cPlaya.mouseRight)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    if (Owner.channel && CanBeUsed)
                    {
                        Projectile.timeLeft++;
                        if (++Charge >= 120)
                        {
                            Charge = 120; //don't go over 120
                            //dust blob at the tip
                            float dustAmount = 2f;
                            float randomConstant = ToRadians(Main.rand.Next(0, 360));
                            for (int i = 0; (float)i < dustAmount; i++)
                            {
                                Vector2 spinningpoint = Vector2.UnitX * 0f;
                                spinningpoint += -Vector2.UnitY.RotatedBy((float)i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(5f, 1f);
                                spinningpoint = spinningpoint.RotatedBy(Projectile.velocity.ToRotation() + randomConstant);
                                int dust = Dust.NewDust(tipPos, 0, 0, DustType<OnyxDust>());
                                Main.dust[dust].scale = Main.rand.NextFloat(1.1f, 1.45f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].position = tipPos + spinningpoint;
                                Main.dust[dust].velocity = Projectile.velocity * 0f + spinningpoint.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(0.2f, 0.35f);
                            }
                        }
                        else
                        {
                            //draw in dust near the tip 
                            Vector2 vector = new Vector2(Main.rand.Next(-9, 9) * (0.01f - (121 / (Charge + 1))), Main.rand.Next(-9, 9) * (0.01f - (121 / (Charge + 1))));
                            Dust d = Main.dust[Dust.NewDust(tipPos + vector, 1, 1, DustType<OnyxDust>(), 0, 0, 0, default, 0.8f)];
                            d.velocity = -vector / 12;
                            d.velocity -= Owner.velocity / 8;
                            d.noLight = true;
                            d.noGravity = true;
                        }
                        HandleSounds(tipPos);

                    }
                    if (!Owner.channel)
                    {
                        CanBeUsed = false;
                        int damageCalculated = (int)(Projectile.damage * (1.5f * (Charge / 30))); //109 * 1.25 * (120 / 30) => 136.25 * 4 =>  545 dmg 
                        int damageReal = (int)(damageCalculated * Main.rand.NextFloat(0.85f, 1.15f)); //make it a bit more random
                        if (Charge >= 15 && ShouldChange)
                        {
                            SoundEngine.PlaySound(SoundID.Item68, tipPos);
                            Owner.velocity += -Projectile.velocity * 10.5f; //weeee
                        }
                        if (Charge >= 120 && ShouldChange)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPos, FiringVelocity.RotatedByRandom(MathHelper.ToRadians(0.1f)), ProjectileType<OnyxBeam>(), (int)(damageReal * 1.2f), 8f, Projectile.owner, ai2: 1.5f);
                            for (int i = 0; i < 2; i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPos, (FiringVelocity * 0.45f).RotatedByRandom(MathHelper.ToRadians(15f)), ProjectileType<OnyxShot>(), (int)(damageReal * .2f), 8f, Projectile.owner);
                            }
                            KillTimer = 35;
                            ShouldChange = false;
                        }
                        else
                        {
                            if (Charge < 15 && ShouldChange)
                            {
                                KillTimer = 5;
                                ShouldChange = false;
                            }
                            if (Charge < 30 && Charge >= 15 && ShouldChange)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPos, FiringVelocity.RotatedByRandom(MathHelper.ToRadians(0.1f)), ProjectileType<OnyxBeam>(), damageReal / 2, 1f, Projectile.owner, ai2: .25f);
                                KillTimer = 10;
                                ShouldChange = false;
                            }
                            if (Charge < 60 && Charge >= 30 && ShouldChange)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPos, FiringVelocity.RotatedByRandom(MathHelper.ToRadians(0.1f)), ProjectileType<OnyxBeam>(), damageReal, 3f, Projectile.owner, ai2: .5f);
                                KillTimer = 20;
                                ShouldChange = false;
                            }
                            if (Charge < 90 && Charge >= 60 && ShouldChange)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPos, FiringVelocity.RotatedByRandom(MathHelper.ToRadians(0.1f)), ProjectileType<OnyxBeam>(), damageReal, 4f, Projectile.owner, ai2: .75f);
                                KillTimer = 25;
                                ShouldChange = false;
                            }
                            if (Charge < 120 && Charge >= 90 && ShouldChange)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPos, FiringVelocity.RotatedByRandom(MathHelper.ToRadians(0.1f)), ProjectileType<OnyxBeam>(), damageReal, 6f, Projectile.owner, ai2: 1.25f);
                                KillTimer = 30;
                                ShouldChange = false;
                            }
                        }
                        //Main.NewText(KillTimer);
                        if (KillTimer > 0)
                        {
                            KillTimer--;
                        }
                        if (KillTimer <= 0)
                        {
                            Projectile.Kill();
                            CanBeUsed = true;
                            ShouldChange = true;
                        }
                    }
                }
            }
            else
            {
                Projectile.Center += Projectile.velocity * 20;
                return;
            }
        }
        /// <summary>
        /// plays a sound when at certain intervals of charge.
        /// </summary>
        public void HandleSounds(Vector2 soundPos)
        {
            if (CanBeUsed) //dont play any sounds when in recoil state
            {
                if (Charge == 15)
                {
                    SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -0.4f, PitchVariance = 0.1f }, soundPos);
                }
                if (Charge == 30)
                {
                    SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -0.3f, PitchVariance = 0.1f }, soundPos);
                }
                if (Charge == 60)
                {
                    SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -0.2f, PitchVariance = 0.1f }, soundPos);
                }
                if (Charge == 90)
                {
                    SoundEngine.PlaySound(SoundID.Item29 with { Pitch = -0.1f, PitchVariance = 0.1f }, soundPos);
                }
                if (Charge == 120 && !playedFullyChargedSound)
                {
                    playedFullyChargedSound = true;
                    SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.15f, PitchVariance = 0.1f }, soundPos);
                }
            }
        }
        #region drawing
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(sourceRectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Weapons/OnyxRailgun/OnyxRailgunGlow").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(sourceRectangle), Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

        }
        #endregion
    }
    public class OnyxShot : ModProjectile
    {
        public override string Texture => "Shade/Common/Textures/InvisibleProj";
        public int PotentialTargetIndex = -1;
        public float Time = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // how long you want the trail to be
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = 2;//2 hits!
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 900; //
            Projectile.light = 0.25f;
            Projectile.extraUpdates = 2;
            Projectile.Opacity = 0f;
        }
        public override void AI()
        {
            Projectile.Opacity += 0.1f; //fade in
            if (Projectile.Opacity >= 1f)
                Projectile.Opacity = 1f;
            if (Projectile.velocity.Length() < 20f)
                Projectile.velocity *= 1.025f; //speed up a bit
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (++Time >= 30) //home in after a bit
            {
                if (PotentialTargetIndex >= 0) //we found a target!
                {
                    if (!Main.npc[PotentialTargetIndex].active || !Main.npc[PotentialTargetIndex].CanBeChasedBy())
                        PotentialTargetIndex = -1; //oh. they arent valid. nevermind.

                    else //they are valid!!! time to chase them!
                    {
                        Vector2 idealVelocity = Projectile.SafeDirectionTo(Main.npc[PotentialTargetIndex].Center) * (Projectile.velocity.Length() + 3f);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.045f);
                    }
                }

                if (PotentialTargetIndex == -1) //looking for a target rn
                {
                    NPC potentialTarget = Projectile.Center.ClosestNPCAt(1600f, false);
                    if (potentialTarget != null)
                        PotentialTargetIndex = potentialTarget.whoAmI;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float dustAmount = 7f;
            float randomConstant = ToRadians(Main.rand.Next(0, 360));
            for (int i = 0; (float)i < dustAmount; i++)
            {
                Vector2 spinningpoint = Vector2.UnitX * 0f;
                spinningpoint += -Vector2.UnitY.RotatedBy((float)i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(10f, 3f);
                spinningpoint = spinningpoint.RotatedBy(Projectile.velocity.ToRotation() + randomConstant);
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemAmethyst);
                Main.dust[dust].scale = Main.rand.NextFloat(1.1f, 1.45f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = Projectile.Center + spinningpoint;
                Main.dust[dust].velocity = Projectile.velocity * 0f + spinningpoint.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(1.2f, 2.35f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            float dustAmount = 30f;
            float randomConstant = ToRadians(Main.rand.Next(0, 360));
            for (int i = 0; (float)i < dustAmount; i++)
            {
                Vector2 spinningpoint = Vector2.UnitX * 0f;
                spinningpoint += -Vector2.UnitY.RotatedBy((float)i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(15f, 5f);
                spinningpoint = spinningpoint.RotatedBy(Projectile.velocity.ToRotation() + randomConstant);
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemAmethyst);
                Main.dust[dust].scale = Main.rand.NextFloat(1.1f, 1.45f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = Projectile.Center + spinningpoint;
                Main.dust[dust].velocity = Projectile.velocity * 0f + spinningpoint.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(2.2f, 3.35f);
            }
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //draw ze glowy
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D bloomTexture = ModContent.Request<Texture2D>("Shade/Content02/Items/Weapons/OnyxRailgun/GlowyBall").Value;
            Color bloomColor = Color.Lerp(Color.Purple, Color.Violet, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 15f) * 0.05f + 0.08f);
            Vector2 Center = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(bloomTexture, Center, null, bloomColor, 0, bloomTexture.Size() / 2f, 0.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //then draw the trail and base texure OVER it
            Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Weapons/OnyxRailgun/OnyxShot").Value;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 offset = new(Projectile.width / 2f, Projectile.height / 2f);
                Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset;
                float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
                Color color = Color.Lerp(Color.Purple, Color.Violet, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 15f) * 0.05f + 0.08f) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = 0;
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2, sizec * 0.09f, SpriteEffects.None);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White with { A = 0 } * Projectile.Opacity;
        }
    }
    public class OnyxBeam : BaseRay
    {
        public override float MaxScale => 1f * ChargePercent;
        public override float MaxLaserLength => 2000f;
        public override float Lifetime => 30f;
        public ref float ChargePercent => ref Projectile.ai[2];
        public override Color LightCastColor => Color.Purple;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("Shade/Common/Textures/RayStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("Shade/Common/Textures/RayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("Shade/Common/Textures/RayEnd", AssetRequestMode.ImmediateLoad).Value;
        public override string Texture => "Shade/Common/Textures/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }

        public override void ExtraBehavior()
        {
            // corcol duzt, maybe replace wth the ballz
            if (!Main.dedServ && Time == 5f)
            {
                int Points = 35;
                for (int i = 0; i < Points; i++)
                {
                    float angle = MathHelper.TwoPi * i / Points;
                    Dust onyxDust = Dust.NewDustPerfect(Projectile.Center, DustType<OnyxDust>());
                    onyxDust.velocity = angle.ToRotationVector2() * 6f;
                    onyxDust.scale = 1.2f;
                    onyxDust.noGravity = true;
                }
            }
        }
        public override void PostAI()
        {
            if (Projectile.timeLeft == 30)
            {
                for (float Offset = 0f; Offset < MaxLaserLength; Offset += 10f) //spawns 200 orbs! 
                {
                    Vector2 SpawnPosition = Projectile.Center + Projectile.velocity * Offset;
                    Vector2 Velocity = Projectile.velocity;
                    //the dust just looks bad
          
                   /*Dust onyxDust = Dust.NewDustPerfect(SpawnPosition, DustType<OnyxDust>());
                     onyxDust.position = SpawnPosition;
                     onyxDust.velocity = Velocity * 0f;
                     onyxDust.noGravity = true;
                     onyxDust.scale = 2.56f * Main.rand.NextFloat(0.85f, 1.1f); */
                    //PARTICLES? PARTICLES! (whole reason behind this was after seeign the railgun from fortnite's lil trail of red orbs after firing)
                    BaseParticle particle = new ExpandingBloomParticle(SpawnPosition,
                        Velocity,
                        Color.Lerp(Color.Purple, Color.Violet, (float)Math.Sin(Main.GlobalTimeWrappedHourly * Math.PI)),
                        Vector2.One * 0.95f,
                        Vector2.One * 0.25f,
                        30,
                        true,
                        Color.Lerp(Color.Purple, Color.MediumPurple, (float)Math.Cos(Main.GlobalTimeWrappedHourly * Math.PI)));
                    particle.Spawn();
                }
            }
        }
        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBeamWithColor(Color.Purple * .85f, Projectile.scale * 0.5f); //inner
            DrawBeamWithColor(Color.DarkViolet * .9f, Projectile.scale); //outer
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) //uses the original code
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f); //15% damage falloff per hit
            float dustAmount = 2f;
            float randomConstant = ToRadians(Main.rand.Next(0, 360));
            for (int i = 0; (float)i < dustAmount; i++)
            {
                Vector2 spinningpoint = Vector2.UnitX * 0f;
                spinningpoint += -Vector2.UnitY.RotatedBy((float)i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(18f * ChargePercent, 5f * ChargePercent);
                spinningpoint = spinningpoint.RotatedBy(Projectile.velocity.ToRotation() + randomConstant);
                int dust = Dust.NewDust(target.Center, 0, 0, DustType<OnyxDust>());
                Main.dust[dust].scale = Main.rand.NextFloat(1.1f, 1.45f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = target.Center + spinningpoint;
                Main.dust[dust].velocity = Projectile.velocity * 0f + spinningpoint.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(1.2f * ChargePercent, 2.35f * ChargePercent);
            }
        }
    }
}
