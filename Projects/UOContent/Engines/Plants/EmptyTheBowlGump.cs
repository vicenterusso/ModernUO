using Server.Engines.Help;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Plants
{
    public class EmptyTheBowlGump : Gump
    {
        private readonly PlantItem m_Plant;

        public EmptyTheBowlGump(PlantItem plant) : base(20, 20)
        {
            m_Plant = plant;

            DrawBackground();

            AddLabel(90, 70, 0x44, "Empty the bowl?");

            DrawPicture();

            AddButton(98, 150, 0x47E, 0x480, 1); // Cancel

            AddButton(138, 151, 0xD2, 0xD2, 2); // Help
            AddLabel(143, 151, 0x835, "?");

            AddButton(168, 150, 0x481, 0x483, 3); // Ok
        }

        private void DrawBackground()
        {
            AddBackground(50, 50, 200, 150, 0xE10);

            AddItem(45, 45, 0xCEF);
            AddItem(45, 118, 0xCF0);

            AddItem(211, 45, 0xCEB);
            AddItem(211, 118, 0xCEC);
        }

        private void DrawPicture()
        {
            AddItem(90, 100, 0x1602);
            AddImage(140, 102, 0x15E1);
            AddItem(160, 100, 0x15FD);

            if (m_Plant.PlantStatus != PlantStatus.BowlOfDirt && m_Plant.PlantStatus < PlantStatus.Plant)
            {
                AddItem(156, 130, 0xDCF); // Seed
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var from = sender.Mobile;

            if (info.ButtonID == 0 || m_Plant.Deleted || m_Plant.PlantStatus >= PlantStatus.DecorativePlant)
            {
                return;
            }

            if (info.ButtonID == 3 && !from.InRange(m_Plant.GetWorldLocation(), 3))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 500446); // That is too far away.
                return;
            }

            if (!m_Plant.IsUsableBy(from))
            {
                m_Plant.LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // Cancel
                    {
                        from.SendGump(new MainPlantGump(m_Plant));

                        break;
                    }
                case 2: // Help
                    {
                        from.NetState.SendDisplayHelpTopic(HelpTopic.EmptyingBowl);

                        from.SendGump(new EmptyTheBowlGump(m_Plant));

                        break;
                    }
                case 3: // Ok
                    {
                        var bowl = new PlantBowl();

                        if (!from.PlaceInBackpack(bowl))
                        {
                            bowl.Delete();

                            m_Plant.LabelTo(from, 1053047); // You cannot empty a bowl with a full pack!
                            from.SendGump(new MainPlantGump(m_Plant));

                            break;
                        }

                        if (m_Plant.PlantStatus != PlantStatus.BowlOfDirt && m_Plant.PlantStatus < PlantStatus.Plant)
                        {
                            var seed = new Seed(m_Plant.PlantType, m_Plant.PlantHue, m_Plant.ShowType);

                            if (!from.PlaceInBackpack(seed))
                            {
                                bowl.Delete();
                                seed.Delete();

                                m_Plant.LabelTo(from, 1053047); // You cannot empty a bowl with a full pack!
                                from.SendGump(new MainPlantGump(m_Plant));

                                break;
                            }
                        }

                        m_Plant.Delete();

                        break;
                    }
            }
        }
    }
}
