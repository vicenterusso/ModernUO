using System;
using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Network;

namespace Server.Gumps
{
    public class LoginDevGump : Gump
    {

        public static void OnLogin(Mobile m)
        {

            if (!m.Alive)
            {
                return;
            }

            Console.WriteLine("LoginDevGump:OnLogin()");
            m.SendGump(new LoginDevGump(0));
        }

        private static List<string> pages =
        [
            "EXILE SHARD\n\nBem vindo! Estamos em estágio alpha de desenvolvimento focando na jogabilidade e balanceamento. " +
            "O combate foi herdado da antiga Mystic (m2a) e estamos abertos sugestões e críticas. Aproveite o jogo e divirta-se!\n\n" +
            "Como estamos em fase de testes, o servidor pode ser reiniciado a qualquer momento sem aviso prévio. os comandos disponíves são:\n\n" +
            ".starterkit - Criação de kits prontos para teste de combate para várias classes\n" +
            ".edits - Edição livre de skills e stats, respeitando os limites do servidor\n" +
            "\n\n\nBom divertimento",


            // "Vivamus luctus urna sed urna ultricies ac tempor dui sagittis. In condimentum...",
            //
            // "Curabitur gravida, erat sit amet sodales tristique, nisi ligula facilisis dolor..."
            // Add more pages as needed

        ];

        private int m_Page;

        public LoginDevGump(int page) : base(50, 50)
        {
            m_Page = page;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 400, 400, 0xE10);

            AddHtml(20, 20, 360, 360, pages[m_Page], false, true);

            // if (m_Page > 0)
            // {
            //     AddButton(20, 250, 4014, 4015, 1, GumpButtonType.Reply, 0); // Previous page
            //     AddLabel(60, 250, 1153, "Previous");
            // }
            //
            // if (m_Page < pages.Count - 1)
            // {
            //     AddButton(320, 250, 4005, 4006, 2, GumpButtonType.Reply, 0); // Next page
            //     AddLabel(360, 250, 1153, "Next");
            // }
        }

        public override void OnResponse(NetState sender, in RelayInfo relayInfo)
        {
            Mobile m = sender.Mobile;
            switch (relayInfo.ButtonID)
            {
                case 1:
                    m.SendGump(new LoginDevGump(m_Page - 1));
                    break;
                case 2:
                    m.SendGump(new LoginDevGump(m_Page + 1));
                    break;
            }
        }
    }
}
