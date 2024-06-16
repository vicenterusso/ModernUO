using System.Reflection;
using Server.Commands;
using Server.Network;
using Server.Targeting;

using static Server.Gumps.PropsConfig;

namespace Server.Gumps
{
    public class SetPoint3DGump : Gump
    {
        private static readonly int CoordWidth = 70;
        private static readonly int EntryWidth = CoordWidth + OffsetSize + CoordWidth + OffsetSize + CoordWidth;

        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + 4 * (EntryHeight + OffsetSize);

        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly PropertyInfo m_Property;
        private readonly PropertiesGump m_PropertiesGump;

        public SetPoint3DGump(
            PropertyInfo prop, Mobile mobile, object o, PropertiesGump propertiesGump
        )
            : base(GumpOffsetX, GumpOffsetY)
        {
            m_PropertiesGump = propertiesGump;
            m_Property = prop;
            m_Mobile = mobile;
            m_Object = o;

            var p = (Point3D)(prop?.GetValue(o, null) ?? new Point3D());

            AddPage(0);

            AddBackground(0, 0, BackWidth, BackHeight, BackGumpID);
            AddImageTiled(
                BorderSize,
                BorderSize,
                TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0),
                TotalHeight,
                OffsetGumpID
            );

            var x = BorderSize + OffsetSize;
            var y = BorderSize + OffsetSize;

            AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, prop?.Name);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
            {
                AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            }

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Use your location");
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
            {
                AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            }

            AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, "Target a location");
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
            {
                AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            }

            AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 2);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            AddImageTiled(x, y, CoordWidth, EntryHeight, EntryGumpID);
            AddLabelCropped(x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "X:");
            AddTextEntry(x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 0, p.X.ToString());
            x += CoordWidth + OffsetSize;

            AddImageTiled(x, y, CoordWidth, EntryHeight, EntryGumpID);
            AddLabelCropped(x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "Y:");
            AddTextEntry(x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 1, p.Y.ToString());
            x += CoordWidth + OffsetSize;

            AddImageTiled(x, y, CoordWidth, EntryHeight, EntryGumpID);
            AddLabelCropped(x + TextOffsetX, y, CoordWidth - TextOffsetX, EntryHeight, TextHue, "Z:");
            AddTextEntry(x + 16, y, CoordWidth - 16, EntryHeight, TextHue, 2, p.Z.ToString());
            x += CoordWidth + OffsetSize;

            if (SetGumpID != 0)
            {
                AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
            }

            AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 3);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Point3D toSet;
            bool shouldSet, shouldSend;

            switch (info.ButtonID)
            {
                case 1: // Current location
                    {
                        toSet = m_Mobile.Location;
                        shouldSet = true;
                        shouldSend = true;

                        break;
                    }
                case 2: // Pick location
                    {
                        m_Mobile.Target = new InternalTarget(m_Property, m_Mobile, m_Object, m_PropertiesGump);

                        toSet = Point3D.Zero;
                        shouldSet = false;
                        shouldSend = false;

                        break;
                    }
                case 3: // Use values
                    {
                        toSet = new Point3D(
                            Utility.ToInt32(info.GetTextEntry(0)),
                            Utility.ToInt32(info.GetTextEntry(1)),
                            Utility.ToInt32(info.GetTextEntry(2))
                        );
                        shouldSet = true;
                        shouldSend = true;

                        break;
                    }
                default:
                    {
                        toSet = Point3D.Zero;
                        shouldSet = false;
                        shouldSend = true;

                        break;
                    }
            }

            if (shouldSet)
            {
                try
                {
                    CommandLogging.LogChangeProperty(m_Mobile, m_Object, m_Property.Name, toSet.ToString());
                    m_Property.SetValue(m_Object, toSet, null);
                    m_PropertiesGump.OnValueChanged(m_Object, m_Property);
                }
                catch
                {
                    m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                }
            }

            if (shouldSend)
            {
                m_PropertiesGump.SendPropertiesGump();
            }
        }

        private class InternalTarget : Target
        {
            private readonly Mobile m_Mobile;
            private readonly object m_Object;
            private readonly PropertyInfo m_Property;
            private readonly PropertiesGump m_PropertiesGump;

            public InternalTarget(
                PropertyInfo prop, Mobile mobile, object o, PropertiesGump propertiesGump
            ) : base(-1, true, TargetFlags.None)
            {
                m_PropertiesGump = propertiesGump;
                m_Property = prop;
                m_Mobile = mobile;
                m_Object = o;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D p)
                {
                    try
                    {
                        CommandLogging.LogChangeProperty(m_Mobile, m_Object, m_Property.Name, new Point3D(p).ToString());
                        m_Property.SetValue(m_Object, new Point3D(p), null);
                        m_PropertiesGump.OnValueChanged(m_Object, m_Property);
                    }
                    catch
                    {
                        m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
                    }
                }
            }

            protected override void OnTargetFinish(Mobile from) => m_PropertiesGump.SendPropertiesGump();
        }
    }
}
