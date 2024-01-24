using ModernUO.Serialization;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class SkeletalMount : BaseMount
    {
        [Constructible]
        public SkeletalMount() : base(793, 0x3EBB, AIType.AI_Animal, FightMode.Aggressor)
        {
            SetStr(91, 100);
            SetDex(46, 55);
            SetInt(46, 60);

            SetHits(41, 50);

            SetDamage(5, 12);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Cold, 90, 95);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 10, 15);

            SetSkill(SkillName.MagicResist, 95.1, 100.0);
            SetSkill(SkillName.Tactics, 50.0);
            SetSkill(SkillName.Wrestling, 70.1, 80.0);

            Fame = 0;
            Karma = 0;
        }

        public override int StepsMax => 4480;
        public override string CorpseName => "an undead horse corpse";
        public override string DefaultName => "a skeletal steed";

        public override Poison PoisonImmune => Poison.Lethal;
        public override bool BleedImmune => true;
    }
}
