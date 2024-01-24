/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2023 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: ContainerGridPacketHandler.cs                                   *
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

namespace Server.Network;

public unsafe class ContainerGridPacketHandler : PacketHandler
{
    public ContainerGridPacketHandler(int packetID, int length, bool ingame,
        delegate*<NetState, SpanReader, void> onReceive)
        : base(packetID, length, ingame, onReceive)
    {
    }

    public override int GetLength(NetState ns) => base.GetLength(ns) + (ns.ContainerGridLines ? 1 : 0);
}
