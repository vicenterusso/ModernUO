using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Necromancy;

public class VengefulSpiritSpell : NecromancerSpell, ISpellTargetingMobile
{
    private static readonly SpellInfo _info = new(
        "Vengeful Spirit",
        "Kal Xen Bal Beh",
        203,
        9031,
        Reagent.BatWing,
        Reagent.GraveDust,
        Reagent.PigIron
    );

    public VengefulSpiritSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
    {
    }

    public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.0);

    public override double RequiredSkill => 80.0;
    public override int RequiredMana => 41;

    public void Target(Mobile m)
    {
        if (m == null)
        {
            return;
        }

        if (Caster == m)
        {
            Caster.SendLocalizedMessage(1061832); // You cannot exact vengeance on yourself.
        }
        else if (CheckHSequence(m))
        {
            SpellHelper.Turn(Caster, m);

            /* Summons a Revenant which haunts the target until either the target or the Revenant is dead.
             * Revenants have the ability to track down their targets wherever they may travel.
             * A Revenant's strength is determined by the Necromancy and Spirit Speak skills of the Caster.
             * The effect lasts for ((Spirit Speak skill level * 80) / 120) + 10 seconds.
             */

            var duration = TimeSpan.FromSeconds(GetDamageSkill(Caster) * 80 / 120 + 10);

            var rev = new Revenant(Caster, m, duration);

            if (BaseCreature.Summon(
                    rev,
                    false,
                    Caster,
                    m.Location,
                    0x81,
                    TimeSpan.FromSeconds(duration.TotalSeconds + 2.0)
                ))
            {
                rev.FixedParticles(0x373A, 1, 15, 9909, EffectLayer.Waist);
            }
        }

        FinishSequence();
    }

    public override void OnCast()
    {
        Caster.Target = new SpellTargetMobile(this, TargetFlags.Harmful, Core.ML ? 10 : 12);
    }

    public override bool CheckCast()
    {
        if (!base.CheckCast())
        {
            return false;
        }

        if (Caster.Followers + 3 > Caster.FollowersMax)
        {
            Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
            return false;
        }

        return true;
    }
}