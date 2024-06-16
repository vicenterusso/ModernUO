/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2024 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: GumpGroup.cs                                                    *
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
using Server.Collections;

namespace Server.Gumps;

public class GumpGroup : GumpEntry
{
    public GumpGroup(int group) => Group = group;

    public int Group { get; set; }

    public override void AppendTo(ref SpanWriter writer, OrderedSet<string> strings, ref int entries, ref int switches)
    {
        if (Group == 1)
        {
            writer.Write("{ group 1 }"u8);
        }
        else
        {
            writer.WriteAscii($"{{ group {Group} }}");
        }
    }
}
