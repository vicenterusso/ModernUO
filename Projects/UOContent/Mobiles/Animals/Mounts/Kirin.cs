using ModernUO.Serialization;
using System;
using Server.Items;

namespace Server.Mobiles
{
    [SerializationGenerator(0, false)]
    public partial class Kirin : BaseMount
    {
        public override string DefaultName => "a ki-rin";

        [Constructible]
        public Kirin() : base(132, 0x3EAD, AIType.AI_Mage, FightMode.Evil)
        {
            BaseSoundID = 0x3C5;

            SetStr(296, 325);
            SetDex(86, 105);
            SetInt(186, 225);

            SetHits(191, 210);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Fire, 10);
            SetDamageType(ResistanceType.Cold, 10);
            SetDamageType(ResistanceType.Energy, 10);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.EvalInt, 80.1, 90.0);
            SetSkill(SkillName.Magery, 60.4, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 85.3, 100.0);
            SetSkill(SkillName.Tactics, 20.1, 22.5);
            SetSkill(SkillName.Wrestling, 80.5, 92.5);

            Fame = 9000;
            Karma = 9000;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 95.1;
        }

        public override int StepsMax => 4480;
        public override string CorpseName => "a ki-rin corpse";
        public override bool AllowFemaleRider => false;
        public override bool AllowFemaleTamer => false;

        public override bool InitialInnocent => true;

        public override TimeSpan MountAbilityDelay => TimeSpan.FromHours(1.0);

        public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

        public override int Meat => 3;
        public override int Hides => 10;
        public override HideType HideType => HideType.Horned;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;

        public override void OnDisallowedRider(Mobile m)
        {
            m.SendLocalizedMessage(1042319); // The Ki-Rin refuses your attempts to mount it.
        }

        public override bool DoMountAbility(int damage, Mobile attacker)
        {
            if (Rider == null || attacker == null) // sanity
            {
                return false;
            }

            // Range and map checked here instead of other base function because of abilities that don't need to check this
            if (Rider.Hits - damage < 30 && Rider.Map == attacker.Map && Rider.InRange(attacker, 18))
            {
                attacker.BoltEffect(0);
                // 35~100 damage, unresistable, by the Ki-rin.
                // Don't inform mount about this damage, Still unsure whether or not it's flagged as the mount doing damage or the player.
                // If changed to player, without the extra bool it'd be an infinite loop
                attacker.Damage(Utility.RandomMinMax(35, 100), this, false);

                // Your mount calls down the forces of nature on your opponent.
                Rider.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1042534);
                Rider.FixedParticles(0, 0, 0, 0x13A7, EffectLayer.Waist);
                Rider.PlaySound(0xA9); // Ki-rin's whinny.
                return true;
            }

            return false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.Potions);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.35)
            {
                c.DropItem(new KirinBrains());
            }
        }
    }
}
