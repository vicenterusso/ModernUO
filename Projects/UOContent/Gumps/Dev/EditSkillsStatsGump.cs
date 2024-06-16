using System;
using Server;
using Server.Gumps;
using Server.Network;


namespace Server.Gumps
{
    public class EditSkillsStatsGump : Gump
    {

        private Mobile m_Mobile;
        private const int SkillsPerPage = 10;
        private int m_Page;

        public EditSkillsStatsGump(Mobile mobile, int page) : base(50, 50)
        {
            m_Mobile = mobile;
            m_Page = page;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 450, 500, 0xE10);

            AddLabel(150, 20, 1153, "Edit Skills and Stats");

            // Add Stats
            AddLabel(50, 50, 1153, "Strength: " + mobile.Str);
            AddButton(300, 50, 4005, 4007, 6000, GumpButtonType.Reply, 0);

            AddLabel(50, 80, 1153, "Dexterity: " + mobile.Dex);
            AddButton(300, 80, 4005, 4007, 6001, GumpButtonType.Reply, 0);

            AddLabel(50, 110, 1153, "Intelligence: " + mobile.Int);
            AddButton(300, 110, 4005, 4007, 6002, GumpButtonType.Reply, 0);

            // Add Skills
            int skillStart = page * SkillsPerPage;
            int skillEnd = skillStart + SkillsPerPage;
            int skillCount = mobile.Skills.Length;

            if (skillEnd > skillCount) skillEnd = skillCount;

            for (int i = skillStart; i < skillEnd; i++)
            {
                AddLabel(50, 150 + ((i - skillStart) * 20), 1153, mobile.Skills[i].Name + ": " + mobile.Skills[i].Base);
                AddButton(300, 150 + ((i - skillStart) * 20), 4005, 4007, 1000 + i, GumpButtonType.Reply, 0);
            }

            if (page > 0)
            {
                AddButton(50, 450, 4014, 4015, 5000, GumpButtonType.Reply, 0); // Previous page
                AddLabel(90, 450, 1153, "Previous");
            }

            if (skillEnd < skillCount)
            {
                AddButton(350, 450, 4005, 4006, 5001, GumpButtonType.Reply, 0); // Next page
                AddLabel(390, 450, 1153, "Next");
            }

            AddButton(400, 20, 4017, 4018, 5002, GumpButtonType.Reply, 0); // Close button
        }

        public override void OnResponse(NetState sender, in RelayInfo relayInfo)
        {
            Mobile m = sender.Mobile;
            int buttonID = relayInfo.ButtonID;

            if (buttonID >= 1000 && buttonID < 2000)
            {
                // Open Skill Edit Gump
                int skillIndex = buttonID - 1000;
                m.SendGump(new EditOwnSkillGump(m, skillIndex, m_Page));
            }
            else if (buttonID >= 6000 && buttonID < 6003)
            {
                // Open Stat Edit Gump
                int statIndex = buttonID - 6000;
                m.SendGump(new EditStatGump(m, statIndex, m_Page));
            }
            else if (buttonID == 5000)
            {
                // Previous page
                m.SendGump(new EditSkillsStatsGump(m, m_Page - 1));
            }
            else if (buttonID == 5001)
            {
                // Next page
                m.SendGump(new EditSkillsStatsGump(m, m_Page + 1));
            }
            else if (buttonID == 5002)
            {
                // Close gump
                return;
            }
            else
            {
                return;
            }

        }

    }
}

public class EditOwnSkillGump : Gump
{
    private Mobile m_Mobile;
    private int m_SkillIndex;
    private int m_Page;

    public EditOwnSkillGump(Mobile mobile, int skillIndex, int page) : base(50, 50)
    {
        m_Mobile = mobile;
        m_SkillIndex = skillIndex;
        m_Page = page;

        Closable = true;
        Disposable = true;
        Draggable = true;
        Resizable = false;

        AddPage(0);
        AddBackground(0, 0, 300, 150, 0xE10);

        AddLabel(50, 20, 1153, "Edit " + mobile.Skills[skillIndex].Name);
        AddLabel(50, 50, 1153, "Value (0 - 100):");

        AddTextEntry(200, 50, 50, 20, 1153, 0, mobile.Skills[skillIndex].Base.ToString());

        AddButton(100, 100, 4005, 4007, 1, GumpButtonType.Reply, 0); // Apply button
        AddLabel(140, 100, 1153, "Apply");
    }

    public override void OnResponse(NetState sender, in RelayInfo relayInfo)
    {
        Mobile m = sender.Mobile;
        var entry = relayInfo.GetTextEntry(0);
        if (entry != null && double.TryParse(entry, out double newValue))
        {
            if (newValue > 100)
            {
                newValue = 100;
            }

            if (newValue < 0)
            {
                newValue = 0;
            }

            double totalSkills = 0;
            foreach (Skill skill in m.Skills)
            {
                if (skill != m.Skills[m_SkillIndex])
                {
                    totalSkills += skill.Base;
                }
            }

            if (totalSkills + newValue > m.SkillsCap)
            {
                newValue = m.SkillsCap - totalSkills;
            }

            m.Skills[m_SkillIndex].Base = newValue;
            m.SendMessage("Updated " + m.Skills[m_SkillIndex].Name + " to " + newValue);

        }
        else
        {
            m.SendMessage("Invalid input.");
        }
        m.SendGump(new EditSkillsStatsGump(m, m_Page));
    }
}


public class EditStatGump : Gump
{
    private Mobile m_Mobile;
    private int m_StatIndex;
    private int m_Page;

    public EditStatGump(Mobile mobile, int statIndex, int page) : base(50, 50)
    {
        m_Mobile = mobile;
        m_StatIndex = statIndex;
        m_Page = page;

        Closable = true;
        Disposable = true;
        Draggable = true;
        Resizable = false;

        AddPage(0);
        AddBackground(0, 0, 300, 150, 0xE10);

        string statName = statIndex switch
        {
            0 => "Strength",
            1 => "Dexterity",
            2 => "Intelligence",
            _ => "Unknown"
        };

        int currentValue = statIndex switch
        {
            0 => mobile.Str,
            1 => mobile.Dex,
            2 => mobile.Int,
            _ => 0
        };

        AddLabel(50, 20, 1153, "Edit " + statName);
        AddLabel(50, 50, 1153, "Value (0 - 100):");

        AddTextEntry(200, 50, 50, 20, 1153, 0, currentValue.ToString());

        AddButton(100, 100, 4005, 4007, 1, GumpButtonType.Reply, 0); // Apply button
        AddLabel(140, 100, 1153, "Apply");
    }

    public override void OnResponse(NetState sender, in RelayInfo relayInfo)
    {
        Mobile m = sender.Mobile;
        var entry = relayInfo.GetTextEntry(0);
        if (entry != null && int.TryParse(entry, out int newValue))
        {

            if (newValue > 100)
            {
                newValue = 100;
            }

            if (newValue < 0)
            {
                newValue = 0;
            }

            int totalStats = m_StatIndex switch
            {
                0 => m.Dex + m.Int,
                1 => m.Str + m.Int,
                2 => m.Str + m.Dex,
                _ => 0
            };

            Console.WriteLine("m.StatCap " + m.StatCap);
            Console.WriteLine("totalStats " + totalStats);
            Console.WriteLine("m.Dex " + m.Dex);
            Console.WriteLine("m.Str " + m.Str);
            Console.WriteLine(" m.Int " +  m.Int);
            Console.WriteLine(" newValue " +  newValue);
            if (totalStats + newValue > m.StatCap)
            {
                newValue = m.StatCap - totalStats;
            }

            switch (m_StatIndex)
            {
                case 0:
                    m.Str = newValue;
                    m.SendMessage("Updated Strength to " + newValue);
                    break;
                case 1:
                    m.Dex = newValue;
                    m.SendMessage("Updated Dexterity to " + newValue);
                    break;
                case 2:
                    m.Int = newValue;
                    m.SendMessage("Updated Intelligence to " + newValue);
                    break;
            }
        }
        else
        {
            m.SendMessage("Invalid input.");
        }
        m.SendGump(new EditSkillsStatsGump(m, m_Page));
    }
}
