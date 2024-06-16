using Server.Gumps;

namespace Server.Commands;

public static class EditSkillsStats
{
    public static void Configure()
    {
        CommandSystem.Register("EditSkillsStats", AccessLevel.Player, Online_EditSkillsStats);
    }

    private static void Online_EditSkillsStats(CommandEventArgs e)
    {
        e.Mobile.SendGump(new EditSkillsStatsGump(e.Mobile, 0));
    }

}
