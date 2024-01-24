using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class TavaraSewel : BaseCreature
{
    [Constructible]
    public TavaraSewel() : base(AIType.AI_Melee)
    {
        Title = "the Cursed";

        Hue = 0x8838;
        Female = true;
        Body = 0x191;

        AddItem(new Kilt(0x59C));
        AddItem(new Sandals(0x599));

        var kryss = new Kryss();
        var buckler = new Buckler();
        var gloves = new RingmailGloves();
        var chest = new FemalePlateChest();

        kryss.Hue = 0x96F;
        kryss.Movable = false;
        buckler.Hue = 0x96F;
        buckler.Movable = false;
        gloves.Hue = 0x599;
        chest.Hue = 0x96F;

        AddItem(kryss);
        AddItem(buckler);
        AddItem(gloves);
        AddItem(chest);

        SetStr(111, 120);
        SetDex(111, 120);
        SetInt(111, 120);

        SetHits(180, 207);
        SetStam(126, 150);
        SetMana(0);

        SetDamage(13, 16);

        SetResistance(ResistanceType.Physical, 25, 30);
        SetResistance(ResistanceType.Fire, 25, 30);
        SetResistance(ResistanceType.Cold, 50, 60);
        SetResistance(ResistanceType.Poison, 25, 35);
        SetResistance(ResistanceType.Energy, 25, 35);

        SetSkill(SkillName.Fencing, 90.1, 100.0);
        SetSkill(SkillName.Tactics, 90.1, 100.0);
        SetSkill(SkillName.MagicResist, 80.1, 90.0);
        SetSkill(SkillName.Anatomy, 90.1, 100.0);

        Fame = 5000;
        Karma = -1000;
    }

    public override bool ShowFameTitle => false;
    public override bool DeleteCorpseOnDeath => true;
    public override string DefaultName => "Tavara Sewel";

    public override bool AlwaysMurderer => true;

    public override int GetIdleSound() => 0x27F;

    public override int GetAngerSound() => 0x258;

    public override int GetDeathSound() => 0x25B;

    public override int GetHurtSound() => 0x257;

    public override bool OnBeforeDeath()
    {
        var gold = new Gold(Utility.RandomMinMax(190, 230));
        gold.MoveToWorld(Location, Map);

        if (Utility.Random(3) == 0)
        {
            var journal = Loot.RandomTavarasJournal();
            journal.MoveToWorld(Location, Map);
        }

        Effects.SendLocationEffect(Location, Map, 0x376A, 10, 1);
        return true;
    }
}
