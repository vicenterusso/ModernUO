using System;
using Server.Engines.BulkOrders;
using Server.Items;
using Server.Spells;
using Server.Utilities;

namespace Server.Engines.Craft;

public class DefInscription : CraftSystem
{
    private static readonly Type typeofSpellScroll = typeof(SpellScroll);

    private static readonly Type[] regTypes =
    {
        typeof(BlackPearl),
        typeof(Bloodmoss),
        typeof(Garlic),
        typeof(Ginseng),
        typeof(MandrakeRoot),
        typeof(Nightshade),
        typeof(SulfurousAsh),
        typeof(SpidersSilk)
    };

    private int _circle, _mana;
    private int _index;

    public static void Initialize()
    {
        CraftSystem = new DefInscription();
    }

    private DefInscription() : base(1, 1, 1.25)
    {
    }

    public override SkillName MainSkill => SkillName.Inscribe;

    public override TextDefinition GumpTitle { get; } = 1044009;

    public static CraftSystem CraftSystem { get; private set; }

    public override double GetChanceAtMin(CraftItem item) => 0.0;

    public override int CanCraft(Mobile from, BaseTool tool, Type typeItem)
    {
        if (tool?.Deleted != false || tool.UsesRemaining < 0)
        {
            return 1044038; // You have worn out your tool!
        }

        if (!BaseTool.CheckAccessible(tool, from))
        {
            return 1044263; // The tool must be on your person to use.
        }

        var scroll = typeItem?.CreateEntityInstance<SpellScroll>();

        if (scroll != null)
        {
            var hasSpell = Spellbook.Find(from, scroll.SpellID)?.HasSpell(scroll.SpellID) == true;

            scroll.Delete();

            return hasSpell ? 0 : 1042404; // null : You don't have that spell!
        }

        return 0;
    }

    public override void PlayCraftEffect(Mobile from)
    {
        from.PlaySound(0x249);
    }

    public override int PlayEndingEffect(
        Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality,
        bool makersMark, CraftItem item
    )
    {
        if (toolBroken)
        {
            from.SendLocalizedMessage(1044038); // You have worn out your tool
        }

        if (!typeofSpellScroll.IsAssignableFrom(item.ItemType)) // not a scroll
        {
            if (failed)
            {
                if (lostMaterial)
                {
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                }

                return 1044157; // You failed to create the item, but no materials were lost.
            }

            if (quality == 0)
            {
                return 502785; // You were barely able to make this item.  It's quality is below average.
            }

            if (makersMark && quality == 2)
            {
                return 1044156; // You create an exceptional quality item and affix your maker's mark.
            }

            if (quality == 2)
            {
                return 1044155; // You create an exceptional quality item.
            }

            return 1044154; // You create the item.
        }

        if (failed)
        {
            return 501630; // You fail to inscribe the scroll, and the scroll is ruined.
        }

        return 501629; // You inscribe the spell and put the scroll in your backpack.
    }

    private void AddSpell(Type type, params Reg[] regs)
    {
        double minSkill, maxSkill;

        switch (_circle)
        {
            default:
                {
                    minSkill = -25.0;
                    maxSkill = 25.0;
                    break;
                }
            case 1:
                {
                    minSkill = -10.8;
                    maxSkill = 39.2;
                    break;
                }
            case 2:
                {
                    minSkill = 03.5;
                    maxSkill = 53.5;
                    break;
                }
            case 3:
                {
                    minSkill = 17.8;
                    maxSkill = 67.8;
                    break;
                }
            case 4:
                {
                    minSkill = 32.1;
                    maxSkill = 82.1;
                    break;
                }
            case 5:
                {
                    minSkill = 46.4;
                    maxSkill = 96.4;
                    break;
                }
            case 6:
                {
                    minSkill = 60.7;
                    maxSkill = 110.7;
                    break;
                }
            case 7:
                {
                    minSkill = 75.0;
                    maxSkill = 125.0;
                    break;
                }
        }

        var index = AddCraft(
            type,
            1044369 + _circle,
            1044381 + _index++,
            minSkill,
            maxSkill,
            regTypes[(int)regs[0]],
            1044353 + (int)regs[0],
            1,
            1044361 + (int)regs[0]
        );

        for (var i = 1; i < regs.Length; ++i)
        {
            AddRes(index, regTypes[(int)regs[i]], 1044353 + (int)regs[i], 1, 1044361 + (int)regs[i]);
        }

        AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

        SetManaReq(index, _mana, 1042403);
    }

    private void AddNecroSpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
    {
        var index = AddCraft(
            type,
            1061677,
            1060509 + spell,
            minSkill,
            minSkill + 1.0, // Yes, on OSI it's only 1.0 skill diff'.  Don't blame me, blame OSI.
            regs[0],
            null,
            1,
            501627
        );

        for (var i = 1; i < regs.Length; ++i)
        {
            AddRes(index, regs[i], null, 1, 501627);
        }

        AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);
        SetManaReq(index, mana, 1042403);
    }

    private void AddMysticismSpell(int spell, int mana, double minSkill, double maxSkill, Type type, params Type[] regs)
    {
        var index = AddCraft(
            type,
            1111671,
            1031678 + spell,
            minSkill,
            maxSkill,
            regs[0],
            null,
            1,
            501627
        );

        for (var i = 1; i < regs.Length; ++i)
        {
            AddRes(index, regs[i], null, 1, 501627);
        }

        AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);
        SetManaReq(index, mana, 1042403);
    }

    public override void InitCraftList()
    {
        _circle = 0;
        _mana = 4;

        AddSpell(typeof(ReactiveArmorScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(ClumsyScroll), Reg.Bloodmoss, Reg.Nightshade);
        AddSpell(typeof(CreateFoodScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
        AddSpell(typeof(FeeblemindScroll), Reg.Nightshade, Reg.Ginseng);
        AddSpell(typeof(HealScroll), Reg.Garlic, Reg.Ginseng, Reg.SpidersSilk);
        AddSpell(typeof(MagicArrowScroll), Reg.SulfurousAsh);
        AddSpell(typeof(NightSightScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(WeakenScroll), Reg.Garlic, Reg.Nightshade);

        _circle = 1;
        _mana = 6;

        AddSpell(typeof(AgilityScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
        AddSpell(typeof(CunningScroll), Reg.Nightshade, Reg.MandrakeRoot);
        AddSpell(typeof(CureScroll), Reg.Garlic, Reg.Ginseng);
        AddSpell(typeof(HarmScroll), Reg.Nightshade, Reg.SpidersSilk);
        AddSpell(typeof(MagicTrapScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(MagicUnTrapScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
        AddSpell(typeof(ProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.SulfurousAsh);
        AddSpell(typeof(StrengthScroll), Reg.Nightshade, Reg.MandrakeRoot);

        _circle = 2;
        _mana = 9;

        AddSpell(typeof(BlessScroll), Reg.Garlic, Reg.MandrakeRoot);
        AddSpell(typeof(FireballScroll), Reg.BlackPearl);
        AddSpell(typeof(MagicLockScroll), Reg.Bloodmoss, Reg.Garlic, Reg.SulfurousAsh);
        AddSpell(typeof(PoisonScroll), Reg.Nightshade);
        AddSpell(typeof(TelekinesisScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
        AddSpell(typeof(TeleportScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
        AddSpell(typeof(UnlockScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
        AddSpell(typeof(WallOfStoneScroll), Reg.Bloodmoss, Reg.Garlic);

        _circle = 3;
        _mana = 11;

        AddSpell(typeof(ArchCureScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
        AddSpell(typeof(ArchProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot, Reg.SulfurousAsh);
        AddSpell(typeof(CurseScroll), Reg.Garlic, Reg.Nightshade, Reg.SulfurousAsh);
        AddSpell(typeof(FireFieldScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(GreaterHealScroll), Reg.Garlic, Reg.SpidersSilk, Reg.MandrakeRoot, Reg.Ginseng);
        AddSpell(typeof(LightningScroll), Reg.MandrakeRoot, Reg.SulfurousAsh);
        AddSpell(typeof(ManaDrainScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.MandrakeRoot);
        AddSpell(typeof(RecallScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot);

        _circle = 4;
        _mana = 14;

        AddSpell(typeof(BladeSpiritsScroll), Reg.BlackPearl, Reg.Nightshade, Reg.MandrakeRoot);
        AddSpell(typeof(DispelFieldScroll), Reg.BlackPearl, Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(IncognitoScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Nightshade);
        AddSpell(typeof(MagicReflectScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
        AddSpell(typeof(MindBlastScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
        AddSpell(typeof(ParalyzeScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
        AddSpell(typeof(PoisonFieldScroll), Reg.BlackPearl, Reg.Nightshade, Reg.SpidersSilk);
        AddSpell(typeof(SummonCreatureScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

        _circle = 5;
        _mana = 20;

        AddSpell(typeof(DispelScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
        AddSpell(typeof(EnergyBoltScroll), Reg.BlackPearl, Reg.Nightshade);
        AddSpell(typeof(ExplosionScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
        AddSpell(typeof(InvisibilityScroll), Reg.Bloodmoss, Reg.Nightshade);
        AddSpell(typeof(MarkScroll), Reg.Bloodmoss, Reg.BlackPearl, Reg.MandrakeRoot);
        AddSpell(typeof(MassCurseScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
        AddSpell(typeof(ParalyzeFieldScroll), Reg.BlackPearl, Reg.Ginseng, Reg.SpidersSilk);
        AddSpell(typeof(RevealScroll), Reg.Bloodmoss, Reg.SulfurousAsh);

        _circle = 6;
        _mana = 40;

        AddSpell(typeof(ChainLightningScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh);
        AddSpell(typeof(EnergyFieldScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(FlamestrikeScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(GateTravelScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SulfurousAsh);
        AddSpell(typeof(ManaVampireScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
        AddSpell(typeof(MassDispelScroll), Reg.BlackPearl, Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
        AddSpell(typeof(MeteorSwarmScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh, Reg.SpidersSilk);
        AddSpell(typeof(PolymorphScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

        _circle = 7;
        _mana = 50;

        AddSpell(typeof(EarthquakeScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Ginseng, Reg.SulfurousAsh);
        AddSpell(typeof(EnergyVortexScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Nightshade);
        AddSpell(typeof(ResurrectionScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Ginseng);
        AddSpell(typeof(SummonAirElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
        AddSpell(typeof(SummonDaemonScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(SummonEarthElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
        AddSpell(typeof(SummonFireElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
        AddSpell(typeof(SummonWaterElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

        if (Core.SE)
        {
            AddNecroSpell(0, 23, 39.6, typeof(AnimateDeadScroll), Reagent.GraveDust, Reagent.DaemonBlood);
            AddNecroSpell(1, 13, 19.6, typeof(BloodOathScroll), Reagent.DaemonBlood);
            AddNecroSpell(2, 11, 19.6, typeof(CorpseSkinScroll), Reagent.BatWing, Reagent.GraveDust);
            AddNecroSpell(3, 7, 19.6, typeof(CurseWeaponScroll), Reagent.PigIron);
            AddNecroSpell(4, 11, 19.6, typeof(EvilOmenScroll), Reagent.BatWing, Reagent.NoxCrystal);
            AddNecroSpell(5, 11, 39.6, typeof(HorrificBeastScroll), Reagent.BatWing, Reagent.DaemonBlood);
            AddNecroSpell(
                6,
                23,
                69.6,
                typeof(LichFormScroll),
                Reagent.GraveDust,
                Reagent.DaemonBlood,
                Reagent.NoxCrystal
            );
            AddNecroSpell(7, 17, 29.6, typeof(MindRotScroll), Reagent.BatWing, Reagent.DaemonBlood, Reagent.PigIron);
            AddNecroSpell(8, 5, 19.6, typeof(PainSpikeScroll), Reagent.GraveDust, Reagent.PigIron);
            AddNecroSpell(9, 17, 49.6, typeof(PoisonStrikeScroll), Reagent.NoxCrystal);
            AddNecroSpell(10, 29, 64.6, typeof(StrangleScroll), Reagent.DaemonBlood, Reagent.NoxCrystal);
            AddNecroSpell(
                11,
                17,
                29.6,
                typeof(SummonFamiliarScroll),
                Reagent.BatWing,
                Reagent.GraveDust,
                Reagent.DaemonBlood
            );
            AddNecroSpell(
                12,
                23,
                98.6,
                typeof(VampiricEmbraceScroll),
                Reagent.BatWing,
                Reagent.NoxCrystal,
                Reagent.PigIron
            );
            AddNecroSpell(
                13,
                41,
                79.6,
                typeof(VengefulSpiritScroll),
                Reagent.BatWing,
                Reagent.GraveDust,
                Reagent.PigIron
            );
            AddNecroSpell(14, 23, 59.6, typeof(WitherScroll), Reagent.GraveDust, Reagent.NoxCrystal, Reagent.PigIron);
            AddNecroSpell(15, 17, 79.6, typeof(WraithFormScroll), Reagent.NoxCrystal, Reagent.PigIron);
            AddNecroSpell(16, 40, 79.6, typeof(ExorcismScroll), Reagent.NoxCrystal, Reagent.GraveDust);
        }

        int index;

        if (Core.ML)
        {
            index = AddCraft(
                typeof(EnchantedSwitch),
                1044294,
                1072893,
                45.0,
                95.0,
                typeof(BlankScroll),
                1044377,
                1,
                1044378
            );
            AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
            AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
            AddRes(index, typeof(SwitchItem), 1073464, 1, 1044253);
            ForceNonExceptional(index);
            SetNeededExpansion(index, Expansion.ML);

            index = AddCraft(typeof(RunedPrism), 1044294, 1073465, 45.0, 95.0, typeof(BlankScroll), 1044377, 1, 1044378);
            AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
            AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
            AddRes(index, typeof(HollowPrism), 1072895, 1, 1044253);
            ForceNonExceptional(index);
            SetNeededExpansion(index, Expansion.ML);
        }

        // Runebook
        index = AddCraft(typeof(Runebook), 1044294, 1041267, 45.0, 95.0, typeof(BlankScroll), 1044377, 8, 1044378);
        AddRes(index, typeof(RecallScroll), 1044445, 1, 1044253);
        AddRes(index, typeof(GateTravelScroll), 1044446, 1, 1044253);

        if (Core.AOS)
        {
            AddCraft(typeof(BulkOrderBook), 1044294, 1028793, 65.0, 115.0, typeof(BlankScroll), 1044377, 10, 1044378);
        }

        if (Core.SE)
        {
            AddCraft(typeof(Spellbook), 1044294, 1023834, 50.0, 126, typeof(BlankScroll), 1044377, 10, 1044378);
        }

        /* TODO
        if (Core.ML)
        {
          index = AddCraft( typeof( ScrappersCompendium ), 1044294, 1072940, 75.0, 125.0, typeof( BlankScroll ), 1044377, 100, 1044378 );
          AddRes( index, typeof( DreadHornMane ), 1032682, 1, 1044253 );
          AddRes( index, typeof( Taint ), 1032679, 10, 1044253 );
          AddRes( index, typeof( Corruption ), 1032676, 10, 1044253 );
          AddRareRecipe( index, 400 );
          ForceNonExceptional( index );
          SetNeededExpansion( index, Expansion.ML );
        }
        */

        if (Core.SA)
        {
            AddCraft(typeof(MysticSpellbook), 1044294, 1031677, 50.0, 150.0, typeof(BlankScroll), 1044377, 10, 1044378);

            AddMysticismSpell(0, 4, -25.0, 25.0, typeof(NetherBoltScroll), Reagent.BlackPearl, Reagent.SulfurousAsh);
            AddMysticismSpell(
                1,
                4,
                -25.0,
                25.0,
                typeof(HealingStoneScroll),
                Reagent.Bone,
                Reagent.Garlic,
                Reagent.Ginseng,
                Reagent.SpidersSilk
            );
            AddMysticismSpell(
                2,
                6,
                -10.8,
                39.2,
                typeof(PurgeMagicScroll),
                Reagent.FertileDirt,
                Reagent.Garlic,
                Reagent.MandrakeRoot,
                Reagent.SulfurousAsh
            );
            AddMysticismSpell(
                3,
                6,
                -10.8,
                39.2,
                typeof(EnchantScroll),
                Reagent.SpidersSilk,
                Reagent.MandrakeRoot,
                Reagent.SulfurousAsh
            );
            AddMysticismSpell(
                4,
                9,
                3.5,
                53.5,
                typeof(SleepScroll),
                Reagent.Nightshade,
                Reagent.SpidersSilk,
                Reagent.BlackPearl
            );
            AddMysticismSpell(
                5,
                9,
                3.5,
                53.5,
                typeof(EagleStrikeScroll),
                Reagent.Bloodmoss,
                Reagent.Bone,
                Reagent.SpidersSilk,
                Reagent.MandrakeRoot
            );
            AddMysticismSpell(
                6,
                11,
                17.8,
                67.8,
                typeof(AnimatedWeaponScroll),
                Reagent.Bone,
                Reagent.BlackPearl,
                Reagent.MandrakeRoot,
                Reagent.Nightshade
            );
            AddMysticismSpell(
                7,
                11,
                17.8,
                67.8,
                typeof(StoneFormScroll),
                Reagent.Bloodmoss,
                Reagent.FertileDirt,
                Reagent.Garlic
            );
            AddMysticismSpell(
                8,
                14,
                32.1,
                82.1,
                typeof(SpellTriggerScroll),
                Reagent.DragonsBlood,
                Reagent.Garlic,
                Reagent.MandrakeRoot,
                Reagent.SpidersSilk
            );
            AddMysticismSpell(
                9,
                14,
                32.1,
                82.1,
                typeof(MassSleepScroll),
                Reagent.Ginseng,
                Reagent.Nightshade,
                Reagent.SpidersSilk
            );
            AddMysticismSpell(
                10,
                20,
                46.4,
                96.4,
                typeof(CleansingWindsScroll),
                Reagent.DragonsBlood,
                Reagent.Garlic,
                Reagent.Ginseng,
                Reagent.MandrakeRoot
            );
            AddMysticismSpell(
                11,
                20,
                46.4,
                96.4,
                typeof(BombardScroll),
                Reagent.Bloodmoss,
                Reagent.DragonsBlood,
                Reagent.Garlic,
                Reagent.SulfurousAsh
            );
            AddMysticismSpell(
                12,
                40,
                60.7,
                110.7,
                typeof(SpellPlagueScroll),
                Reagent.DaemonBone,
                Reagent.DragonsBlood,
                Reagent.Nightshade,
                Reagent.SulfurousAsh
            );
            AddMysticismSpell(
                13,
                40,
                60.7,
                110.7,
                typeof(HailStormScroll),
                Reagent.DragonsBlood,
                Reagent.Bloodmoss,
                Reagent.BlackPearl,
                Reagent.MandrakeRoot
            );
            AddMysticismSpell(
                14,
                50,
                75.0,
                125.0,
                typeof(NetherCycloneScroll),
                Reagent.MandrakeRoot,
                Reagent.Nightshade,
                Reagent.SulfurousAsh,
                Reagent.Bloodmoss
            );
            AddMysticismSpell(
                15,
                50,
                75.0,
                125.0,
                typeof(RisingColossusScroll),
                Reagent.DaemonBone,
                Reagent.DragonsBlood,
                Reagent.FertileDirt,
                Reagent.Nightshade
            );
        }

        MarkOption = true;
    }

    private enum Reg
    {
        BlackPearl,
        Bloodmoss,
        Garlic,
        Ginseng,
        MandrakeRoot,
        Nightshade,
        SulfurousAsh,
        SpidersSilk
    }
}
