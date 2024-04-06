using System.IO;
using Terraria;
using Terraria.ModLoader;
using NetEasy;
namespace Shade
{
    public class Shade : Mod
    {
        internal static Shade Instance; //renamed to internal
        //much better to do it like this
        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
        
        public override void PostSetupContent()
        {
            NetEasy.NetEasy.Register(this);
        }
        
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetEasy.NetEasy.HandleModule(reader, whoAmI);
        }
    }
}
