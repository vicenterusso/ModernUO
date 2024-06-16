/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2024 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: GumpImage.cs                                                    *
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

public class GumpImage : GumpEntry
{
    public GumpImage(int x, int y, int gumpID, int hue = 0, string cls = null)
    {
        X = x;
        Y = y;
        GumpID = gumpID;
        Hue = hue;
        Class = cls;
    }

    public int X { get; set; }

    public int Y { get; set; }

    public int GumpID { get; set; }

    public int Hue { get; set; }

    public string Class { get; set; }

    public override void AppendTo(ref SpanWriter writer, OrderedSet<string> strings, ref int entries, ref int switches)
    {
        var hasClass = !string.IsNullOrEmpty(Class);
        if (Hue != 0)
        {
            if (hasClass)
            {
                writer.WriteAscii($"{{ gumppic {X} {Y} {GumpID} hue={Hue} class={Class} }}");
            }
            else
            {
                writer.WriteAscii($"{{ gumppic {X} {Y} {GumpID} hue={Hue} }}");
            }
        }
        else if (hasClass)
        {
            writer.WriteAscii($"{{ gumppic {X} {Y} {GumpID} class={Class} }}");
        }
        else
        {
            writer.WriteAscii($"{{ gumppic {X} {Y} {GumpID} }}");
        }
    }
}
