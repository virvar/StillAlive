using System;

namespace GameProcess.Actors.Client
{
    public interface IClientPacketHandler
    {
        void Receive(Virvar.Net.ServerPacket msg);
    }
}
