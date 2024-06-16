using System;
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class MarkContainer : LockableContainer
{
    [TimerDrift]
    [SerializableField(1, getter: "private", setter: "private")]
    private InternalTimer _relockTimer;

    [DeserializeTimerField(1)]
    private void DeserializeRelockTimer(TimeSpan delay)
    {
        if (!Locked && _autoLock)
        {
            _relockTimer = new InternalTimer(this, delay);
        }
    }

    [SerializableField(2)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private Map _targetMap;

    [SerializableField(3)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private Point3D _target;

    [SerializableField(4)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private string _description;

    [Constructible]
    public MarkContainer(bool bone = false, bool locked = false) : base(bone ? 0xECA : 0xE79)
    {
        Movable = false;

        if (bone)
        {
            Hue = 1102;
        }

        _autoLock = locked;
        Locked = locked;

        if (locked)
        {
            LockLevel = ILockpickable.MagicLock;
        }
    }

    [SerializableProperty(0)]
    [CommandProperty(AccessLevel.GameMaster)]
    public bool AutoLock
    {
        get => _autoLock;
        set
        {
            _autoLock = value;

            if (!_autoLock)
            {
                StopTimer();
            }
            else if (!Locked)
            {
                _relockTimer ??= new InternalTimer(this);
            }
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public bool Bone
    {
        get => ItemID == 0xECA;
        set
        {
            ItemID = value ? 0xECA : 0xE79;
            Hue = value ? 1102 : 0;
        }
    }

    public override bool IsDecoContainer => false;

    [CommandProperty(AccessLevel.GameMaster)]
    public override bool Locked
    {
        get => base.Locked;
        set
        {
            base.Locked = value;

            if (_autoLock)
            {
                StopTimer();

                if (!Locked)
                {
                    _relockTimer = new InternalTimer(this);
                }
            }
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public DateTime NextRelock => _relockTimer.Next;

    public static void Configure()
    {
        CommandSystem.Register("SecretLocGen", AccessLevel.Developer, SecretLocGen_OnCommand);
    }

    [Usage("SecretLocGen")]
    [Description("Generates mark containers to Malas secret locations.")]
    public static void SecretLocGen_OnCommand(CommandEventArgs e)
    {
        CreateMalasPassage(951, 546, -70, 1006, 994, -70, false, false);
        CreateMalasPassage(914, 192, -79, 1019, 1062, -70, false, false);
        CreateMalasPassage(1614, 143, -90, 1214, 1313, -90, false, false);
        CreateMalasPassage(2176, 324, -90, 1554, 172, -90, false, false);
        CreateMalasPassage(864, 812, -90, 1061, 1161, -70, false, false);
        CreateMalasPassage(1051, 1434, -85, 1076, 1244, -70, false, true);
        CreateMalasPassage(1326, 523, -87, 1201, 1554, -70, false, false);
        CreateMalasPassage(424, 189, -1, 2333, 1501, -90, true, false);
        CreateMalasPassage(1313, 1115, -85, 1183, 462, -45, false, false);

        e.Mobile.SendMessage("Secret mark containers have been created.");
    }

    private static bool FindMarkContainer(Point3D p, Map map)
    {
        foreach (var item in map.GetItemsAt<MarkContainer>(p))
        {
            if (item.Z == p.Z)
            {
                return true;
            }
        }

        return false;
    }

    private static void CreateMalasPassage(
        int x, int y, int z, int xTarget, int yTarget, int zTarget, bool bone, bool locked
    )
    {
        var location = new Point3D(x, y, z);

        if (FindMarkContainer(location, Map.Malas))
        {
            return;
        }

        var cont = new MarkContainer(bone, locked)
        {
            TargetMap = Map.Malas,
            Target = new Point3D(xTarget, yTarget, zTarget),
            Description = "strange location"
        };

        cont.MoveToWorld(location, Map.Malas);
    }

    public void StopTimer()
    {
        _relockTimer?.Stop();
        _relockTimer = null;
    }

    public void Mark(RecallRune rune)
    {
        if (_targetMap != null)
        {
            rune.Marked = true;
            rune.TargetMap = _targetMap;
            rune.Target = _target;
            rune.Description = _description;
            rune.House = null;
        }
    }

    public override bool OnDragDrop(Mobile from, Item dropped)
    {
        if (dropped is RecallRune rune && base.OnDragDrop(from, dropped))
        {
            Mark(rune);
            return true;
        }

        return false;
    }

    public override bool OnDragDropInto(Mobile from, Item dropped, Point3D p)
    {
        if (dropped is RecallRune rune && base.OnDragDropInto(from, dropped, p))
        {
            Mark(rune);
            return true;
        }

        return false;
    }

    private class InternalTimer : Timer
    {
        public InternalTimer(MarkContainer container) : this(container, TimeSpan.FromMinutes(5.0))
        {
        }

        public InternalTimer(MarkContainer container, TimeSpan delay) : base(delay)
        {
            Container = container;

            Start();
        }

        public MarkContainer Container { get; }

        protected override void OnTick()
        {
            Container.Locked = true;
            Container.LockLevel = ILockpickable.MagicLock;
        }
    }
}
