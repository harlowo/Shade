using Terraria.ModLoader;

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

    }
}
