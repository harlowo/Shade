using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Shade.Content02.Items.Weapons.OnyxClaymore
{
    public class OnyxClaymore : ModItem
    {
        int attackType;
        public override void SetDefaults()
        {
            Item.damage = 428;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.knockBack = 8;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 65, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ClaymoreSwing>();
            Item.shootSpeed = 15f;
            Item.reuseDelay = 5;

        }
        public override bool CanUseItem(Player player)
        {
            OnyxWeaponPlayer playa = player.GetModPlayer<OnyxWeaponPlayer>();
            if (attackType == 0) //downwards swing, slower
            {
                Item.useTime = Item.useAnimation = playa.OnyxFury == true ? 35 : 45;
            }
            if (attackType == 1) //upwards swing, faster
            {
                Item.useTime = Item.useAnimation = playa.OnyxFury == true ? 25 : 35;
            }
            if (attackType == 2) //the spin
            {
                Item.useTime = Item.useAnimation = 60;
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            OnyxWeaponPlayer playa = player.GetModPlayer<OnyxWeaponPlayer>();
            if (player.altFunctionUse == 2)
            {
                if(playa.OnyxWrathStacks >=20)
                {
                    //apply the buff
                    player.AddBuff(BuffType<OnyxFury>(), 720);
                    Rectangle textPos = new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height);
                    CombatText.NewText(textPos, new Color(200, 20, 200), "Onyx Fury Activated", false, false);
                    playa.OnyxWrathStacks = 0;
                }
            }
            return true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            OnyxWeaponPlayer mp = player.GetModPlayer<OnyxWeaponPlayer>();
            if (player.altFunctionUse != 2)
            {
                switch (attackType)
                {
                    case 0: //Swing downwards
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, position);
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ClaymoreSwing>(), (int)(damage * 0.85f), knockback, player.whoAmI, 0, 0, player.direction);
                        attackType++;
                        return false;
                    case 1: //Swing upwards
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, position);
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ClaymoreSwing>(), (int)(damage * 1.25f), knockback, player.whoAmI, 0, 1, player.direction);
                        attackType++;
                        return false;
                    case 2: //Spin downwards
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, position);
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ClaymoreSpin>(), (int)(damage * 1.4f), knockback, player.whoAmI, 0, 0, player.direction);
                        //fire the disc
                        Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<OnyxSlashDisc>(), (int)(damage * .7f), knockback, player.whoAmI);
                        attackType = 0;
                        return false;
                }
                //fallback
                if (attackType > 2)
                {
                    attackType = 0;
                }
            }
            return false;
        }
    }
    public class OnyxWeaponPlayer : ModPlayer
    {
        public int OnyxWrathStacks = 0;
        public bool OnyxFury; //for the buff
        public override void ResetEffects()
        {
            OnyxFury = false;
        }
        public override void PreUpdate()
        {
            OnyxWrathStacks = Math.Clamp(OnyxWrathStacks, 0, 20);
        }

    }
    //the swing
    public class ClaymoreSwing : BaseSword
    {
        public override string Texture => "Shade/Content02/Items/Weapons/OnyxClaymore/OnyxClaymore";
        public override bool UseRecoil => false;
        public override bool DoSpin => false;
        public override float BaseDistance => 34;
        public override Color BackDarkColor => Color.Violet;
        public override Color MiddleMediumColor => Color.MediumPurple;
        public override Color FrontLightColor => Color.Purple;
        public override float ScaleAdder => 1f;
        public override float ScaleMulti => .5f;
        public override bool Rotate45Degrees => true;
        public override bool CenterOnPlayer => true;
        public override int DustType => DustID.GemAmethyst;
        public override Color BladeLightingColor => Color.Purple;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X -= 30;
            hitbox.Y -= 30;
            hitbox.Width += 60;
            hitbox.Height += 60;
            base.ModifyDamageHitbox(ref hitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hit.HitDirection = Main.player[Projectile.owner].Center.X < target.Center.X ? 1 : -1;
            float dustAmount = 16f;
            float randomConstant = MathHelper.ToRadians(Main.rand.Next(0, 360));
            float randomConstant2 = Main.rand.Next(0, 360);
            for (int i = 0; i < dustAmount; i++)
            {
                Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                spinningpoint5 += -Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(25f, 1f);
                spinningpoint5 = spinningpoint5.RotatedBy(target.velocity.ToRotation() + randomConstant);
                int dust = Dust.NewDust(target.Center, 0, 0, DustID.GemAmethyst);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = target.Center + spinningpoint5;
                Main.dust[dust].velocity = target.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * (1.85f + randomConstant2 / 40);
            }
            OnyxWeaponPlayer playa = Main.player[Projectile.owner].GetModPlayer<OnyxWeaponPlayer>();
            if (playa.OnyxFury)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile proj = ProjectileRain(Projectile.InheritSource(Projectile), target.Center, 150, 50, 500, 800, 20f, ProjectileID.BlackBolt, Projectile.damage / 3, 3f, Projectile.owner);
                    proj.timeLeft = (int)(proj.timeLeft * 5f);
                }
            }
            else
            {
                playa.OnyxWrathStacks += 1;
                if (playa.OnyxWrathStacks <= 20)
                {
                    Rectangle textPos = new Rectangle((int)Main.player[Projectile.owner].position.X, (int)Main.player[Projectile.owner].position.Y - 20, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height);
                    CombatText.NewText(textPos, new Color(200, 20, 200), $"{playa.OnyxWrathStacks}", false, false);
                }
            }
        }
    }
    //the spin
    public class ClaymoreSpin : BaseSword
    {
        public override string Texture => "Shade/Content02/Items/Weapons/OnyxClaymore/OnyxClaymore";
        public override bool UseRecoil => false;
        public override bool DoSpin => true;
        public override float BaseDistance => 34;
        public override Color BackDarkColor => Color.Violet;
        public override Color MiddleMediumColor => Color.MediumPurple;
        public override Color FrontLightColor => Color.Purple;
        public override float ScaleAdder => 1f;
        public override float ScaleMulti => .5f;
        public override bool Rotate45Degrees => true;
        public override bool CenterOnPlayer => true;
        public override int DustType => DustID.GemAmethyst;
        public override Color BladeLightingColor => Color.Purple;  //
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30; //can hit twice
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X -= 30;
            hitbox.Y -= 30;
            hitbox.Width += 60;
            hitbox.Height += 60;
            base.ModifyDamageHitbox(ref hitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hit.HitDirection = Main.player[Projectile.owner].Center.X < target.Center.X ? 1 : -1;
            float dustAmount = 16f;
            float randomConstant = MathHelper.ToRadians(Main.rand.Next(0, 360));
            float randomConstant2 = Main.rand.Next(0, 360);
            for (int i = 0; i < dustAmount; i++)
            {
                Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                spinningpoint5 += -Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f / dustAmount)) * new Vector2(25f, 1f);
                spinningpoint5 = spinningpoint5.RotatedBy(target.velocity.ToRotation() + randomConstant);
                int dust = Dust.NewDust(target.Center, 0, 0, DustID.GemAmethyst);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = target.Center + spinningpoint5;
                Main.dust[dust].velocity = target.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * (1.85f + randomConstant2 / 40);
            }
            OnyxWeaponPlayer playa = Main.player[Projectile.owner].GetModPlayer<OnyxWeaponPlayer>();
            if (playa.OnyxFury)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile proj = ProjectileRain(Projectile.InheritSource(Projectile), target.Center, 150, 50, 500, 800, 20f, ProjectileID.BlackBolt, Projectile.damage / 3, 3f, Projectile.owner);
                    proj.timeLeft = (int)(proj.timeLeft * 5f);
                }
            }
            else
            {
                playa.OnyxWrathStacks += 1;
                if (playa.OnyxWrathStacks <= 20)
                {
                    Rectangle textPos = new Rectangle((int)Main.player[Projectile.owner].position.X, (int)Main.player[Projectile.owner].position.Y - 20, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height);
                    CombatText.NewText(textPos, new Color(200, 20, 200), $"{playa.OnyxWrathStacks}", false, false);
                }
            }
        }
    }
    public class OnyxSlashDisc : ModProjectile
    {
        public override string Texture => "Extras/Graphics/VanillaSlashEffect";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.scale = 1.4f;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.usesLocalNPCImmunity = true;
        }

        float VelMult = 1f;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3());
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 18f * 0.03f * Projectile.direction;
            int size = 129;
            bool collding = Collision.SolidCollision(Projectile.position + new Vector2(size / 2f, size / 2f), 2, 2);
            if (collding || Projectile.penetrate <= 1)
            {
                Projectile.alpha += 16;
                Projectile.scale -= 0.01f;
                VelMult *= 0.80f;
            }
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
            Projectile.frame = 0;

            Projectile.velocity.Normalize();
            Projectile.velocity *= 15f * VelMult;
            Projectile.localAI[0]++;
            Projectile.idStaticNPCHitCooldown = 10 - (int)(10 * (player.GetAttackSpeed(DamageClass.Melee) + player.GetAttackSpeed(DamageClass.Generic)));
            if (Projectile.idStaticNPCHitCooldown < 5)
                Projectile.idStaticNPCHitCooldown = 5;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8);
            if (Projectile.alpha < 200)
            {
                for (int i = 0; i < 30; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Scale: 1f);
                    d.velocity *= 0.4f;
                    d.velocity += Projectile.velocity * 0.5f;
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale * 0.6f;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.alpha < 200)
            {
                for (int i = 0; i < 30; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Scale: 1f);
                    d.velocity *= 0.4f;
                    d.velocity += Projectile.velocity * 0.5f;
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale * 0.6f;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                }
            }
        }

        // This code is a fucking amalgamation, and it brings naught but an inexplicable pain upon those who suffer to read it
        public override bool PreDraw(ref Color lightColor)
        {
            var opacity = Projectile.Opacity;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.direction == -1)
                spriteEffects = SpriteEffects.FlipVertically;

            float num3 = Utils.Remap(Projectile.localAI[0] / 60, 0f, 0.6f, 0f, 1f) * Utils.Remap(Projectile.localAI[0] / 60, 0.6f, 1f, 0.8f, 0f);
            Color color6 = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            Vector3 val = color6.ToVector3();
            float fromValue = val.Length() / (float)Math.Sqrt(3.0);
            fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);

            Texture2D swish = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            var swishFrame = swish.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Color swishColor = Color.Violet with { A = 0} * opacity;
            Color swishColor2 = Color.Purple with { A = 100 } * opacity;
            var swishOrigin = swishFrame.Size() / 2;
            var drawOffset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            float swishScale = Projectile.scale;
            var swishPosition = Projectile.position + drawOffset;

            var flareColor = Color.Purple * num3 * 0.5f * opacity;
            flareColor.A = (byte)(float)(int)(flareColor.A * (1f - fromValue));
            float swishRotation = Projectile.rotation;

            Color color5 = flareColor * fromValue * 0.5f;
            color5.G = (byte)(color5.G * fromValue);
            color5.B = (byte)(color5.R * (0.25f + fromValue * 0.75f));

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 1f)
            {
                swishColor *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                swishColor2 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                flareColor *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color5 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];

                swishRotation += MathHelper.ToRadians(120f * i);

                Main.EntitySpriteDraw(swish, swishPosition, swishFrame, swishColor * num3,
                    swishRotation + MathHelper.ToRadians(-20f * Projectile.direction), swishOrigin, swishScale, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swishFrame, swishColor2 * 1.25f * num3,
                    swishRotation + MathHelper.ToRadians(20f * Projectile.direction), swishOrigin, swishScale, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swishFrame, color5 * 0.15f,
                    swishRotation + Projectile.ai[0] * 0.01f, swishOrigin, swishScale, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swishFrame, swishColor2 * num3 * 0.3f,
                    swishRotation, swishOrigin, swishScale, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swishFrame, swishColor2 * num3 * 0.5f,
                    swishRotation, swishOrigin, swishScale * 0.975f, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swishFrame, swishColor * num3,
                    swishRotation, swishOrigin, swishScale * 0.75f, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swish.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame + 3),
                    swishColor2 * num3, swishRotation, swishOrigin, swishScale, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swish.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame + 3),
                    swishColor2 * 0.8f * num3, swishRotation, swishOrigin, swishScale, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swish.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame + 3),
                    flareColor * num3, swishRotation, swishOrigin, swishScale * 0.8f, spriteEffects, 0);

                Main.EntitySpriteDraw(swish, swishPosition, swish.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame + 3),
                    flareColor * num3, swishRotation, swishOrigin, swishScale * 0.65f, spriteEffects, 0);
            }

            return false;
        }
    }
//needs a new texture
    public class OnyxFury : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Magic) += 0.2f; // 20% more damama
            player.GetCritChance(DamageClass.Magic) += 20; // 15% more khridgidal chanze
            player.GetModPlayer<OnyxWeaponPlayer>().OnyxFury = true;

        }
    }
}
