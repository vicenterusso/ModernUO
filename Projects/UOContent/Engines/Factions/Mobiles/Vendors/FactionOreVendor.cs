using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Items;
using Server.Mobiles;

namespace Server.Factions;

[SerializationGenerator(0, false)]
public partial class FactionOreVendor : BaseFactionVendor
{
    public FactionOreVendor(Town town, Faction faction) : base(town, faction, "the Ore Man")
    {
        // NOTE: Skills verified
        SetSkill(SkillName.Carpentry, 85.0, 100.0);
        SetSkill(SkillName.Lumberjacking, 60.0, 83.0);
    }

    public override void InitSBInfo()
    {
        SBInfos.Add(new SBFactionOre());
    }

    public override void InitOutfit()
    {
        base.InitOutfit();

        AddItem(new HalfApron());
    }
}

public class SBFactionOre : SBInfo
{
    private static readonly object[] m_FixedSizeArgs = { true };

    public override IShopSellInfo SellInfo { get; } = new InternalSellInfo();

    public override List<GenericBuyInfo> BuyInfo { get; } = new InternalBuyInfo();

    public class InternalBuyInfo : List<GenericBuyInfo>
    {
        public InternalBuyInfo()
        {
            for (var i = 0; i < 5; ++i)
            {
                Add(new GenericBuyInfo(typeof(IronOre), 16, 20, 0x19B8, 0, m_FixedSizeArgs));
            }
        }
    }

    public class InternalSellInfo : GenericSellInfo;
}
