using Shade.Content01.Items;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Shade.Common.Players
{
    public class OnyxLifePlayer : ModPlayer
    {
        public int LifeCorez;
        public int ManaCorez;

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            health.Base = LifeCorez * OnyxLifeModule.LifePerFruit;
            mana = StatModifier.Default;
            mana.Base = ManaCorez * OnyxLifeModule.ManaPerCrystal;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)LifeCorez);
            packet.Send(toWho, fromWho);
        }

        public void ReceivePlayerSync(BinaryReader reader)
        {
            LifeCorez = reader.ReadByte();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            OnyxLifePlayer clone = (OnyxLifePlayer)targetCopy;
            clone.LifeCorez = LifeCorez;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            OnyxLifePlayer clone = (OnyxLifePlayer)clientPlayer;

            if (LifeCorez != clone.LifeCorez)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["LifeCorez"] = LifeCorez;
        }

        public override void LoadData(TagCompound tag)
        {
            LifeCorez = tag.GetInt("LifeCorez");
        }
    }
}