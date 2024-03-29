using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Shade.System;
using Shade.Content02.Items.Materials;

namespace Shade.Content02.Items.Weapons
{
    public class ClutchOfTheForest : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 10);

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 36;
            Item.useTime = 42;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.damage = 25;
            Item.knockBack = 0f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<ClutchOfTheForestProjectile>();
        }
        public override bool CanUseItem(Terraria.Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool? UseItem(Terraria.Player player)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }

            return null;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SoulOfAnger>(), 3)
                .AddIngredient(ModContent.ItemType<SoulOfEvergrowth>(), 3)
                .Register();
        }
    }
    public class ClutchOfTheForestProjectile : ModProjectile
    {
        public Terraria.Player player; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
        public Vector2 grabOffset;
        public NPC grabbedNPC = null;
        public float progress;

        // Define the range of the Spear Projectile. These are overridable properties, in case you'll want to make a class inheriting from this one.
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 128f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist > 0f)
            {
                grabOffset = target.Center - Projectile.Center;
                grabbedNPC = target;
                target.velocity = new(0, 0);
            }
        }
        public override bool PreAI()
        {
            player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity.Normalize(); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.7f;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / (duration * 0.3f);
                if (progress < 0.5f)
                {
                    progress = 0.5f;
                    Projectile.Kill();
                }
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / (duration * 0.5f);
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);
            if (grabbedNPC != null && grabbedNPC.active)
            {
                grabbedNPC.velocity = new(0, 0);
                grabbedNPC.Center = Projectile.Center + grabOffset;
            }
            else
            {
                grabbedNPC = null;
            }

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(135f);
            }

            return false; // Don't execute vanilla AI.
        }
    }
}
