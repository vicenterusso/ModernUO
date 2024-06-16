using System;
using Server.Collections;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Necromancy;

public class WitherSpell : NecromancerSpell
{
    private static readonly SpellInfo _info = new(
        "Wither",
        "Kal Vas An Flam",
        203,
        9031,
        Reagent.NoxCrystal,
        Reagent.GraveDust,
        Reagent.PigIron
    );

    public WitherSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
    {
    }

    public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(Core.Expansion switch
    {
        >= Expansion.SA => 1.25,
        >= Expansion.ML => 1.5,
        _               => 1.0
    });

    public override double RequiredSkill => 60.0;

    public override int RequiredMana => 23;

    public override bool DelayedDamage => false;

    public override void OnCast()
    {
        if (CheckSequence())
        {
            /* Creates a withering frost around the Caster,
             * which deals Cold Damage to all valid targets in a radius of 5 tiles.
             */

            var map = Caster.Map;

            if (map != null)
            {
                using var pool = PooledRefQueue<Mobile>.Create();

                var cbc = Caster as BaseCreature;
                var isMonster = cbc?.Controlled == false && (cbc.IsAnimatedDead || !cbc.Summoned);

                foreach (var targ in Caster.GetMobilesInRange(Core.ML ? 4 : 5))
                {
                    if (targ == Caster
                        || !Caster.InLOS(targ)
                        || !isMonster && !SpellHelper.ValidIndirectTarget(Caster, targ)
                        || !Caster.CanBeHarmful(targ, false))
                    {
                        continue;
                    }

                    if (isMonster && targ.Player)
                    {
                        continue;
                    }

                    // Animate dead casting poison strike shouldn't hit: familiars or player or pets
                    if (targ is BaseCreature bc)
                    {
                        if (bc.IsAnimatedDead)
                        {
                            continue;
                        }

                        if (isMonster && (bc.Controlled || bc.Summoned || bc.Team == cbc.Team || bc.IsNecroFamiliar))
                        {
                            continue;
                        }
                    }

                    pool.Enqueue(targ);
                }

                Effects.PlaySound(Caster.Location, map, 0x1FB);
                Effects.PlaySound(Caster.Location, map, 0x10B);
                Effects.SendLocationParticles(
                    EffectItem.Create(Caster.Location, map, EffectItem.DefaultDuration),
                    0x37CC,
                    1,
                    40,
                    97,
                    3,
                    9917,
                    0
                );

                while (pool.Count > 0)
                {
                    var m = pool.Dequeue();

                    Caster.DoHarmful(m);
                    m.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);

                    double damage = Utility.RandomMinMax(30, 35);

                    damage *= 300 + m.Karma / 100.0 + GetDamageSkill(Caster) * 10;
                    damage /= 1000;

                    var sdiBonus = AosAttributes.GetValue(Caster, AosAttribute.SpellDamage);

                    // PvP spell damage increase cap of 15% from an item's magic property in Publish 33(SE)
                    if (Core.SE && m.Player && Caster.Player && sdiBonus > 15)
                    {
                        sdiBonus = 15;
                    }

                    damage *= 100 + sdiBonus;
                    damage /= 100;

                    // TODO: cap?
                    // if (damage > 40)
                    // damage = 40;

                    SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);
                }
            }
        }

        FinishSequence();
    }
}