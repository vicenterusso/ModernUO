using System;
using System.Text.Json.Serialization;

namespace Server.Regions;

public class ArenaRegion : BaseRegion
{
    [JsonConstructor]
    public ArenaRegion(string name, Map map, int priority, params Rectangle3D[] area) : base(name, map, priority, area)
    {
    }

    public ArenaRegion(string name, Map map, Region parent, int priority, params Rectangle3D[] area) : base(name, map, parent, priority, area)
    {
    }

    public override bool YoungProtected => false;

    public Point3D Entrance { get; set; }

    public override bool AllowHousing(Mobile from, Point3D p) => false;

    public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
    {
        global = LightCycle.NightLevel;
    }

    public override bool CanUseStuckMenu(Mobile m) => Map != Map.Felucca && base.CanUseStuckMenu(m);

    public override void OnEnter(Mobile m)
    {
        base.OnEnter(m);
        Console.WriteLine("entering arena region " + Name);
    }

    public override void OnExit(Mobile m)
    {
        Console.WriteLine("exiting arena region " + Name);
    }
}
