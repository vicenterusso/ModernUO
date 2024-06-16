using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class AquariumGump : Gump
    {
        private readonly Aquarium m_Aquarium;

        public AquariumGump(Aquarium aquarium, bool edit) : base(100, 100)
        {
            m_Aquarium = aquarium;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 350, 323, 0xE10);
            AddImage(0, 0, 0x2C96);

            if (m_Aquarium.Items.Count == 0)
            {
                return;
            }

            for (var i = 1; i <= m_Aquarium.Items.Count; i++)
            {
                DisplayPage(i, edit);
            }
        }

        public void DisplayPage(int page, bool edit)
        {
            AddPage(page);

            var item = m_Aquarium.Items[page - 1];

            // item name
            if (item.LabelNumber != 0)
            {
                AddHtmlLocalized(20, 217, 250, 20, item.LabelNumber, 0x7FFF); // Name
            }

            // item details
            if (item is BaseFish fish)
            {
                AddHtmlLocalized(20, 239, 315, 20, fish.GetDescription(), 0x7FFF);
            }
            else
            {
                AddHtmlLocalized(20, 239, 315, 20, 1073634, 0x7FFF); // An aquarium decoration
            }

            // item image
            AddItem(150, 80, item.ItemID, item.Hue);

            // item number / all items
            AddHtml(20, 195, 250, 20, $"{page}/{m_Aquarium.Items.Count}".Color(0xFFFFFF));

            // remove item
            if (edit)
            {
                AddBackground(230, 195, 100, 26, 0x13BE);
                AddButton(235, 200, 0x845, 0x846, page);
                AddHtmlLocalized(260, 198, 60, 26, 1073838, 0x0); // Remove
            }

            // next page
            if (page < m_Aquarium.Items.Count)
            {
                AddButton(195, 280, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page + 1);
                AddHtmlLocalized(230, 283, 100, 18, 1044045, 0x7FFF); // NEXT PAGE
            }

            // previous page
            if (page > 1)
            {
                AddButton(45, 280, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page - 1);
                AddHtmlLocalized(80, 283, 100, 18, 1044044, 0x7FFF); // PREV PAGE
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (m_Aquarium?.Deleted != false)
            {
                return;
            }

            var edit = m_Aquarium.HasAccess(sender.Mobile);

            if (info.ButtonID > 0 && info.ButtonID <= m_Aquarium.Items.Count && edit)
            {
                m_Aquarium.RemoveItem(sender.Mobile, info.ButtonID - 1);
            }

            if (info.ButtonID > 0)
            {
                sender.Mobile.SendGump(new AquariumGump(m_Aquarium, edit));
            }
        }
    }
}
