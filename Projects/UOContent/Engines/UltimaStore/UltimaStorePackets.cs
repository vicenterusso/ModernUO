using System.Buffers;
using Server.Network;

namespace Server.Engines.UltimaStore
{
    public static class UltimaStorePackets
    {
        public static unsafe void Configure()
        {
            IncomingPackets.Register(0xFA, 1, true, &UltimaStoreOpenRequest);
        }

        public static void UltimaStoreOpenRequest(NetState state, SpanReader reader)
        {
            state.Mobile.SendMessage("Ultima Store is not currently available.");
        }
    }
}
