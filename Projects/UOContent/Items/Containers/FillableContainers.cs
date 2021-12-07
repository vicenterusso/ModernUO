using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    [Serializable(2, false)]
    public abstract partial class FillableContainer : LockableContainer
    {
        [SerializableField(0)]
        protected FillableContentType _rawContentType;

        [TimerDrift]
        [SerializableField(1)]
        private Timer _respawnTimer;

        [DeserializeTimerField(1)]
        private void DeserializeRespawnTimer(TimeSpan delay)
        {
            if (delay > TimeSpan.MinValue)
            {
                _respawnTimer = Timer.DelayCall(delay, Respawn);
            }
        }

        public FillableContainer(int itemID) : base(itemID) => Movable = false;

        public virtual int MinRespawnMinutes => 60;
        public virtual int MaxRespawnMinutes => 90;

        public virtual bool IsLockable => true;
        public virtual bool IsTrappable => IsLockable;

        public virtual int SpawnThreshold => 2;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRespawnTime => _respawnTimer.Next;

        [CommandProperty(AccessLevel.GameMaster)]
        public FillableContentType ContentType
        {
            get => _rawContentType;
            set
            {
                ClearContents();
                _rawContentType = value;
            }
        }

        protected void ClearContents()
        {
            for (var i = Items.Count - 1; i >= 0; --i)
            {
                if (i < Items.Count)
                {
                    Items[i].Delete();
                }
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            AcquireContent();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            AcquireContent();
        }

        public virtual void AcquireContent()
        {
            if (_rawContentType != FillableContentType.None)
            {
                return;
            }

            _rawContentType = FillableContent.Acquire(GetWorldLocation(), Map);

            if (_rawContentType != FillableContentType.None)
            {
                Respawn();
            }
        }

        public override void OnItemRemoved(Item item)
        {
            CheckRespawn();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            _respawnTimer?.Stop();
            _respawnTimer = null;
        }

        public int GetItemsCount()
        {
            var count = 0;

            foreach (var item in Items)
            {
                count += item.Amount;
            }

            return count;
        }

        public void CheckRespawn()
        {
            var canSpawn = _rawContentType != FillableContentType.None && !Deleted && GetItemsCount() <= SpawnThreshold && !Movable &&
                           Parent == null && !IsLockedDown && !IsSecure;

            if (canSpawn)
            {
                if (_respawnTimer?.Running != true)
                {
                    var mins = Utility.RandomMinMax(MinRespawnMinutes, MaxRespawnMinutes);
                    var delay = TimeSpan.FromMinutes(mins);
                    _respawnTimer = Timer.DelayCall(delay, Respawn);
                }
            }
            else
            {
                _respawnTimer?.Stop();
                _respawnTimer = null;
            }
        }

        public void Respawn()
        {
            _respawnTimer?.Stop();
            _respawnTimer = null;

            if (_rawContentType != FillableContentType.None || Deleted)
            {
                return;
            }

            GenerateContent();

            var level = FillableContent.Lookup(_rawContentType).Level;

            if (IsLockable)
            {
                Locked = true;

                var difficulty = (level - 1) * 30;

                LockLevel = difficulty - 10;
                MaxLockLevel = difficulty + 30;
                RequiredSkill = difficulty;
            }

            if (IsTrappable && (level > 1 || Utility.Random(5) < 4))
            {
                if (level > Utility.Random(5))
                {
                    TrapType = TrapType.PoisonTrap;
                }
                else
                {
                    TrapType = TrapType.ExplosionTrap;
                }

                TrapPower = level * Utility.RandomMinMax(10, 30);
                TrapLevel = level;
            }
            else
            {
                TrapType = TrapType.None;
                TrapPower = 0;
                TrapLevel = 0;
            }

            CheckRespawn();
        }

        protected virtual int GetSpawnCount()
        {
            var itemsCount = GetItemsCount();

            if (itemsCount > SpawnThreshold)
            {
                return 0;
            }

            var maxSpawnCount = (1 + SpawnThreshold - itemsCount) * 2;

            return Utility.RandomMinMax(0, maxSpawnCount);
        }

        public virtual void GenerateContent()
        {
            if (_rawContentType != FillableContentType.None || Deleted)
            {
                return;
            }

            var content = FillableContent.Lookup(_rawContentType);

            var toSpawn = GetSpawnCount();

            for (var i = 0; i < toSpawn; ++i)
            {
                var item = content.Construct();

                if (item == null)
                {
                    continue;
                }

                var list = Items;

                for (var j = 0; j < list.Count; ++j)
                {
                    var subItem = list[j];

                    if (subItem is not Container && subItem.StackWith(null, item, false))
                    {
                        break;
                    }
                }

                if (!item.Deleted)
                {
                    DropItem(item);
                }
            }
        }

        private void Deserialize(IGenericReader reader, int version)
        {
            _rawContentType = (FillableContentType)reader.ReadInt();
            var respawnTimerNext = reader.ReadDeltaTime();
            DeserializeRespawnTimer(respawnTimerNext == DateTime.MinValue ? TimeSpan.MinValue : respawnTimerNext - Core.Now);
        }

        [AfterDeserialization]
        private void AfterDeserialization()
        {
            if (_respawnTimer?.Running != true)
            {
                CheckRespawn();
            }
        }
    }

    [Flippable(0xA97, 0xA99, 0xA98, 0xA9A, 0xA9B, 0xA9C)]
    [Serializable(0)]
    public partial class LibraryBookcase : FillableContainer
    {
        [Constructible]
        public LibraryBookcase() : base(0xA97) => Weight = 1.0;

        public override bool IsLockable => false;
        public override int SpawnThreshold => 5;

        protected override int GetSpawnCount() => 5 - GetItemsCount();

        public override void AcquireContent()
        {
            if (_rawContentType != FillableContentType.None)
            {
                return;
            }

            _rawContentType = FillableContentType.Library;
            Respawn();
        }
    }

    [Flippable(0xE3D, 0xE3C)]
    [Serializable(0)]
    public partial class FillableLargeCrate : FillableContainer
    {
        [Constructible]
        public FillableLargeCrate() : base(0xE3D) => Weight = 1.0;
    }

    [Flippable(0x9A9, 0xE7E)]
    [Serializable(0)]
    public partial class FillableSmallCrate : FillableContainer
    {
        [Constructible]
        public FillableSmallCrate() : base(0x9A9) => Weight = 1.0;
    }

    [Flippable(0x9AA, 0xE7D)]
    [Serializable(0)]
    public partial class FillableWoodenBox : FillableContainer
    {
        [Constructible]
        public FillableWoodenBox() : base(0x9AA) => Weight = 4.0;
    }

    [Flippable(0x9A8, 0xE80)]
    [Serializable(0)]
    public partial class FillableMetalBox : FillableContainer
    {
        [Constructible]
        public FillableMetalBox() : base(0x9A8)
        {
        }
    }

    [Serializable(0)]
    public partial class FillableBarrel : FillableContainer
    {
        [Constructible]
        public FillableBarrel() : base(0xE77)
        {
        }
    }

    [Flippable(0x9AB, 0xE7C)]
    [Serializable(0, false)]
    public partial class FillableMetalChest : FillableContainer
    {
        [Constructible]
        public FillableMetalChest() : base(0x9AB)
        {
        }
    }

    [Flippable(0xE41, 0xE40)]
    [Serializable(0, false)]
    public partial class FillableMetalGoldenChest : FillableContainer
    {
        [Constructible]
        public FillableMetalGoldenChest()
            : base(0xE41)
        {
        }
    }

    [Flippable(0xE43, 0xE42)]
    [Serializable(0, false)]
    public partial class FillableWoodenChest : FillableContainer
    {
        [Constructible]
        public FillableWoodenChest()
            : base(0xE43)
        {
        }
    }

    public class FillableEntry
    {
        protected Type[] m_Types;
        protected int m_Weight;

        public FillableEntry(Type type)
            : this(1, new[] { type })
        {
        }

        public FillableEntry(int weight, Type type)
            : this(weight, new[] { type })
        {
        }

        public FillableEntry(Type[] types)
            : this(1, types)
        {
        }

        public FillableEntry(int weight, Type[] types)
        {
            m_Weight = weight;
            m_Types = types;
        }

        public FillableEntry(int weight, Type[] types, int offset, int count)
        {
            m_Weight = weight;
            m_Types = new Type[count];

            for (var i = 0; i < m_Types.Length; ++i)
            {
                m_Types[i] = types[offset + i];
            }
        }

        public Type[] Types => m_Types;
        public int Weight => m_Weight;

        public virtual Item Construct()
        {
            var item = Loot.Construct(m_Types);

            if (item is Key key)
            {
                key.ItemID = Utility.RandomList(
                    (int)KeyType.Copper,
                    (int)KeyType.Gold,
                    (int)KeyType.Iron,
                    (int)KeyType.Rusty
                );
            }
            else if (item is Arrow || item is Bolt)
            {
                item.Amount = Utility.RandomMinMax(2, 6);
            }
            else if (item is Bandage || item is Lockpick)
            {
                item.Amount = Utility.RandomMinMax(1, 3);
            }

            return item;
        }
    }

    public class FillableBvrge : FillableEntry
    {
        public FillableBvrge(Type type, BeverageType content) : this(1, type, content)
        {
        }

        public FillableBvrge(int weight, Type type, BeverageType content)
            : base(weight, type) =>
            Content = content;

        public BeverageType Content { get; }

        public override Item Construct()
        {
            Item item;

            var index = Utility.Random(m_Types.Length);

            if (m_Types[index] == typeof(BeverageBottle))
            {
                item = new BeverageBottle(Content);
            }
            else if (m_Types[index] == typeof(Jug))
            {
                item = new Jug(Content);
            }
            else
            {
                item = base.Construct();

                if (item is BaseBeverage bev)
                {
                    bev.Content = Content;
                    bev.Quantity = bev.MaxQuantity;
                }
            }

            return item;
        }
    }

    public enum FillableContentType
    {
        None = -1,
        Weaponsmith,
        Provisioner,
        Mage,
        Alchemist,
        Armorer,
        ArtisanGuild,
        Baker,
        Bard,
        Blacksmith,
        Bowyer,
        Butcher,
        Carpenter,
        Clothier,
        Cobbler,
        Docks,
        Farm,
        FighterGuild,
        Guard,
        Healer,
        Herbalist,
        Inn,
        Jeweler,
        Library,
        Merchant,
        Mill,
        Mine,
        Observatory,
        Painter,
        Ranger,
        Stables,
        Tanner,
        Tavern,
        ThiefGuild,
        Tinker,
        Veterinarian
    }

    public class FillableContent
    {
        private static readonly FillableContent Alchemist = new(
            1,
            new[]
            {
                typeof(Alchemist)
            },
            new[]
            {
                new FillableEntry(typeof(NightSightPotion)),
                new FillableEntry(typeof(LesserCurePotion)),
                new FillableEntry(typeof(AgilityPotion)),
                new FillableEntry(typeof(StrengthPotion)),
                new FillableEntry(typeof(LesserPoisonPotion)),
                new FillableEntry(typeof(RefreshPotion)),
                new FillableEntry(typeof(LesserHealPotion)),
                new FillableEntry(typeof(LesserExplosionPotion)),
                new FillableEntry(typeof(MortarPestle))
            }
        );

        private static readonly FillableContent Armorer = new(
            2,
            new[]
            {
                typeof(Armorer)
            },
            new[]
            {
                new FillableEntry(2, typeof(ChainCoif)),
                new FillableEntry(1, typeof(PlateGorget)),
                new FillableEntry(1, typeof(BronzeShield)),
                new FillableEntry(1, typeof(Buckler)),
                new FillableEntry(2, typeof(MetalKiteShield)),
                new FillableEntry(2, typeof(HeaterShield)),
                new FillableEntry(1, typeof(WoodenShield)),
                new FillableEntry(1, typeof(MetalShield))
            }
        );

        private static readonly FillableContent ArtisanGuild = new(
            1,
            Array.Empty<Type>(),
            new[]
            {
                new FillableEntry(1, typeof(PaintsAndBrush)),
                new FillableEntry(1, typeof(SledgeHammer)),
                new FillableEntry(2, typeof(SmithHammer)),
                new FillableEntry(2, typeof(Tongs)),
                new FillableEntry(4, typeof(Lockpick)),
                new FillableEntry(4, typeof(TinkerTools)),
                new FillableEntry(1, typeof(MalletAndChisel)),
                new FillableEntry(1, typeof(StatueEast2)),
                new FillableEntry(1, typeof(StatueSouth)),
                new FillableEntry(1, typeof(StatueSouthEast)),
                new FillableEntry(1, typeof(StatueWest)),
                new FillableEntry(1, typeof(StatueNorth)),
                new FillableEntry(1, typeof(StatueEast)),
                new FillableEntry(1, typeof(BustEast)),
                new FillableEntry(1, typeof(BustSouth)),
                new FillableEntry(1, typeof(BearMask)),
                new FillableEntry(1, typeof(DeerMask)),
                new FillableEntry(4, typeof(OrcHelm)),
                new FillableEntry(1, typeof(TribalMask)),
                new FillableEntry(1, typeof(HornedTribalMask))
            }
        );

        private static readonly FillableContent Baker = new(
            1,
            new[]
            {
                typeof(Baker)
            },
            new[]
            {
                new FillableEntry(1, typeof(RollingPin)),
                new FillableEntry(2, typeof(SackFlour)),
                new FillableEntry(2, typeof(BreadLoaf)),
                new FillableEntry(1, typeof(FrenchBread))
            }
        );

        private static readonly FillableContent Bard = new(
            1,
            new[]
            {
                typeof(Bard),
                typeof(BardGuildmaster)
            },
            new[]
            {
                new FillableEntry(1, typeof(LapHarp)),
                new FillableEntry(2, typeof(Lute)),
                new FillableEntry(1, typeof(Drums)),
                new FillableEntry(1, typeof(Tambourine)),
                new FillableEntry(1, typeof(TambourineTassel))
            }
        );

        private static readonly FillableContent Blacksmith = new(
            2,
            new[]
            {
                typeof(Blacksmith),
                typeof(BlacksmithGuildmaster)
            },
            new[]
            {
                new FillableEntry(8, typeof(SmithHammer)),
                new FillableEntry(8, typeof(Tongs)),
                new FillableEntry(8, typeof(SledgeHammer)),
                // new FillableEntry( 8, typeof( IronOre ) ), TODO: Smaller ore
                new FillableEntry(8, typeof(IronIngot)),
                new FillableEntry(1, typeof(IronWire)),
                new FillableEntry(1, typeof(SilverWire)),
                new FillableEntry(1, typeof(GoldWire)),
                new FillableEntry(1, typeof(CopperWire)),
                new FillableEntry(1, typeof(HorseShoes)),
                new FillableEntry(1, typeof(ForgedMetal))
            }
        );

        private static readonly FillableContent Bowyer = new(
            2,
            new[]
            {
                typeof(Bowyer)
            },
            new[]
            {
                new FillableEntry(2, typeof(Bow)),
                new FillableEntry(2, typeof(Crossbow)),
                new FillableEntry(1, typeof(Arrow))
            }
        );

        private static readonly FillableContent Butcher = new(
            1,
            new[]
            {
                typeof(Butcher)
            },
            new[]
            {
                new FillableEntry(2, typeof(Cleaver)),
                new FillableEntry(2, typeof(SlabOfBacon)),
                new FillableEntry(2, typeof(Bacon)),
                new FillableEntry(1, typeof(RawFishSteak)),
                new FillableEntry(1, typeof(FishSteak)),
                new FillableEntry(2, typeof(CookedBird)),
                new FillableEntry(2, typeof(RawBird)),
                new FillableEntry(2, typeof(Ham)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(LambLeg)),
                new FillableEntry(1, typeof(Ribs)),
                new FillableEntry(1, typeof(RawRibs)),
                new FillableEntry(2, typeof(Sausage)),
                new FillableEntry(1, typeof(RawChickenLeg)),
                new FillableEntry(1, typeof(ChickenLeg))
            }
        );

        private static readonly FillableContent Carpenter = new(
            1,
            new[]
            {
                typeof(Carpenter),
                typeof(Architect),
                typeof(RealEstateBroker)
            },
            new[]
            {
                new FillableEntry(1, typeof(ChiselsNorth)),
                new FillableEntry(1, typeof(ChiselsWest)),
                new FillableEntry(2, typeof(DovetailSaw)),
                new FillableEntry(2, typeof(Hammer)),
                new FillableEntry(2, typeof(MouldingPlane)),
                new FillableEntry(2, typeof(Nails)),
                new FillableEntry(2, typeof(JointingPlane)),
                new FillableEntry(2, typeof(SmoothingPlane)),
                new FillableEntry(2, typeof(Saw)),
                new FillableEntry(2, typeof(DrawKnife)),
                new FillableEntry(1, typeof(Log)),
                new FillableEntry(1, typeof(Froe)),
                new FillableEntry(1, typeof(Inshave)),
                new FillableEntry(1, typeof(Scorp))
            }
        );

        private static readonly FillableContent Clothier = new(
            1,
            new[]
            {
                typeof(Tailor),
                typeof(Weaver),
                typeof(TailorGuildmaster)
            },
            new[]
            {
                new FillableEntry(1, typeof(Cotton)),
                new FillableEntry(1, typeof(Wool)),
                new FillableEntry(1, typeof(DarkYarn)),
                new FillableEntry(1, typeof(LightYarn)),
                new FillableEntry(1, typeof(LightYarnUnraveled)),
                new FillableEntry(1, typeof(SpoolOfThread)),
                // Four different types
                // new FillableEntry( 1, typeof( FoldedCloth ) ),
                // new FillableEntry( 1, typeof( FoldedCloth ) ),
                // new FillableEntry( 1, typeof( FoldedCloth ) ),
                // new FillableEntry( 1, typeof( FoldedCloth ) ),
                new FillableEntry(1, typeof(Dyes)),
                new FillableEntry(2, typeof(Leather))
            }
        );

        private static readonly FillableContent Cobbler = new(
            1,
            new[]
            {
                typeof(Cobbler)
            },
            new[]
            {
                new FillableEntry(1, typeof(Boots)),
                new FillableEntry(2, typeof(Shoes)),
                new FillableEntry(2, typeof(Sandals)),
                new FillableEntry(1, typeof(ThighBoots))
            }
        );

        private static readonly FillableContent Docks = new(
            1,
            new[]
            {
                typeof(Fisherman),
                typeof(FisherGuildmaster)
            },
            new[]
            {
                new FillableEntry(1, typeof(FishingPole)),
                // Two different types
                // new FillableEntry( 1, typeof( SmallFish ) ),
                // new FillableEntry( 1, typeof( SmallFish ) ),
                new FillableEntry(4, typeof(Fish))
            }
        );

        private static readonly FillableContent Farm = new(
            1,
            new[]
            {
                typeof(Farmer),
                typeof(Rancher)
            },
            new[]
            {
                new FillableEntry(1, typeof(Shirt)),
                new FillableEntry(1, typeof(ShortPants)),
                new FillableEntry(1, typeof(Skirt)),
                new FillableEntry(1, typeof(PlainDress)),
                new FillableEntry(1, typeof(Cap)),
                new FillableEntry(2, typeof(Sandals)),
                new FillableEntry(2, typeof(GnarledStaff)),
                new FillableEntry(2, typeof(Pitchfork)),
                new FillableEntry(1, typeof(Bag)),
                new FillableEntry(1, typeof(Kindling)),
                new FillableEntry(1, typeof(Lettuce)),
                new FillableEntry(1, typeof(Onion)),
                new FillableEntry(1, typeof(Turnip)),
                new FillableEntry(1, typeof(Ham)),
                new FillableEntry(1, typeof(Bacon)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(SheafOfHay)),
                new FillableBvrge(1, typeof(Pitcher), BeverageType.Milk)
            }
        );

        private static readonly FillableContent FighterGuild = new(
            3,
            new[]
            {
                typeof(WarriorGuildmaster)
            },
            new[]
            {
                new FillableEntry(12, Loot.ArmorTypes),
                new FillableEntry(8, Loot.WeaponTypes),
                new FillableEntry(3, Loot.ShieldTypes),
                new FillableEntry(1, typeof(Arrow))
            }
        );

        private static readonly FillableContent Guard = new(
            3,
            Array.Empty<Type>(),
            new[]
            {
                new FillableEntry(12, Loot.ArmorTypes),
                new FillableEntry(8, Loot.WeaponTypes),
                new FillableEntry(3, Loot.ShieldTypes),
                new FillableEntry(1, typeof(Arrow))
            }
        );

        private static readonly FillableContent Healer = new(
            1,
            new[]
            {
                typeof(Healer),
                typeof(HealerGuildmaster)
            },
            new[]
            {
                new FillableEntry(1, typeof(Bandage)),
                new FillableEntry(1, typeof(MortarPestle)),
                new FillableEntry(1, typeof(LesserHealPotion))
            }
        );

        private static readonly FillableContent Herbalist = new(
            1,
            new[]
            {
                typeof(Herbalist)
            },
            new[]
            {
                new FillableEntry(10, typeof(Garlic)),
                new FillableEntry(10, typeof(Ginseng)),
                new FillableEntry(10, typeof(MandrakeRoot)),
                new FillableEntry(1, typeof(DeadWood)),
                new FillableEntry(1, typeof(WhiteDriedFlowers)),
                new FillableEntry(1, typeof(GreenDriedFlowers)),
                new FillableEntry(1, typeof(DriedOnions)),
                new FillableEntry(1, typeof(DriedHerbs))
            }
        );

        private static readonly FillableContent Inn = new(
            1,
            Array.Empty<Type>(),
            new[]
            {
                new FillableEntry(1, typeof(Candle)),
                new FillableEntry(1, typeof(Torch)),
                new FillableEntry(1, typeof(Lantern))
            }
        );

        private static readonly FillableContent Jeweler = new(
            2,
            new[]
            {
                typeof(Jeweler)
            },
            new[]
            {
                new FillableEntry(1, typeof(GoldRing)),
                new FillableEntry(1, typeof(GoldBracelet)),
                new FillableEntry(1, typeof(GoldEarrings)),
                new FillableEntry(1, typeof(GoldNecklace)),
                new FillableEntry(1, typeof(GoldBeadNecklace)),
                new FillableEntry(1, typeof(Necklace)),
                new FillableEntry(1, typeof(Beads)),
                new FillableEntry(9, Loot.GemTypes)
            }
        );

        private static readonly FillableContent Library = new(
            1,
            new[]
            {
                typeof(Scribe)
            },
            new[]
            {
                new FillableEntry(8, Loot.LibraryBookTypes),
                new FillableEntry(1, typeof(RedBook)),
                new FillableEntry(1, typeof(BlueBook))
            }
        );

        private static readonly FillableContent Mage = new(
            2,
            new[]
            {
                typeof(Mage),
                typeof(HolyMage),
                typeof(MageGuildmaster)
            },
            new[]
            {
                new FillableEntry(16, typeof(BlankScroll)),
                new FillableEntry(14, typeof(Spellbook)),
                new FillableEntry(12, Loot.RegularScrollTypes, 0, 8),
                new FillableEntry(11, Loot.RegularScrollTypes, 8, 8),
                new FillableEntry(10, Loot.RegularScrollTypes, 16, 8),
                new FillableEntry(9, Loot.RegularScrollTypes, 24, 8),
                new FillableEntry(8, Loot.RegularScrollTypes, 32, 8),
                new FillableEntry(7, Loot.RegularScrollTypes, 40, 8),
                new FillableEntry(6, Loot.RegularScrollTypes, 48, 8),
                new FillableEntry(5, Loot.RegularScrollTypes, 56, 8)
            }
        );

        private static readonly FillableContent Merchant = new(
            1,
            new[]
            {
                typeof(MerchantGuildmaster)
            },
            new[]
            {
                new FillableEntry(1, typeof(CheeseWheel)),
                new FillableEntry(1, typeof(CheeseWedge)),
                new FillableEntry(1, typeof(CheeseSlice)),
                new FillableEntry(1, typeof(Eggs)),
                new FillableEntry(4, typeof(Fish)),
                new FillableEntry(2, typeof(RawFishSteak)),
                new FillableEntry(2, typeof(FishSteak)),
                new FillableEntry(1, typeof(Apple)),
                new FillableEntry(2, typeof(Banana)),
                new FillableEntry(2, typeof(Bananas)),
                new FillableEntry(2, typeof(OpenCoconut)),
                new FillableEntry(1, typeof(SplitCoconut)),
                new FillableEntry(1, typeof(Coconut)),
                new FillableEntry(1, typeof(Dates)),
                new FillableEntry(1, typeof(Grapes)),
                new FillableEntry(1, typeof(Lemon)),
                new FillableEntry(1, typeof(Lemons)),
                new FillableEntry(1, typeof(Lime)),
                new FillableEntry(1, typeof(Limes)),
                new FillableEntry(1, typeof(Peach)),
                new FillableEntry(1, typeof(Pear)),
                new FillableEntry(2, typeof(SlabOfBacon)),
                new FillableEntry(2, typeof(Bacon)),
                new FillableEntry(2, typeof(CookedBird)),
                new FillableEntry(2, typeof(RawBird)),
                new FillableEntry(2, typeof(Ham)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(LambLeg)),
                new FillableEntry(1, typeof(Ribs)),
                new FillableEntry(1, typeof(RawRibs)),
                new FillableEntry(2, typeof(Sausage)),
                new FillableEntry(1, typeof(RawChickenLeg)),
                new FillableEntry(1, typeof(ChickenLeg)),
                new FillableEntry(1, typeof(Watermelon)),
                new FillableEntry(1, typeof(SmallWatermelon)),
                new FillableEntry(3, typeof(Turnip)),
                new FillableEntry(2, typeof(YellowGourd)),
                new FillableEntry(2, typeof(GreenGourd)),
                new FillableEntry(2, typeof(Pumpkin)),
                new FillableEntry(1, typeof(SmallPumpkin)),
                new FillableEntry(2, typeof(Onion)),
                new FillableEntry(2, typeof(Lettuce)),
                new FillableEntry(2, typeof(Squash)),
                new FillableEntry(2, typeof(HoneydewMelon)),
                new FillableEntry(1, typeof(Carrot)),
                new FillableEntry(2, typeof(Cantaloupe)),
                new FillableEntry(2, typeof(Cabbage)),
                new FillableEntry(4, typeof(EarOfCorn))
            }
        );

        private static readonly FillableContent Mill = new(
            1,
            Array.Empty<Type>(),
            new[]
            {
                new FillableEntry(1, typeof(SackFlour))
            }
        );

        private static readonly FillableContent Mine = new(
            1,
            new[]
            {
                typeof(Miner)
            },
            new[]
            {
                new FillableEntry(2, typeof(Pickaxe)),
                new FillableEntry(2, typeof(Shovel)),
                new FillableEntry(2, typeof(IronIngot)),
                // new FillableEntry( 2, typeof( IronOre ) ),	TODO: Smaller Ore
                new FillableEntry(1, typeof(ForgedMetal))
            }
        );

        private static readonly FillableContent Observatory = new(
            1,
            Array.Empty<Type>(),
            new[]
            {
                new FillableEntry(2, typeof(Sextant)),
                new FillableEntry(2, typeof(Clock)),
                new FillableEntry(1, typeof(Spyglass))
            }
        );

        private static readonly FillableContent Painter = new(
            1,
            Array.Empty<Type>(),
            new[]
            {
                new FillableEntry(1, typeof(PaintsAndBrush)),
                new FillableEntry(2, typeof(PenAndInk))
            }
        );

        private static readonly FillableContent Provisioner = new(
            1,
            new[]
            {
                typeof(Provisioner)
            },
            new[]
            {
                new FillableEntry(1, typeof(CheeseWheel)),
                new FillableEntry(1, typeof(CheeseWedge)),
                new FillableEntry(1, typeof(CheeseSlice)),
                new FillableEntry(1, typeof(Eggs)),
                new FillableEntry(4, typeof(Fish)),
                new FillableEntry(1, typeof(DirtyFrypan)),
                new FillableEntry(1, typeof(DirtyPan)),
                new FillableEntry(1, typeof(DirtyKettle)),
                new FillableEntry(1, typeof(DirtySmallRoundPot)),
                new FillableEntry(1, typeof(DirtyRoundPot)),
                new FillableEntry(1, typeof(DirtySmallPot)),
                new FillableEntry(1, typeof(DirtyPot)),
                new FillableEntry(1, typeof(Apple)),
                new FillableEntry(2, typeof(Banana)),
                new FillableEntry(2, typeof(Bananas)),
                new FillableEntry(2, typeof(OpenCoconut)),
                new FillableEntry(1, typeof(SplitCoconut)),
                new FillableEntry(1, typeof(Coconut)),
                new FillableEntry(1, typeof(Dates)),
                new FillableEntry(1, typeof(Grapes)),
                new FillableEntry(1, typeof(Lemon)),
                new FillableEntry(1, typeof(Lemons)),
                new FillableEntry(1, typeof(Lime)),
                new FillableEntry(1, typeof(Limes)),
                new FillableEntry(1, typeof(Peach)),
                new FillableEntry(1, typeof(Pear)),
                new FillableEntry(2, typeof(SlabOfBacon)),
                new FillableEntry(2, typeof(Bacon)),
                new FillableEntry(1, typeof(RawFishSteak)),
                new FillableEntry(1, typeof(FishSteak)),
                new FillableEntry(2, typeof(CookedBird)),
                new FillableEntry(2, typeof(RawBird)),
                new FillableEntry(2, typeof(Ham)),
                new FillableEntry(1, typeof(RawLambLeg)),
                new FillableEntry(1, typeof(LambLeg)),
                new FillableEntry(1, typeof(Ribs)),
                new FillableEntry(1, typeof(RawRibs)),
                new FillableEntry(2, typeof(Sausage)),
                new FillableEntry(1, typeof(RawChickenLeg)),
                new FillableEntry(1, typeof(ChickenLeg)),
                new FillableEntry(1, typeof(Watermelon)),
                new FillableEntry(1, typeof(SmallWatermelon)),
                new FillableEntry(3, typeof(Turnip)),
                new FillableEntry(2, typeof(YellowGourd)),
                new FillableEntry(2, typeof(GreenGourd)),
                new FillableEntry(2, typeof(Pumpkin)),
                new FillableEntry(1, typeof(SmallPumpkin)),
                new FillableEntry(2, typeof(Onion)),
                new FillableEntry(2, typeof(Lettuce)),
                new FillableEntry(2, typeof(Squash)),
                new FillableEntry(2, typeof(HoneydewMelon)),
                new FillableEntry(1, typeof(Carrot)),
                new FillableEntry(2, typeof(Cantaloupe)),
                new FillableEntry(2, typeof(Cabbage)),
                new FillableEntry(4, typeof(EarOfCorn))
            }
        );

        private static readonly FillableContent Ranger = new(
            2,
            new[]
            {
                typeof(Ranger),
                typeof(RangerGuildmaster)
            },
            new[]
            {
                new FillableEntry(2, typeof(StuddedChest)),
                new FillableEntry(2, typeof(StuddedLegs)),
                new FillableEntry(2, typeof(StuddedArms)),
                new FillableEntry(2, typeof(StuddedGloves)),
                new FillableEntry(1, typeof(StuddedGorget)),

                new FillableEntry(2, typeof(LeatherChest)),
                new FillableEntry(2, typeof(LeatherLegs)),
                new FillableEntry(2, typeof(LeatherArms)),
                new FillableEntry(2, typeof(LeatherGloves)),
                new FillableEntry(1, typeof(LeatherGorget)),

                new FillableEntry(2, typeof(FeatheredHat)),
                new FillableEntry(1, typeof(CloseHelm)),
                new FillableEntry(1, typeof(TallStrawHat)),
                new FillableEntry(1, typeof(Bandana)),
                new FillableEntry(1, typeof(Cloak)),
                new FillableEntry(2, typeof(Boots)),
                new FillableEntry(2, typeof(ThighBoots)),

                new FillableEntry(2, typeof(GnarledStaff)),
                new FillableEntry(1, typeof(Whip)),

                new FillableEntry(2, typeof(Bow)),
                new FillableEntry(2, typeof(Crossbow)),
                new FillableEntry(2, typeof(HeavyCrossbow)),
                new FillableEntry(4, typeof(Arrow))
            }
        );

        private static readonly FillableContent Stables = new(
            1,
            new[]
            {
                typeof(AnimalTrainer),
                typeof(GypsyAnimalTrainer)
            },
            new[]
            {
                // new FillableEntry( 1, typeof( Wheat ) ),
                new FillableEntry(1, typeof(Carrot))
            }
        );

        private static readonly FillableContent Tanner = new(
            2,
            new[]
            {
                typeof(Tanner),
                typeof(LeatherWorker),
                typeof(Furtrader)
            },
            new[]
            {
                new FillableEntry(1, typeof(FeatheredHat)),
                new FillableEntry(1, typeof(LeatherArms)),
                new FillableEntry(2, typeof(LeatherLegs)),
                new FillableEntry(2, typeof(LeatherChest)),
                new FillableEntry(2, typeof(LeatherGloves)),
                new FillableEntry(1, typeof(LeatherGorget)),
                new FillableEntry(2, typeof(Leather))
            }
        );

        private static readonly FillableContent Tavern = new(
            1,
            new[]
            {
                typeof(TavernKeeper),
                typeof(Barkeeper),
                typeof(Waiter),
                typeof(Cook)
            },
            new FillableEntry[]
            {
                new FillableBvrge(1, typeof(BeverageBottle), BeverageType.Ale),
                new FillableBvrge(1, typeof(BeverageBottle), BeverageType.Wine),
                new FillableBvrge(1, typeof(BeverageBottle), BeverageType.Liquor),
                new FillableBvrge(1, typeof(Jug), BeverageType.Cider)
            }
        );

        private static readonly FillableContent ThiefGuild = new(
            1,
            new[]
            {
                typeof(Thief),
                typeof(ThiefGuildmaster)
            },
            new[]
            {
                new FillableEntry(1, typeof(Lockpick)),
                new FillableEntry(1, typeof(BearMask)),
                new FillableEntry(1, typeof(DeerMask)),
                new FillableEntry(1, typeof(TribalMask)),
                new FillableEntry(1, typeof(HornedTribalMask)),
                new FillableEntry(4, typeof(OrcHelm))
            }
        );

        private static readonly FillableContent Tinker = new(
            1,
            new[]
            {
                typeof(Tinker),
                typeof(TinkerGuildmaster)
            },
            new[]
            {
                new FillableEntry(1, typeof(Lockpick)),
                // new FillableEntry( 1, typeof( KeyRing ) ),
                new FillableEntry(2, typeof(Clock)),
                new FillableEntry(2, typeof(ClockParts)),
                new FillableEntry(2, typeof(AxleGears)),
                new FillableEntry(2, typeof(Gears)),
                new FillableEntry(2, typeof(Hinge)),
                // new FillableEntry( 1, typeof( ArrowShafts ) ),
                new FillableEntry(2, typeof(Sextant)),
                new FillableEntry(2, typeof(SextantParts)),
                new FillableEntry(2, typeof(Axle)),
                new FillableEntry(2, typeof(Springs)),
                new FillableEntry(5, typeof(TinkerTools)),
                new FillableEntry(4, typeof(Key)),
                new FillableEntry(1, typeof(DecoArrowShafts)),
                new FillableEntry(1, typeof(Lockpicks)),
                new FillableEntry(1, typeof(ToolKit))
            }
        );

        private static readonly FillableContent Veterinarian = new(
            1,
            new[]
            {
                typeof(Veterinarian)
            },
            new[]
            {
                new FillableEntry(1, typeof(Bandage)),
                new FillableEntry(1, typeof(MortarPestle)),
                new FillableEntry(1, typeof(LesserHealPotion)),
                // new FillableEntry( 1, typeof( Wheat ) ),
                new FillableEntry(1, typeof(Carrot))
            }
        );

        private static readonly FillableContent Weaponsmith = new(
            2,
            new[]
            {
                typeof(Weaponsmith)
            },
            new[]
            {
                new FillableEntry(8, Loot.WeaponTypes),
                new FillableEntry(1, typeof(Arrow))
            }
        );

        private static Dictionary<Type, FillableContentType> _acquireTable;

        private static readonly FillableContent[] ContentTypes =
        {
            Weaponsmith, Provisioner, Mage,
            Alchemist, Armorer, ArtisanGuild,
            Baker, Bard, Blacksmith,
            Bowyer, Butcher, Carpenter,
            Clothier, Cobbler, Docks,
            Farm, FighterGuild, Guard,
            Healer, Herbalist, Inn,
            Jeweler, Library, Merchant,
            Mill, Mine, Observatory,
            Painter, Ranger, Stables,
            Tanner, Tavern, ThiefGuild,
            Tinker, Veterinarian
        };

        private readonly FillableEntry[] m_Entries;
        private readonly int m_Weight;

        public FillableContent(int level, Type[] vendors, FillableEntry[] entries)
        {
            Level = level;
            Vendors = vendors;
            m_Entries = entries;

            for (var i = 0; i < entries.Length; ++i)
            {
                m_Weight += entries[i].Weight;
            }
        }

        public int Level { get; }

        public Type[] Vendors { get; }

        public FillableContentType TypeID => Lookup(this);

        public virtual Item Construct()
        {
            var index = Utility.Random(m_Weight);

            for (var i = 0; i < m_Entries.Length; ++i)
            {
                var entry = m_Entries[i];

                if (index < entry.Weight)
                {
                    return entry.Construct();
                }

                index -= entry.Weight;
            }

            return null;
        }

        private static FillableContent Lookup(FillableContentType type)
        {
            var v = (int)type;

            if (v >= 0 && v < ContentTypes.Length)
            {
                return ContentTypes[v];
            }

            return null;
        }

        public static FillableContentType Lookup(FillableContent content)
        {
            if (content == null)
            {
                return FillableContentType.None;
            }

            return (FillableContentType)Array.IndexOf(ContentTypes, content);
        }

        public static FillableContentType Acquire(Point3D loc, Map map)
        {
            FillableContentType content = FillableContentType.None;

            if (map == null || map == Map.Internal)
            {
                return content;
            }

            if (_acquireTable == null)
            {
                _acquireTable = new Dictionary<Type, FillableContentType>();

                for (var i = 0; i < ContentTypes.Length; ++i)
                {
                    var fill = ContentTypes[i];

                    for (var j = 0; j < fill.Vendors.Length; ++j)
                    {
                        _acquireTable[fill.Vendors[j]] = fill.TypeID;
                    }
                }
            }

            Mobile nearest = null;

            foreach (var mob in map.GetMobilesInRange(loc, 20))
            {
                if (nearest != null && mob.GetDistanceToSqrt(loc) > nearest.GetDistanceToSqrt(loc) &&
                    !(nearest is Cobbler && mob is Provisioner))
                {
                    continue;
                }

                if (_acquireTable.TryGetValue(mob.GetType(), out var check))
                {
                    nearest = mob;
                    content = check;
                }
            }

            return content;
        }
    }
}
