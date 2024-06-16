using System.Text.Json.Serialization;
using Server.Network;
using Server.Spells;
using Server.Spells.Ninjitsu;

namespace Server.Regions;

public class TwistedWealdDesertRegion : MondainRegion
{
    [JsonConstructor] // Don't include parent, since it is special
    public TwistedWealdDesertRegion(string name, Map map, int priority, params Rectangle3D[] area) : base(name, map, priority, area)
    {
    }

    public TwistedWealdDesertRegion(string name, Map map, Region parent, params Rectangle3D[] area)
        : base(name, map, parent, area)
    {
    }

    public TwistedWealdDesertRegion(string name, Map map, Region parent, int priority, params Rectangle3D[] area)
        : base(name, map, parent, priority, area)
    {
    }

    public override void OnEnter(Mobile m)
    {
        var ns = m.NetState;
        if (ns != null && !TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)) &&
            m.AccessLevel == AccessLevel.Player)
        {
            ns.SendSpeedControl(SpeedControlSetting.Walk);
        }
    }

    public override void OnExit(Mobile m)
    {
        var ns = m.NetState;
        if (ns != null && !TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)))
        {
            ns.SendSpeedControl(SpeedControlSetting.Disable);
        }
    }

    public static void OnLogin(Mobile m)
    {
        if (m.Region.IsPartOf<TwistedWealdDesertRegion>() && m.AccessLevel == AccessLevel.Player)
        {
            m.NetState.SendSpeedControl(SpeedControlSetting.Walk);
        }
    }
}
