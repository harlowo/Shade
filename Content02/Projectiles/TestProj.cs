using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Content02.Projectiles
{
    public class TestProj : ModProjectile
	{
		int npcTargetID = -1;
		NPC npcTarget = null;
		float targetDistance;
		Vector2 targetDirection;
		int targetHits = 1;
		float inertia = 4f;
		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 0;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
		public override void AI()
		{
			// Effects

			Lighting.AddLight(Projectile.Center, 1f, 0.3f, 0f);

			int dust = Dust.NewDust(Projectile.Center, 5, 5, DustID.Torch, 0f, 0f, 230, default(Color), 1f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].velocity *= 0.3f;
            Main.dust[dust].velocity.X *= Main.rand.Next(10, 30) * 0.1f / (Main.dust[dust].velocity.X / Main.dust[dust].velocity.Y * 0.7f);
            Main.dust[dust].velocity.Y *= Main.rand.Next(10, 30) * 0.1f / (Main.dust[dust].velocity.Y / Main.dust[dust].velocity.X * 0.7f);
			Main.dust[dust].scale = Main.rand.Next(5, 20) * 0.05f;

			int dust_2 = Dust.NewDust(Projectile.Center, 5, 5, DustID.Pixie, 0f, 0f, 230, default(Color), 1f);
            Main.dust[dust_2].noGravity = true;
			Main.dust[dust_2].velocity *= 0.9f;
            Main.dust[dust_2].scale = Main.rand.Next(4, 14) * 0.1f;

			// AI
			if (npcTargetID != -1 && npcTarget.active)
			{
				targetDistance = Vector2.Distance(Main.npc[npcTargetID].Center, Projectile.Center);
				targetDirection = Main.npc[npcTargetID].Center - Projectile.Center;
				targetDirection.Normalize();
				Projectile.velocity = (Projectile.velocity * inertia + targetDirection * (targetDistance / 10)) / (inertia + 0.16f);
			}
			else if (npcTarget != null && !npcTarget.active)
			{
				Projectile.Kill();
			}
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (npcTargetID == -1)
			{
				npcTargetID = target.whoAmI;
			}
			else if (target.whoAmI == npcTargetID && targetHits != 3)
			{
				targetHits++;
			}
			else if (target.whoAmI == npcTargetID && targetHits == 3)
			{
                Projectile.Kill();
            }
			if (npcTarget == null)
			{
				npcTarget = target;
			}
		}
	}
}