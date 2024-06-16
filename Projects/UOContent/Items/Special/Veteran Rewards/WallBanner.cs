using ModernUO.Serialization;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Network;

namespace Server.Items;

[SerializationGenerator(0)]
public partial class WallBannerComponent : AddonComponent, IDyable
{
    public WallBannerComponent(int itemID) : base(itemID)
    {
    }

    public override bool NeedsWall => true;
    public override Point3D WallPosition => East ? new Point3D(-1, 0, 0) : new Point3D(0, -1, 0);

    public bool East => ((WallBanner)Addon).East;

    public bool Dye(Mobile from, DyeTub sender)
    {
        if (Deleted)
        {
            return false;
        }

        if (Addon != null)
        {
            Addon.Hue = sender.DyedHue;
        }

        return true;
    }
}

[SerializationGenerator(0)]
public partial class WallBanner : BaseAddon, IRewardItem
{
    [InvalidateProperties]
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private bool _east;

    [InvalidateProperties]
    [SerializableField(1)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private bool _isRewardItem;

    [Constructible]
    public WallBanner(int bannerID)
    {
        switch (bannerID)
        {
            case 1:
                {
                    AddComponent(new WallBannerComponent(0x161F), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x161E), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x161D), 0, 2, 0);
                    break;
                }
            case 2:
                {
                    AddComponent(new WallBannerComponent(0x1586), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1587), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x1588), 2, 0, 0);
                    break;
                }
            case 3:
                {
                    AddComponent(new WallBannerComponent(0x1622), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1621), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1620), 0, 2, 0);
                    break;
                }
            case 4:
                {
                    AddComponent(new WallBannerComponent(0x1589), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x158A), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158B), 2, 0, 0);
                    break;
                }
            case 5:
                {
                    AddComponent(new WallBannerComponent(0x1625), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1624), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1623), 0, 2, 0);
                    break;
                }
            case 6:
                {
                    AddComponent(new WallBannerComponent(0x158C), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x158D), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158E), 2, 0, 0);
                    break;
                }
            case 7:
                {
                    AddComponent(new WallBannerComponent(0x1628), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1627), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1626), 0, 2, 0);
                    break;
                }
            case 8:
                {
                    AddComponent(new WallBannerComponent(0x1590), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1591), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158F), 2, 0, 0);
                    break;
                }
            case 9:
                {
                    AddComponent(new WallBannerComponent(0x162A), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1629), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x1626), 0, 2, 0);
                    break;
                }
            case 10:
                {
                    AddComponent(new WallBannerComponent(0x1592), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1593), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x158F), 2, 0, 0);
                    break;
                }
            case 11:
                {
                    AddComponent(new WallBannerComponent(0x162D), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x162C), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x162B), 0, 2, 0);
                    break;
                }
            case 12:
                {
                    AddComponent(new WallBannerComponent(0x1594), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1595), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x1596), 2, 0, 0);
                    break;
                }
            case 13:
                {
                    AddComponent(new WallBannerComponent(0x1632), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1631), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x162E), 0, 2, 0);
                    break;
                }
            case 14:
                {
                    AddComponent(new WallBannerComponent(0x1598), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x159B), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x159C), 2, 0, 0);
                    break;
                }
            case 15:
                {
                    AddComponent(new WallBannerComponent(0x1633), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1630), 0, 1, 0);
                    AddComponent(new WallBannerComponent(0x162F), 0, 2, 0);
                    break;
                }
            case 16:
                {
                    AddComponent(new WallBannerComponent(0x1599), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x159A), 1, 0, 0);
                    AddComponent(new WallBannerComponent(0x159D), 2, 0, 0);
                    break;
                }

            case 17:
                {
                    AddComponent(new WallBannerComponent(0x1610), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x160F), 0, 1, 0);
                    break;
                }
            case 18:
                {
                    AddComponent(new WallBannerComponent(0x15A0), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A1), 1, 0, 0);
                    break;
                }

            case 19:
                {
                    AddComponent(new WallBannerComponent(0x1612), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1611), 0, 1, 0);
                    break;
                }
            case 20:
                {
                    AddComponent(new WallBannerComponent(0x15A2), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A3), 1, 0, 0);
                    break;
                }

            case 21:
                {
                    AddComponent(new WallBannerComponent(0x1614), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1613), 0, 1, 0);
                    break;
                }
            case 22:
                {
                    AddComponent(new WallBannerComponent(0x15A4), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A5), 1, 0, 0);
                    break;
                }

            case 23:
                {
                    AddComponent(new WallBannerComponent(0x1616), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1615), 0, 1, 0);
                    break;
                }
            case 24:
                {
                    AddComponent(new WallBannerComponent(0x15A6), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A7), 1, 0, 0);
                    break;
                }

            case 25:
                {
                    AddComponent(new WallBannerComponent(0x1618), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1617), 0, 1, 0);
                    break;
                }
            case 26:
                {
                    AddComponent(new WallBannerComponent(0x15A8), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15A9), 1, 0, 0);
                    break;
                }

            case 27:
                {
                    AddComponent(new WallBannerComponent(0x161A), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x1619), 0, 1, 0);
                    break;
                }
            case 28:
                {
                    AddComponent(new WallBannerComponent(0x15AA), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15AB), 1, 0, 0);
                    break;
                }

            case 29:
                {
                    AddComponent(new WallBannerComponent(0x161C), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x161B), 0, 1, 0);
                    break;
                }
            case 30:
                {
                    AddComponent(new WallBannerComponent(0x15AC), 0, 0, 0);
                    AddComponent(new WallBannerComponent(0x15AD), 1, 0, 0);
                    break;
                }
        }
    }

    public override BaseAddonDeed Deed =>
        new WallBannerDeed
        {
            IsRewardItem = _isRewardItem
        };
}

[SerializationGenerator(0)]
public partial class WallBannerDeed : BaseAddonDeed, IRewardItem
{
    [InvalidateProperties]
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private bool _isRewardItem;

    private int _bannerId;

    [Constructible]
    public WallBannerDeed() => LootType = LootType.Blessed;

    public override int LabelNumber => 1080549; // Wall Banner Deed

    public override BaseAddon Addon =>
        new WallBanner(_bannerId)
        {
            IsRewardItem = _isRewardItem
        };

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        if (_isRewardItem)
        {
            list.Add(1076225); // 9th Year Veteran Reward
        }
    }

    public override void OnDoubleClick(Mobile from)
    {
        if (_isRewardItem && !RewardSystem.CheckIsUsableBy(from, this))
        {
            return;
        }

        if (IsChildOf(from.Backpack))
        {
            from.CloseGump<InternalGump>();
            from.SendGump(new InternalGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
        }
    }

    public void Use(Mobile m, int bannerID)
    {
        _bannerId = bannerID;

        base.OnDoubleClick(m);
    }

    private class InternalGump : Gump
    {
        private readonly WallBannerDeed _wallBanner;

        public InternalGump(WallBannerDeed wallBanner) : base(150, 50)
        {
            _wallBanner = wallBanner;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddBackground(25, 0, 500, 265, 0xA28);
            AddLabel(70, 12, 0x3E3, "Choose a Wall Banner:");

            AddPage(1);

            AddItem(55, 110, 0x161D);
            AddItem(75, 90, 0x161E);
            AddItem(95, 70, 0x161F);
            AddButton(70, 50, 0x845, 0x846, 1);
            AddItem(105, 70, 0x1586);
            AddItem(125, 90, 0x1587);
            AddItem(145, 110, 0x1588);
            AddButton(145, 50, 0x845, 0x846, 2);
            AddItem(200, 110, 0x1620);
            AddItem(220, 90, 0x1621);
            AddItem(240, 70, 0x1622);
            AddButton(220, 50, 0x845, 0x846, 3);
            AddItem(250, 70, 0x1589);
            AddItem(270, 90, 0x158A);
            AddItem(290, 110, 0x158B);
            AddButton(300, 50, 0x845, 0x846, 4);
            AddItem(350, 110, 0x1623);
            AddItem(370, 90, 0x1624);
            AddItem(390, 70, 0x1625);
            AddButton(365, 50, 0x845, 0x846, 5);
            AddItem(400, 70, 0x158C);
            AddItem(420, 90, 0x158D);
            AddItem(440, 110, 0x158E);
            AddButton(445, 50, 0x845, 0x846, 6);
            AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 2);

            AddPage(2);

            AddItem(52, 110, 0x1626);
            AddItem(72, 90, 0x1627);
            AddItem(95, 70, 0x1628);
            AddButton(70, 50, 0x845, 0x846, 7);
            AddItem(105, 70, 0x1590);
            AddItem(125, 90, 0x1591);
            AddItem(145, 110, 0x158F);
            AddButton(145, 50, 0x845, 0x846, 8);
            AddItem(197, 110, 0x1626);
            AddItem(217, 90, 0x1629);
            AddItem(240, 70, 0x162A);
            AddButton(220, 50, 0x845, 0x846, 9);
            AddItem(250, 70, 0x1592);
            AddItem(270, 90, 0x1593);
            AddItem(290, 110, 0x158F);
            AddButton(300, 50, 0x845, 0x846, 10);
            AddItem(340, 110, 0x162B);
            AddItem(363, 90, 0x162C);
            AddItem(385, 70, 0x162D);
            AddButton(365, 50, 0x845, 0x846, 11);
            AddItem(395, 70, 0x1594);
            AddItem(417, 90, 0x1595);
            AddItem(439, 111, 0x1596);
            AddButton(445, 50, 0x845, 0x846, 12);
            AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 1);
            AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 3);

            AddPage(3);

            AddItem(55, 110, 0x162E);
            AddItem(75, 93, 0x1631);
            AddItem(95, 70, 0x1632);
            AddButton(70, 50, 0x845, 0x846, 13);
            AddItem(118, 70, 0x1598);
            AddItem(138, 94, 0x159B);
            AddItem(159, 113, 0x159C);
            AddButton(160, 50, 0x845, 0x846, 14);
            AddItem(219, 111, 0x162F);
            AddItem(238, 94, 0x1630);
            AddItem(258, 70, 0x1633);
            AddButton(240, 50, 0x845, 0x846, 15);
            AddItem(279, 70, 0x1599);
            AddItem(298, 93, 0x159A);
            AddItem(319, 113, 0x159D);
            AddButton(320, 50, 0x845, 0x846, 16);
            AddItem(380, 90, 0x160F);
            AddItem(400, 70, 0x1610);
            AddButton(390, 50, 0x845, 0x846, 17);
            AddItem(420, 70, 0x15A0);
            AddItem(440, 90, 0x15A1);
            AddButton(455, 50, 0x845, 0x846, 18);
            AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 2);
            AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 4);

            AddPage(4);

            AddItem(55, 90, 0x1611);
            AddItem(75, 70, 0x1612);
            AddButton(70, 50, 0x845, 0x846, 19);
            AddItem(105, 70, 0x15A2);
            AddItem(125, 90, 0x15A3);
            AddButton(145, 50, 0x845, 0x846, 20);
            AddItem(200, 84, 0x1613);
            AddItem(220, 70, 0x1614);
            AddButton(215, 50, 0x845, 0x846, 21);
            AddItem(250, 70, 0x15A4);
            AddItem(270, 84, 0x15A5);
            AddButton(290, 50, 0x845, 0x846, 22);
            AddItem(350, 90, 0x1615);
            AddItem(370, 70, 0x1616);
            AddButton(365, 50, 0x845, 0x846, 23);
            AddItem(400, 70, 0x15A6);
            AddItem(420, 90, 0x15A7);
            AddButton(445, 50, 0x845, 0x846, 24);
            AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 3);
            AddButton(455, 205, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 5);

            AddPage(5);

            AddItem(55, 90, 0x1617);
            AddItem(77, 70, 0x1618);
            AddButton(70, 50, 0x845, 0x846, 25);
            AddItem(105, 70, 0x15A8);
            AddItem(127, 90, 0x15A9);
            AddButton(145, 50, 0x845, 0x846, 26);
            AddItem(200, 90, 0x1619);
            AddItem(222, 70, 0x161A);
            AddButton(220, 50, 0x845, 0x846, 27);
            AddItem(250, 70, 0x15AA);
            AddItem(272, 90, 0x15AB);
            AddButton(300, 50, 0x845, 0x846, 28);
            AddItem(350, 90, 0x161B);
            AddItem(372, 70, 0x161C);
            AddButton(365, 50, 0x845, 0x846, 29);
            AddItem(400, 70, 0x15AC);
            AddItem(422, 90, 0x15AD);
            AddButton(445, 50, 0x845, 0x846, 30);
            AddButton(70, 205, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 4);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (_wallBanner?.Deleted != false || info.ButtonID is <= 0 or >= 31)
            {
                return;
            }

            _wallBanner.Use(sender.Mobile, info.ButtonID);
        }
    }
}
