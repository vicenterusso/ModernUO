using System;
using Server.Mobiles;
using Server.SkillHandlers;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Spells.Ninjitsu;
using Server.Spells.Spellweaving;

namespace Server.Items
{
    public abstract class BaseMeleeWeapon : BaseWeapon
    {
        public BaseMeleeWeapon(int itemID) : base(itemID)
        {
        }

        public BaseMeleeWeapon(Serial serial) : base(serial)
        {
        }

        public override bool CheckHit(Mobile attacker, Mobile defender)
        {
            var atkWeapon = attacker.Weapon as BaseWeapon;
            var defWeapon = defender.Weapon as BaseWeapon;

            var atkSkill = attacker.Skills[atkWeapon?.Skill ?? SkillName.Wrestling];

            var atkValue = atkWeapon?.GetAttackSkillValue(attacker, defender) ?? 0.0;
            var defValue = defWeapon?.GetDefendSkillValue(attacker, defender) ?? 0.0;

            double ourValue, theirValue;

            var bonus = GetHitChanceBonus();

            if (Core.AOS)
            {
                if (atkValue <= -20.0)
                {
                    atkValue = -19.9;
                }

                if (defValue <= -20.0)
                {
                    defValue = -19.9;
                }

                bonus += AosAttributes.GetValue(attacker, AosAttribute.AttackChance);

                if (DivineFurySpell.UnderEffect(attacker))
                {
                    bonus += 10; // attacker gets 10% bonus when they're under divine fury
                }

                if (AnimalForm.UnderTransformation(attacker, typeof(GreyWolf)) ||
                    AnimalForm.UnderTransformation(attacker, typeof(BakeKitsune)))
                {
                    bonus += 20; // attacker gets 20% bonus when under Wolf or Bake Kitsune form
                }

                if (HitLower.IsUnderAttackEffect(attacker))
                {
                    bonus -= 25; // Under Hit Lower Attack effect -> 25% malus
                }

                var ability = WeaponAbility.GetCurrentAbility(attacker);

                if (ability != null)
                {
                    bonus += ability.AccuracyBonus;
                }

                var move = SpecialMove.GetCurrentMove(attacker);

                if (move != null)
                {
                    bonus += move.GetAccuracyBonus(attacker);
                }

                // Max Hit Chance Increase = 45%
                if (bonus > 45)
                {
                    bonus = 45;
                }

                ourValue = (atkValue + 20.0) * (100 + bonus);

                bonus = AosAttributes.GetValue(defender, AosAttribute.DefendChance);

                var info = ForceArrow.GetInfo(attacker, defender);

                if (info != null && info.Defender == defender)
                {
                    bonus -= info.DefenseChanceMalus;
                }

                if (DivineFurySpell.UnderEffect(defender))
                {
                    bonus -= 20; // defender loses 20% bonus when they're under divine fury
                }

                if (HitLower.IsUnderDefenseEffect(defender))
                {
                    bonus -= 25; // Under Hit Lower Defense effect -> 25% malus
                }

                var blockBonus = 0;

                if (Block.GetBonus(defender, ref blockBonus))
                {
                    bonus += blockBonus;
                }

                var surpriseMalus = 0;

                if (SurpriseAttack.GetMalus(defender, ref surpriseMalus))
                {
                    bonus -= surpriseMalus;
                }

                var discordanceEffect = 0;

                // Defender loses -0/-28% if under the effect of Discordance.
                if (Discordance.GetEffect(attacker, ref discordanceEffect))
                {
                    bonus -= discordanceEffect;
                }

                // Defense Chance Increase = 45%
                if (bonus > 45)
                {
                    bonus = 45;
                }

                theirValue = (defValue + 20.0) * (100 + bonus);

                bonus = 0;
            }
            else
            {
                ourValue = Math.Max(0.1, atkValue + 50.0);
                theirValue = Math.Max(0.1, defValue + 50.0);
            }


            if (defender.Spell.IsCasting)
            {
                var isMace = atkSkill.SkillName == SkillName.Macing;
                var isFencing = atkSkill.SkillName == SkillName.Fencing;
                var isSwords = atkSkill.SkillName == SkillName.Swords;
                var notStaffs = defender.Weapon is not QuarterStaff && defender.Weapon is not BlackStaff &&
                                defender.Weapon is not GnarledStaff;
                if ((isMace && notStaffs) || isFencing || isSwords)
                {
                    bonus += 25;
                }
            }

            if(defender.Skills[SkillName.Wrestling].Value > 0)
            {
                int wrestlingValue = (int)defender.Skills[SkillName.Wrestling].Value;
                if (wrestlingValue >= 100)
                {
                    bonus -= 7;
                }
                if (wrestlingValue >= 80)
                {
                    bonus -= 5;
                }
            }

            var chance = ourValue / (theirValue * 2.0) * 1.0 + (double)bonus / 100;

            if (Core.AOS && chance < 0.02)
            {
                chance = 0.02;
            }

            return attacker.CheckSkill(atkSkill.SkillName, chance);

        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus = 1.0)
        {
            base.OnHit(attacker, defender, damageBonus);

            // Se atacante usa fencing ou mace, remove 1 stam do defender
            var atkWeapon = attacker.Weapon as BaseWeapon;
            var atkSkill = attacker.Skills[atkWeapon?.Skill ?? SkillName.Wrestling];
            var isMace = atkSkill.SkillName == SkillName.Macing;
            var isFencing = atkSkill.SkillName == SkillName.Fencing;
            if(isMace || isFencing)
            {
                defender.Stam -= 1;
            }

        }

        public override int AbsorbDamage(Mobile attacker, Mobile defender, int damage)
        {
            damage = base.AbsorbDamage(attacker, defender, damage);

            AttuneWeaponSpell.TryAbsorb(defender, ref damage);

            if (Core.AOS)
            {
                return damage;
            }

            var absorb = defender.MeleeDamageAbsorb;

            if (absorb > 0)
            {
                if (absorb > damage)
                {
                    var react = damage / 5;

                    if (react <= 0)
                    {
                        react = 1;
                    }

                    defender.MeleeDamageAbsorb -= damage;
                    damage = 0;

                    attacker.Damage(react, defender);

                    attacker.PlaySound(0x1F1);
                    attacker.FixedEffect(0x374A, 10, 16);
                }
                else
                {
                    defender.MeleeDamageAbsorb = 0;
                    defender.SendLocalizedMessage(1005556); // Your reactive armor spell has been nullified.
                    DefensiveSpell.Nullify(defender);
                }
            }

            return damage;
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
