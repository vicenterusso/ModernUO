using Server.Ethics;

namespace Server.Mobiles
{
    public class UnholySteed : BaseMount
    {
        public override string DefaultName => "a dark steed";

        [Constructible]
        public UnholySteed() : base(0x74, 0x3EA7, AIType.AI_Melee, FightMode.Aggressor)
        {
            SetStr(496, 525);
            SetDex(86, 105);
            SetInt(86, 125);

            SetHits(298, 315);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 40);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 25.1, 30.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 80.5, 92.5);

            Fame = 14000;
            Karma = -14000;

            VirtualArmor = 60;

            Tamable = false;
            ControlSlots = 1;
        }

        public UnholySteed(Serial serial) : base(serial)
        {
        }

        public override int StepsMax => 6400;
        public override string CorpseName => "an unholy corpse";
        public override bool IsDispellable => false;
        public override bool IsBondable => false;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;

        private static MonsterAbility[] _abilities = { MonsterAbilities.FireBreath };
        public override MonsterAbility[] GetMonsterAbilities() => _abilities;

        public override string ApplyNameSuffix(string suffix)
        {
            if (suffix.Length == 0)
            {
                return base.ApplyNameSuffix(Ethic.Evil.Definition.Adjunct.String);
            }

            return base.ApplyNameSuffix($"{suffix} {Ethic.Evil.Definition.Adjunct.String}");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Ethic.Find(from) != Ethic.Evil)
            {
                from.SendMessage("You may not ride this steed.");
            }
            else
            {
                base.OnDoubleClick(from);
            }
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();
        }
    }
}
