using System;
using System.Net;
using Server.Gumps;
using Server.Network;

namespace Server.Factions
{
    public class ElectionManagementGump : Gump
    {
        public const int LabelColor = 0xFFFFFF;
        private readonly Candidate m_Candidate;

        private readonly Election m_Election;
        private readonly int m_Page;

        public ElectionManagementGump(Election election, Candidate candidate = null, int page = 0) : base(40, 40)
        {
            m_Election = election;
            m_Candidate = candidate;
            m_Page = page;

            AddPage(0);

            if (candidate != null)
            {
                AddBackground(0, 0, 448, 354, 9270);
                AddAlphaRegion(10, 10, 428, 334);

                AddHtml(10, 10, 428, 20, "Candidate Management".Center(LabelColor));

                AddHtml(45, 35, 100, 20, "Player Name:".Color(LabelColor));
                AddHtml(145, 35, 100, 20, (candidate.Mobile == null ? "null" : candidate.Mobile.Name).Color(LabelColor));

                AddHtml(45, 55, 100, 20, "Vote Count:".Color(LabelColor));
                AddHtml(145, 55, 100, 20, candidate.Votes.ToString().Color(LabelColor));

                AddButton(12, 73, 4005, 4007, 1);
                AddHtml(45, 75, 100, 20, "Drop Candidate".Color(LabelColor));

                AddImageTiled(13, 99, 422, 242, 9264);
                AddImageTiled(14, 100, 420, 240, 9274);
                AddAlphaRegion(14, 100, 420, 240);

                AddHtml(14, 100, 420, 20, "Voters".Center(LabelColor));

                if (page > 0)
                {
                    AddButton(397, 104, 0x15E3, 0x15E7, 2);
                }
                else
                {
                    AddImage(397, 104, 0x25EA);
                }

                if ((page + 1) * 10 < candidate.Voters.Count)
                {
                    AddButton(414, 104, 0x15E1, 0x15E5, 3);
                }
                else
                {
                    AddImage(414, 104, 0x25E6);
                }

                AddHtml(14, 120, 30, 20, "DEL".Center(LabelColor));
                AddHtml(47, 120, 150, 20, "Name".Color(LabelColor));
                AddHtml(195, 120, 100, 20, "Address".Center(LabelColor));
                AddHtml(295, 120, 80, 20, "Time".Center(LabelColor));
                AddHtml(355, 120, 60, 20, "Legit".Center(LabelColor));

                var idx = 0;

                for (var i = page * 10; i >= 0 && i < candidate.Voters.Count && i < (page + 1) * 10; ++i, ++idx)
                {
                    var voter = candidate.Voters[i];

                    AddButton(13, 138 + idx * 20, 4002, 4004, 4 + i);

                    var fields = voter.AcquireFields();

                    var x = 45;

                    for (var j = 0; j < fields.Length; ++j)
                    {
                        var obj = fields[j];

                        if (obj is Mobile mobile)
                        {
                            AddHtml(x + 2, 140 + idx * 20, 150, 20, mobile.Name.Color(LabelColor));
                            x += 150;
                        }
                        else if (obj is IPAddress)
                        {
                            AddHtml(x, 140 + idx * 20, 100, 20, obj.ToString().Center(LabelColor));
                            x += 100;
                        }
                        else if (obj is DateTime time)
                        {
                            AddHtml(
                                x,
                                140 + idx * 20,
                                80,
                                20,
                                FormatTimeSpan(time - election.LastStateTime).Center(LabelColor)
                            );
                            x += 80;
                        }
                        else if (obj is int i1)
                        {
                            AddHtml(x, 140 + idx * 20, 60, 20, $"{i1}%".Center(LabelColor));
                            x += 60;
                        }
                    }
                }
            }
            else
            {
                AddBackground(0, 0, 288, 334, 9270);
                AddAlphaRegion(10, 10, 268, 314);

                AddHtml(10, 10, 268, 20, "Election Management".Center(LabelColor));

                AddHtml(45, 35, 100, 20, "Current State:".Color(LabelColor));
                AddHtml(145, 35, 100, 20, election.State.ToString().Color(LabelColor));

                AddButton(12, 53, 4005, 4007, 1);
                AddHtml(45, 55, 100, 20, "Transition Time:".Color(LabelColor));
                AddHtml(145, 55, 100, 20, FormatTimeSpan(election.NextStateTime).Color(LabelColor));

                AddImageTiled(13, 79, 262, 242, 9264);
                AddImageTiled(14, 80, 260, 240, 9274);
                AddAlphaRegion(14, 80, 260, 240);

                AddHtml(14, 80, 260, 20, "Candidates".Center(LabelColor));
                AddHtml(14, 100, 30, 20, "-->".Center(LabelColor));
                AddHtml(47, 100, 150, 20, "Name".Color(LabelColor));
                AddHtml(195, 100, 80, 20, "Votes".Center(LabelColor));

                for (var i = 0; i < election.Candidates.Count; ++i)
                {
                    var cd = election.Candidates[i];
                    var mob = cd.Mobile;

                    if (mob == null)
                    {
                        continue;
                    }

                    AddButton(13, 118 + i * 20, 4005, 4007, 2 + i);
                    AddHtml(47, 120 + i * 20, 150, 20, mob.Name.Color(LabelColor));
                    AddHtml(195, 120 + i * 20, 80, 20, cd.Votes.ToString().Center(LabelColor));
                }
            }
        }

        public static string FormatTimeSpan(TimeSpan ts) =>
            $"{ts.Days:D2}:{ts.Hours % 24:D2}:{ts.Minutes % 60:D2}:{ts.Seconds % 60:D2}";

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var from = sender.Mobile;
            var bid = info.ButtonID;

            if (m_Candidate == null)
            {
                if (bid > 1)
                {
                    bid -= 2;

                    if (bid < m_Election.Candidates.Count)
                    {
                        from.SendGump(new ElectionManagementGump(m_Election, m_Election.Candidates[bid]));
                    }
                }
            }
            else if (bid == 0)
            {
                from.SendGump(new ElectionManagementGump(m_Election));
            }
            else if (bid == 1)
            {
                m_Election.RemoveCandidate(m_Candidate.Mobile);
                from.SendGump(new ElectionManagementGump(m_Election));
            }
            else if (bid == 2 && m_Page > 0)
            {
                from.SendGump(new ElectionManagementGump(m_Election, m_Candidate, m_Page - 1));
            }
            else if (bid == 3 && (m_Page + 1) * 10 < m_Candidate.Voters.Count)
            {
                from.SendGump(new ElectionManagementGump(m_Election, m_Candidate, m_Page + 1));
            }
            else
            {
                bid -= 4;

                if (bid >= 0 && bid < m_Candidate.Voters.Count)
                {
                    m_Candidate.Voters.RemoveAt(bid);
                    from.SendGump(new ElectionManagementGump(m_Election, m_Candidate, m_Page));
                }
            }
        }
    }
}
