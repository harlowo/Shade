using Microsoft.Xna.Framework;
using Shade.Content02.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Druid
{
    [AutoloadEquip(EquipType.Head)]

    public class DruidHead : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 3;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DruidBody>() && legs.type == ModContent.ItemType<DruidLegs>();
        }
        public override void UpdateEquip(Terraria.Player player)
        {
            player.statManaMax += 60;
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.statDefense += 3;
            player.GetModPlayer<DruidPlayer>().DruidSet = true;
            player.setBonus = this.GetLocalization("SetBonus").Value;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ModContent.ItemType<SoulOfEvergrowth>(), 3);
            recipe.AddTile(TileID.LivingLoom);
            recipe.Register();
        }
    }
    public class DruidPlayer : ModPlayer
    {
        public int combustDamage = 0;
        public bool removeDebuffs;
        public NPC npc;
        public float dustSpeed;
        public Vector2 dustDir;
        public Vector2 dustVelocity;
        public bool DruidSet = false;
        public override void ResetEffects()
        {
            DruidSet = false;
        }
        public override void PreUpdate()
        {
            if (removeDebuffs)
            {
                for (int i = 0; i < npc.buffType.Length; i++)
                {
                    if (npc.buffType[i] == BuffID.Frostburn || npc.buffType[i] == BuffID.OnFire)
                    {
                        if (npc.buffType[i] == BuffID.Frostburn)
                        {
                            combustDamage = (int)(npc.buffTime[i] / 4f + 12);
                        }
                        else if (npc.buffType[i] == BuffID.OnFire)
                        {
                            combustDamage = (int)(npc.buffTime[i] / 8f + 6);
                        }
                        npc.SimpleStrikeNPC(combustDamage, 0);
                        npc.DelBuff(i);
                        i = 0;
                    }
                }
                removeDebuffs = false;
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (DruidSet)
            {
                if (proj.type == ProjectileID.WandOfFrostingFrost)
                {
                    target.AddBuff(BuffID.Frostburn, 180);
                }
                else if (proj.type == ProjectileID.WandOfSparkingSpark) 
                {
                    target.AddBuff(BuffID.OnFire, 180);
                }
                if ((target.HasBuff(BuffID.OnFire) && proj.type == ProjectileID.WandOfFrostingFrost) || (target.HasBuff(BuffID.Frostburn) && (proj.type == ProjectileID.WandOfSparkingSpark || proj.type == ProjectileID.Flare)))
                {
                    for (int i = 0; i < 40; i++)
                    {
                        dustSpeed = Main.rand.Next(4, 10);
                        dustDir = (MathHelper.ToRadians(i * 40) + (MathHelper.ToRadians(Main.rand.Next(0, 40)) * Main.rand.Next(-1, 1))).ToRotationVector2();
                        dustVelocity = dustDir * dustSpeed;

                        int dust = Dust.NewDust(target.Center, 16, 16, DustID.Smoke, dustVelocity.X, dustVelocity.Y, 0, Color.White, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = Main.rand.Next(1, 50) / 10;
                        Main.dust[dust].rotation = dustSpeed * 2.25f;
                    }
                    npc = target;
                    removeDebuffs = true;
                }
            }
        }
    }
}