using Server.Network;

namespace Server.Commands;

public static class Online
{
    public static void Configure()
    {
        CommandSystem.Register("Online", AccessLevel.Player, Online_OnCommand);
    }

    private static void Online_OnCommand(CommandEventArgs e)
    {
        var userCount = NetState.Instances.Count;

        e.Mobile.SendMessage($"There {(userCount == 1 ? "is" : "are")} currently {userCount} user{(userCount == 1 ? "" : "s")} " + $"online");

    }

}
