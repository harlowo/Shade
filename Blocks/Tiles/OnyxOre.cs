using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Shade.Blocks.Tiles
{
    public class OnyxOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 660; 
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(17, 2, 51), name);

            DustType = 77;
            HitSound = SoundID.Tink;
            MineResist = 4f;
            MinPick = 206;
        }

 
        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.MediumPurple;
            return true;
        }
    }

    public class OnyxOreSystem : ModSystem
    {
        public static LocalizedText OnyxOrePassMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            OnyxOrePassMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(OnyxOrePassMessage)}"));
        }


        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {

            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (ShiniesIndex != -1)
            {

                tasks.Insert(ShiniesIndex + 1, new OnyxOrePass("Onyx Ores", 237.4298f));
            }
        }
    }

    public class OnyxOrePass : GenPass
    {
        public OnyxOrePass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {

            progress.Message = OnyxOreSystem.OnyxOrePassMessage.Value;


            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
            {

                int x = WorldGen.genRand.Next(0, Main.maxTilesX);


                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);


                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<OnyxOre>());

            }
        }
    }
}