/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2023 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: FreeshardProtocol.cs                                            *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System.Buffers;
using System.Runtime.CompilerServices;

namespace Server.Network
{
    public static class FreeshardProtocol
    {
        private static PacketHandler[] _handlers;

        [CallPriority(10)]
        public static void Configure()
        {
            _handlers = ProtocolExtensions<FreeshardProtocolInfo>.Register(new FreeshardProtocolInfo());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Register(int cmd, bool ingame, delegate*<NetState, SpanReader, void> onReceive) =>
            Register(cmd, ingame, false, onReceive);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Register(
            int cmd, bool ingame, bool outgame, delegate*<NetState, SpanReader, void> onReceive
        ) => _handlers[cmd] = new PacketHandler(cmd, onReceive, inGameOnly: ingame, outGameOnly: outgame);

        private struct FreeshardProtocolInfo : IProtocolExtensionsInfo
        {
            public int PacketId => 0xF1;
        }
    }
}
