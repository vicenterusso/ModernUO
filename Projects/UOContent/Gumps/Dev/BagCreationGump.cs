using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Server.Commands.Generic;
using Server.Network;
using CPA = Server.CommandPropertyAttribute;

using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using static Server.Types;
using static Server.Attributes;
using static Server.Gumps.PropsConfig;


namespace Server.Gumps
{
    public class BagCreationGump : Gump
    {
        public BagCreationGump() : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Resizable = false;

            AddPage(0);

            AddBackground(0, 0, 260, 320, 0xE10);

            AddLabel(30, 15, 1153, "Selecione um Starter Kit (Dev)");

            AddButton(30, 50, 4005, 4007, 1, GumpButtonType.Reply, 0); // Button for Armor Bag
            AddLabel(70, 50, 2969, "Kit Plate Armor + Chain");

            AddButton(30, 90, 4005, 4007, 2, GumpButtonType.Reply, 0); // Button for Reagent Bag
            AddLabel(70, 90, 2969, "Kit Regs + Spellbook");

            AddButton(30, 130, 4005, 4007, 3, GumpButtonType.Reply, 0); // Button for Weapon Bag
            AddLabel(70, 130, 2969, "Kit Weapons");

            AddButton(30, 170, 4005, 4007, 4, GumpButtonType.Reply, 0); // Button for Weapon Bag
            AddLabel(70, 170, 2969, "Kit Couro + Staff");

            AddButton(30, 210, 4005, 4007, 5, GumpButtonType.Reply, 0); // Button for Weapon Bag
            AddLabel(70, 210, 2969, "Horse");

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1:

                    var armorBag = new Bag() { Hue = 263 };
                    armorBag.DropItem(new PlateChest() { Resource = CraftResource.Valorite });
                    armorBag.DropItem(new PlateArms() { Resource = CraftResource.Valorite });
                    armorBag.DropItem(new PlateGorget() { Resource = CraftResource.Valorite });
                    armorBag.DropItem(new PlateGloves() { Resource = CraftResource.Valorite });
                    armorBag.DropItem(new PlateLegs() { Resource = CraftResource.Valorite });
                    armorBag.DropItem(new PlateHelm() { Resource = CraftResource.Valorite });
                    m.Backpack.DropItem(armorBag);
                    m.SendMessage("Você recebeu uma bag de armadura de placa");

                    var chainArmorBag = new Bag() { Hue = 93 };
                    chainArmorBag.DropItem(new ChainChest() { Resource = CraftResource.Valorite });
                    chainArmorBag.DropItem(new PlateGloves() { Resource = CraftResource.Valorite });
                    chainArmorBag.DropItem(new ChainLegs() { Resource = CraftResource.Valorite });
                    chainArmorBag.DropItem(new ChainCoif() { Resource = CraftResource.Valorite });
                    m.Backpack.DropItem(chainArmorBag);
                    m.SendMessage("Voccê recebeu uma bag de armadura de malha");

                    break;
                case 2:

                    var mageBag = new BagOfAllReagents() { Hue = 39 };

                    // Add a full spellbook to the bag
                    Spellbook spellbook = new Spellbook();
                    for (int i = 0; i < 64; i++) // There are 64 spells, one for each bit of a ulong
                    {
                        spellbook.Content |= (ulong)1 << i;
                    }
                    mageBag.DropItem(spellbook);

                    m.Backpack.DropItem(mageBag);
                    m.SendMessage("Você recebeu uma bag de regs e full spellbook");


                    break;
                case 3:

                    var weaponBag = new Bag() { Hue = 53 };
                    weaponBag.DropItem(new Longsword() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Kryss() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Katana() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Broadsword() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Halberd() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new DoubleAxe() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Mace() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Maul() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new ShortSpear() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Spear() { Resource = CraftResource.Valorite });
                    weaponBag.DropItem(new Bow() { Resource = CraftResource.OakWood });
                    weaponBag.DropItem(new CompositeBow() { Resource = CraftResource.OakWood });
                    weaponBag.DropItem(new Crossbow() { Resource = CraftResource.OakWood });
                    weaponBag.DropItem(new HeavyCrossbow() { Resource = CraftResource.OakWood });
                    weaponBag.DropItem(new QuarterStaff() { Resource = CraftResource.OakWood });
                    weaponBag.DropItem(new BlackStaff() { Resource = CraftResource.OakWood });// OakWood, AshWood, YewWood, Heartwood, Bloodwood, Frostwood;
                    m.Backpack.DropItem(weaponBag);
                    m.SendMessage("Você recebeu uma bag de armas");

                    break;

                case 4:

                    var leatherBag = new Bag() { Hue = 25 };

                    // Staffs
                    leatherBag.DropItem(new QuarterStaff() { Resource = CraftResource.Frostwood });
                    leatherBag.DropItem(new BlackStaff() { Resource = CraftResource.Frostwood });// OakWood, AshWood, YewWood, Heartwood, Bloodwood, Frostwood;

                    // Couro barbado
                    leatherBag.DropItem(new StuddedChest() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new StuddedArms() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new StuddedGorget() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new StuddedGloves() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new StuddedLegs() { Resource = CraftResource.BarbedLeather });

                    // Couro
                    leatherBag.DropItem(new LeatherCap() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new LeatherChest() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new LeatherArms() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new LeatherGorget() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new LeatherGloves() { Resource = CraftResource.BarbedLeather });
                    leatherBag.DropItem(new LeatherLegs() { Resource = CraftResource.BarbedLeather });

                    m.Backpack.DropItem(leatherBag);
                    m.SendMessage("Você recebeu uma bag com staffs e armadura de couro");

                    break;

                case 5:

                    Horse horse = new Horse();

                    // Set the horse to be owned and controlled by the player
                    horse.Owners.Add(m);
                    horse.SetControlMaster(m);

                    // Optionally: Set the horse as bonded
                    horse.IsBonded = true;

                    // Spawn the horse at the player's location
                    horse.MoveToWorld(m.Location, m.Map);

                    m.SendMessage("Você tem um cavalo!");
                    break;
            }
        }
    }
}
