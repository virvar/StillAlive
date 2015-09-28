using System;

namespace GameProcess.Actors.Server
{
    public interface IServerPacketHandler
    {
        void Receive(Virvar.Net.ClientPacket msg);
    }
}
