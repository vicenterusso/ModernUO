using ModernUO.Serialization;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.Jackrabbit")]
    [SerializationGenerator(0, false)]
    public partial class JackRabbit : BaseCreature
    {
        [Constructible]
        public JackRabbit() : base(AIType.AI_Animal, FightMode.Aggressor)
        {
            Body = 0xCD;
            Hue = 0x1BB;

            SetStr(15);
            SetDex(25);
            SetInt(5);

            SetHits(9);
            SetMana(0);

            SetDamage(1, 2);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 2, 5);

            SetSkill(SkillName.MagicResist, 5.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Fame = 150;
            Karma = 0;

            VirtualArmor = 4;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = -18.9;
        }

        public override string CorpseName => "a jack rabbit corpse";
        public override string DefaultName => "a jack rabbit";

        public override int Meat => 1;
        public override int Hides => 1;
        public override FoodType FavoriteFood => FoodType.FruitsAndVeggies;

        public override int GetAttackSound() => 0xC9;

        public override int GetHurtSound() => 0xCA;

        public override int GetDeathSound() => 0xCB;
    }
}
