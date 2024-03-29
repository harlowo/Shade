using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Shade.Content02.Items.Equipables.Armor.Marble
{
    [AutoloadEquip(EquipType.Head)]

    public class MarbleHead : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 5;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MarbleBody>() && legs.type == ModContent.ItemType<MarbleLegs>();
        }
        public override void UpdateArmorSet(Terraria.Player player)
        {
            MarblePlayer modPlayer = player.GetModPlayer<MarblePlayer>();
            if (modPlayer.parrying)
            {
                player.moveSpeed *= 0.5f;
                player.accRunSpeed = 0;
            }
            modPlayer.set = true;
            player.setBonus = this.GetLocalization("SetBonus").Value;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    public class MarblePlayer : ModPlayer
    {
        public int cooldown = 0;
        public bool set = false;
        public int projID = -1;
        public Projectile ownedProjectile;
        public bool parrying = false;
        public override void ResetEffects()
        {
            set = false;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (set)
            {
                if (cooldown > 0)
                {
                    cooldown--;
                }
                else
                {
                    if (triggersSet.MouseRight)
                    {
                        cooldown = 60;
                        if (parrying == false)
                        {
                            parrying = true;
                            int spirit = Projectile.NewProjectile(Player.GetSource_FromThis(),
                                Player.position + (Vector2.UnitX * (Player.width * 2) * Player.direction),
                                velocity: new(),
                                ModContent.ProjectileType<MarbleSpirit>(),
                                0,
                                0,
                                Player.whoAmI);
                            ownedProjectile = Main.projectile[spirit];
                            ownedProjectile.spriteDirection = Player.direction;
                            ownedProjectile.ai[0] = 1;
                        }
                    }
                    if (triggersSet.MouseMiddle)
                    {
                        cooldown = 60;
                        int spirit = Projectile.NewProjectile(Player.GetSource_FromThis(),
                                Player.position + (Vector2.UnitX * (Player.width * 2) * -Player.direction),
                                new(),
                                ModContent.ProjectileType<MarbleSpirit>(),
                                0,
                                0,
                                Player.whoAmI);
                        ownedProjectile = Main.projectile[spirit];
                        ownedProjectile.ai[0] = 2;
                    }
                }
                if (parrying && !triggersSet.MouseRight)
                {
                    parrying = false;
                    ownedProjectile.Kill();
                }
            }
        }
        public override void PostUpdate()
        {
            if (ownedProjectile != null)
            {
                Main.NewText(ownedProjectile.active);
                if (parrying)
                    Player.direction = ownedProjectile.spriteDirection;
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (parrying)
            {
                modifiers.FinalDamage *= 0.001f;
            }
        }
    }
}
