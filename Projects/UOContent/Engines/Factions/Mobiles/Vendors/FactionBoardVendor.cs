using System.Collections.Generic;
using ModernUO.Serialization;
using Server.Items;
using Server.Mobiles;

namespace Server.Factions;

[SerializationGenerator(0, false)]
public partial class FactionBoardVendor : BaseFactionVendor
{
    public FactionBoardVendor(Town town, Faction faction) :
        base(town, faction, "the LumberMan") // NOTE: title inconsistent, as OSI
    {
        SetSkill(SkillName.Carpentry, 85.0, 100.0);
        SetSkill(SkillName.Lumberjacking, 60.0, 83.0);
    }

    public override void InitSBInfo()
    {
        SBInfos.Add(new SBFactionBoard());
    }

    public override void InitOutfit()
    {
        base.InitOutfit();

        AddItem(new HalfApron());
    }
}

public class SBFactionBoard : SBInfo
{
    public override IShopSellInfo SellInfo { get; } = new InternalSellInfo();

    public override List<GenericBuyInfo> BuyInfo { get; } = new InternalBuyInfo();

    public class InternalBuyInfo : List<GenericBuyInfo>
    {
        public InternalBuyInfo()
        {
            for (var i = 0; i < 5; ++i)
            {
                Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
            }
        }
    }

    public class InternalSellInfo : GenericSellInfo;
}
