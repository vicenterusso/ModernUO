/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2023 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: PacketHandler.cs                                                *
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

public unsafe class PacketHandler
{
    private readonly int _length;

    public PacketHandler(int packetID, int length, bool ingame, delegate*<NetState, SpanReader, void> onReceive)
    {
        _length = length;
        PacketID = packetID;
        Ingame = ingame;
        OnReceive = onReceive;
    }

    public int PacketID { get; }

    public virtual int GetLength(NetState ns) => _length;

    public delegate*<NetState, SpanReader, void> OnReceive { get; }

    public delegate*<int, NetState, out bool, bool> ThrottleCallback { get; set; }

    public bool Ingame { get; }
}
