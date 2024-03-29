using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Shade.System;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Weapons
{
    public class RoyalBirdStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item44;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<RoyalBirdBuff>();
            Item.shoot = ModContent.ProjectileType<RoyalBird>();
        }
        public override void ModifyShootStats(Terraria.Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Terraria.Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
		}
	}
	public class RoyalBird : ModProjectile
	{
		public bool idle;
		public int dashDirection = -1;
		public bool dashing = false;
		public Vector2 targetPosition;
		public Vector2 spriteDashAngle;
		public Vector2 movementDirection;
		public bool setDirection = true;
		public int offsetIterations = 0;
		public int projID = 0;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
			Main.projFrames[Projectile.type] = 5;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			overPlayers.Add(index);
        }
        public sealed override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 24;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minionSlots = 1f;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public override bool MinionContactDamage()
		{
			return true;
		}

		public override void AI()
		{
			Terraria.Player owner = Main.player[Projectile.owner];

			if (!CheckActive(owner))
			{
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition, out Vector2 idlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target);
			Movement(foundTarget, idlePosition, targetCenter, distanceToIdlePosition, vectorToIdlePosition, target, owner);
			Visuals(foundTarget, targetCenter, owner);
		}

		private bool CheckActive(Terraria.Player owner)
		{
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<RoyalBirdBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<RoyalBirdBuff>()))
			{
				Projectile.timeLeft = 2;
			}

			return true;
		}

		private void GeneralBehavior(Terraria.Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition, out Vector2 idlePosition)
		{
			if (owner.ownedProjectileCounts[Projectile.type] > 1)
			{
				idlePosition = (owner.MountedCenter - Vector2.UnitY * owner.height / 2 + Main.OffsetsPlayerHeadgear[owner.bodyFrame.Y / owner.bodyFrame.Height] + new Vector2(-4, -14)).Floor();
				for (int i = 0; i < Main.projectile.Length; i++)
				{
					bool j = Main.projectile[i].type == Projectile.type && Main.projectile[i].frame == 4 && Main.projectile[i].Center == idlePosition && Main.projectile[i] != Projectile && idle;
					if (j)
					{
						offsetIterations++;
						if (offsetIterations % 2 == 0)
						{
							idlePosition.X -= 8;
							idlePosition.Y -= 8;
						}
						else
						{
							idlePosition.X += 8;
						}
						i = 0;
					}
				}
				offsetIterations = 0;
			}
			else
			{
                idlePosition = (owner.MountedCenter - Vector2.UnitY * owner.height / 2 + Main.OffsetsPlayerHeadgear[owner.bodyFrame.Y / owner.bodyFrame.Height] + Vector2.UnitY * -14).Floor();
            }
            vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
			{
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}
		}

		private void SearchForTargets(Terraria.Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target)
		{
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;
			target = Main.npc[0];

			if (owner.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				if (owner.Center.Distance(npc.Center) < 1200f)
				{
					target = npc;
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy())
					{
						target = npc;
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall) && owner.Center.Distance(npc.Center) < 1200f)
						{
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, Vector2 idlePosition, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition, NPC target, Terraria.Player owner)
		{
			float speed = 20f;
			float inertia = 20f;

			if (foundTarget && owner.Center.Distance(target.Center) < 1200f && targetCenter != Projectile.Center)
			{
				if (dashing)
				{
					movementDirection = target.Bottom + new Vector2(0, target.height / 4) - Projectile.Center;
					movementDirection.Normalize();
					spriteDashAngle = movementDirection;
					movementDirection *= speed;
					spriteDashAngle = Projectile.velocity;
					spriteDashAngle.Normalize();
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + movementDirection) / inertia;
					if (Projectile.Center.Distance(targetCenter + new Vector2(0 * dashDirection, 25)) < 80f)
					{
						dashDirection *= -1;
						dashing = false;
					}
				}
				else
				{
					targetPosition = target.Top + new Vector2(100 * -dashDirection, -25);
					movementDirection = targetPosition - Projectile.Center;
					movementDirection.Normalize();
					movementDirection *= speed;

					Projectile.velocity = (Projectile.velocity * (inertia - 1) + movementDirection) / inertia;
					if (Projectile.Center.Distance(targetPosition) < 40f)
					{
						dashing = true;
					}
				}
			}
			else
			{
				dashing = false;
				if (distanceToIdlePosition > 600f)
				{
					speed = 12f;
					inertia = 60f;
				}
				else
				{
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f)
				{
					idle = false;
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else
				{
					idle = true;
					Projectile.velocity = new(0, 0);
					Projectile.Center = idlePosition;
				}
			}
		}
		private void Visuals(bool foundTarget, Vector2 i, Terraria.Player player)
		{
			if (!dashing)
			{
				Projectile.rotation = Projectile.velocity.X * 0.05f;
			}
			else
			{
				spriteDashAngle.SafeNormalize(Vector2.Zero);
				if (spriteDashAngle != Vector2.Zero)
				{
					Projectile.rotation = spriteDashAngle.ToRotation() + (spriteDashAngle.X < 0f ? MathHelper.ToRadians(180) : 0);
				}
				Projectile.spriteDirection = -Math.Sign(spriteDashAngle.X);
			}
			if (foundTarget && !dashing)
			{
				Projectile.spriteDirection = -Math.Sign(i.X - Projectile.Center.X);
			}
			else if (!foundTarget && !idle)
			{
				Projectile.spriteDirection = -Math.Sign(player.Center.X - Projectile.Center.X);
			}
			else if (idle) 
			{
				Projectile.spriteDirection = -player.direction;
			}
			int frameSpeed = 5;

			Projectile.frameCounter++;

			if (Projectile.frameCounter >= frameSpeed)
			{
				Projectile.frameCounter = 0;
				if (!idle)
				{
					Projectile.frame++;
				}
				else
				{
					Projectile.frame = 4;
				}
				if (Projectile.frame >= Main.projFrames[Projectile.type] - 1 && !idle)
				{
					Projectile.frame = 0;
				}
			}
		}
		public override void PostDraw(Color lightColor)
        {
			if (dashing) default(TrailDrawer).Draw(Projectile);
        }
    }
	public struct TrailDrawer
	{
        private static VertexStrip _vertexStrip = new VertexStrip();
        public void Draw(Projectile proj)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["MagicMissile"];
            miscShaderData.UseSaturation(-2.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + (proj.Size / 2));
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.Orange, Color.OrangeRed, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A /= 2;
            return result;
        }

        private float StripWidth(float progressOnStrip) => MathHelper.Lerp(52f, 64f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
	}

	public class RoyalBirdBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Terraria.Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<RoyalBird>()] > 0)
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
    public class RoyalBirdCushion : PlayerDrawLayer
	{
		public Texture2D texture = ModContent.Request<Texture2D>("Shade/Content02/Items/Weapons/RoyalBirdCushion", AssetRequestMode.ImmediateLoad).Value;
		public Rectangle source = new Rectangle(0, 0, 28, 20);
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Terraria.Player player = drawInfo.drawPlayer;
			if (player.ownedProjectileCounts[ModContent.ProjectileType<RoyalBird>()] > 0)
			{
                drawInfo.DrawDataCache.Add(new DrawData(texture,
				(new Vector2(player.Top.X, drawInfo.Position.Y + player.height - player.bodyFrame.Height + 12 + drawInfo.helmetOffset.Y) + Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height] - Main.screenPosition).Floor(),
				source,
				drawInfo.colorArmorHead,
				0,
				source.Size() / 2f,
				1,
				0));
            }
			
		}
    }
}
