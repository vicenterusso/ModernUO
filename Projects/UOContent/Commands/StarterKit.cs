using Server.Gumps;
using Server.Items;

using static Server.Types;

namespace Server.Commands
{
    public static class StarterKit
    {
        public static void Initialize()
        {
            CommandSystem.Register("StarterKit", AccessLevel.Player, StarterKit_OnCommand);
        }

        private static void StarterKit_OnCommand(CommandEventArgs e)
        {

            var type = AssemblyHandler.FindTypeByName("bag");

            if (!IsEntity(type))
            {
                e.Mobile.SendMessage("No type with that name was found.");
            }

            var sLongsword = new Longsword();
            //e.Mobile.Backpack.DropItem(sLongsword);
            // e.Mobile.SendMessage("You've received a sword!");
            e.Mobile.SendGump(new BagCreationGump());

            //var bag = new Bag();
            /*

            */

        }

    }
}
