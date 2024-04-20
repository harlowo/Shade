using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Common.Base
{
    /// <summary>
    /// A base class for yoyo projectiles. Does nothing except handle yoyo logic, everything else is up to you.
    /// </summary>
    public abstract class BaseYoyoProjectile : ModProjectile
    {
        #region overrideable fields/methods
        /// <summary>
        /// how many times the yoyo should be updated per tick
        /// </summary>
        public abstract int MaxUpdates { get; }

        /// <summary>
        /// The distance the yoyo can travel, in tiles
        /// </summary>
        public abstract float MaxRange { get; }

        /// <summary>
        /// The lifetime of the yoyo, in seconds
        /// </summary>
        public abstract float LifeTime { get; }
        /// <summary>
        /// How fast the yoyo should spin, also how fast it moves around relative to the mouse
        /// </summary>
        public abstract float TopSpeed { get; }

        /// <summary>
        /// Length of the afteriamges behind the yoyo (if applied). Set to 0 for none
        /// </summary>
        public abstract int TrailLength { get; }

        /// <summary>
        /// how many ticks, before being multiplied by MaxUpdates must pass before you can deal damage again.
        /// </summary>
        public abstract int NPCHitCDModifier { get; }

        /// <summary>
        /// called in SetDefaults, set things like width and height here. 
        /// </summary>
        public virtual void SafeSetDefaults() { }

        /// <summary>
        /// called in SetStaticDefaults, set other things here
        /// </summary>
        public virtual void SafeSetStaticDefaults() { }

        /// <summary>
        /// handles the drawing of the yoyo, and anything else. Everything is drawn in order. (glow => trails => the projectile itself)
        /// </summary>
        public abstract void DoDrawing_Behind(Color col);

        /// <summary>
        /// handles drawing things over the projectile. Again, things are drawn in order.
        /// </summary>
        public virtual void DoDrawing_Above(Color col) { }

        /// <summary>
        /// called in AI, handles anythingg you want the yoyo to do. 
        /// </summary>
        public virtual void YoyoAI() { }
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = LifeTime;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = MaxRange * 16f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = TopSpeed / MaxUpdates;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            SafeSetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = NPCHitCDModifier * MaxUpdates;
            SafeSetDefaults();
        }

        public override void AI()
        {
            YoyoAI();
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > (MaxRange * 10) * 16f) 
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DoDrawing_Behind(lightColor);
            return false; //vanilla drawing is yucky
        }

        public override void PostDraw(Color lightColor)
        {
            DoDrawing_Above(lightColor);
        }
    }
}
