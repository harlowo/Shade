using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Shade.Common.Base
{
    //might not really be needed, but it makes things easier
    public abstract class BaseYoyoItem : ModItem
    {
        /// <summary>
        /// The yoyo projectile that this yoyo should shoot
        /// </summary>
        public abstract int YoyoProjToShoot { get; }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = YoyoProjToShoot;
            Item.autoReuse = true;
            SafeSetDefaults();
        }

        /// <summary>
        /// Modify things like damage, width/height, value, and rarity here
        /// </summary>
        public virtual void SafeSetDefaults() { }
    }
}
