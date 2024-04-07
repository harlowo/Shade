using Terraria.ModLoader;

namespace Shade
{
    public class Shade : Mod
    {
        public static Shade Instance;
        public Shade() => Instance = this;
        //this is needed, why wasnt it merged?
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
