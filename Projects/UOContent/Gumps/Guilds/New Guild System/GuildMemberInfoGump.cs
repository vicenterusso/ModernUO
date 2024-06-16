using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Guilds
{
    public class GuildMemberInfoGump : BaseGuildGump
    {
        private readonly PlayerMobile m_Member;
        private readonly bool m_toKick;
        private readonly bool m_ToLeader;

        public GuildMemberInfoGump(
            PlayerMobile pm, Guild g, PlayerMobile member, bool toKick, bool toPromoteToLeader
        ) : base(pm, g, 10, 40)
        {
            m_ToLeader = toPromoteToLeader;
            m_toKick = toKick;
            m_Member = member;
            PopulateGump();
        }

        public override void PopulateGump()
        {
            AddPage(0);

            AddBackground(0, 0, 350, 255, 0x242C);
            AddHtmlLocalized(20, 15, 310, 26, 1063018, 0x0); // <div align=center><i>Guild Member Information</i></div>
            AddImageTiled(20, 40, 310, 2, 0x2711);

            AddHtmlLocalized(20, 50, 150, 26, 1062955, 0x0, true); // <i>Name</i>
            AddHtml(180, 53, 150, 26, m_Member.Name);

            AddHtmlLocalized(20, 80, 150, 26, 1062956, 0x0, true); // <i>Rank</i>
            AddHtmlLocalized(180, 83, 150, 26, m_Member.GuildRank.Name, 0x0);

            AddHtmlLocalized(20, 110, 150, 26, 1062953, 0x0, true); // <i>Guild Title</i>
            AddHtml(180, 113, 150, 26, m_Member.GuildTitle);
            AddImageTiled(20, 142, 310, 2, 0x2711);

            AddBackground(20, 150, 310, 26, 0x2486);
            AddButton(25, 155, 0x845, 0x846, 4);
            AddHtmlLocalized(
                50,
                153,
                270,
                26,
                m_Member == player.GuildFealty && guild.Leader != m_Member ? 1063082 : 1062996,
                0x0
            ); // Clear/Cast Vote For This Member

            AddBackground(20, 180, 150, 26, 0x2486);
            AddButton(25, 185, 0x845, 0x846, 1);
            AddHtmlLocalized(50, 183, 110, 26, 1062993, m_ToLeader ? 0x990000 : 0); // Promote

            AddBackground(180, 180, 150, 26, 0x2486);
            AddButton(185, 185, 0x845, 0x846, 3);
            AddHtmlLocalized(210, 183, 110, 26, 1062995, 0x0); // Set Guild Title

            AddBackground(20, 210, 150, 26, 0x2486);
            AddButton(25, 215, 0x845, 0x846, 2);
            AddHtmlLocalized(50, 213, 110, 26, 1062994, 0x0); // Demote

            AddBackground(180, 210, 150, 26, 0x2486);
            AddButton(185, 215, 0x845, 0x846, 5);
            AddHtmlLocalized(210, 213, 110, 26, 1062997, m_toKick ? 0x5000 : 0); // Kick
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (sender.Mobile is not PlayerMobile pm || !IsMember(pm, guild) || !IsMember(m_Member, guild))
            {
                return;
            }

            var playerRank = pm.GuildRank;
            var targetRank = m_Member.GuildRank;

            switch (info.ButtonID)
            {
                case 1: // Promote
                    {
                        if (playerRank.GetFlag(RankFlags.CanPromoteDemote) &&
                            (playerRank.Rank - 1 > targetRank.Rank ||
                             playerRank == RankDefinition.Leader && playerRank.Rank > targetRank.Rank))
                        {
                            targetRank = RankDefinition.Ranks[targetRank.Rank + 1];

                            if (targetRank == RankDefinition.Leader)
                            {
                                if (m_ToLeader)
                                {
                                    m_Member.GuildRank = targetRank;
                                    pm.SendLocalizedMessage(
                                        1063156,
                                        m_Member.Name
                                    ); // The guild information for ~1_val~ has been updated.
                                    pm.SendLocalizedMessage(
                                        1063156,
                                        pm.Name
                                    ); // The guild information for ~1_val~ has been updated.
                                    guild.Leader = m_Member;
                                }
                                else
                                {
                                    pm.SendLocalizedMessage(
                                        1063144
                                    ); // Are you sure you wish to make this member the new guild leader?
                                    pm.SendGump(new GuildMemberInfoGump(player, guild, m_Member, false, true));
                                }
                            }
                            else
                            {
                                m_Member.GuildRank = targetRank;
                                pm.SendLocalizedMessage(
                                    1063156,
                                    m_Member.Name
                                ); // The guild information for ~1_val~ has been updated.
                            }
                        }
                        else
                        {
                            pm.SendLocalizedMessage(1063143); // You don't have permission to promote this member.
                        }

                        break;
                    }
                case 2: // Demote
                    {
                        if (playerRank.GetFlag(RankFlags.CanPromoteDemote) && playerRank.Rank > targetRank.Rank)
                        {
                            if (targetRank == RankDefinition.Lowest)
                            {
                                if (RankDefinition.Lowest.Name.Number == 1062963)
                                {
                                    pm.SendLocalizedMessage(1063333); // You can't demote a ronin.
                                }
                                else
                                {
                                    pm.SendMessage($"You can't demote a {RankDefinition.Lowest.Name}.");
                                }
                            }
                            else
                            {
                                m_Member.GuildRank = RankDefinition.Ranks[targetRank.Rank - 1];
                                pm.SendLocalizedMessage(
                                    1063156,
                                    m_Member.Name
                                ); // The guild information for ~1_val~ has been updated.
                            }
                        }
                        else
                        {
                            pm.SendLocalizedMessage(1063146); // You don't have permission to demote this member.
                        }

                        break;
                    }
                case 3: // Set Guild title
                    {
                        if (playerRank.GetFlag(RankFlags.CanSetGuildTitle) &&
                            (playerRank.Rank > targetRank.Rank || m_Member == player))
                        {
                            pm.SendLocalizedMessage(
                                1011128
                            ); // Enter the new title for this guild member or 'none' to remove a title:

                            pm.BeginPrompt(SetTitle_Callback);
                        }
                        else if (m_Member.GuildTitle == null || m_Member.GuildTitle.Length <= 0)
                        {
                            pm.SendLocalizedMessage(
                                1070746
                            ); // You don't have the permission to set that member's guild title.
                        }
                        else
                        {
                            pm.SendLocalizedMessage(
                                1063148
                            ); // You don't have permission to change this member's guild title.
                        }

                        break;
                    }
                case 4: // Vote
                    {
                        if (m_Member == pm.GuildFealty && guild.Leader != m_Member)
                        {
                            pm.SendLocalizedMessage(1063158); // You have cleared your vote for guild leader.
                        }
                        else if (guild.CanVote(m_Member))
                        {
                            if (m_Member == guild.Leader)
                            {
                                pm.SendLocalizedMessage(1063424); // You can't vote for the current guild leader.
                            }
                            else if (!guild.CanBeVotedFor(m_Member))
                            {
                                pm.SendLocalizedMessage(1063425); // You can't vote for an inactive guild member.
                            }
                            else
                            {
                                pm.GuildFealty = m_Member;
                                pm.SendLocalizedMessage(
                                    1063159,
                                    m_Member.Name
                                ); // You cast your vote for ~1_val~ for guild leader.
                            }
                        }
                        else
                        {
                            pm.SendLocalizedMessage(1063149); // You don't have permission to vote.
                        }

                        break;
                    }
                case 5: // Kick
                    {
                        if (playerRank.GetFlag(RankFlags.RemovePlayers) && playerRank.Rank > targetRank.Rank ||
                            playerRank.GetFlag(RankFlags.RemoveLowestRank) && targetRank == RankDefinition.Lowest)
                        {
                            if (m_toKick)
                            {
                                guild.RemoveMember(m_Member);
                                pm.SendLocalizedMessage(1063157); // The member has been removed from your guild.
                            }
                            else
                            {
                                pm.SendLocalizedMessage(
                                    1063152
                                ); // Are you sure you wish to kick this member from the guild?
                                pm.SendGump(new GuildMemberInfoGump(player, guild, m_Member, true, false));
                            }
                        }
                        else
                        {
                            pm.SendLocalizedMessage(1063151); // You don't have permission to remove this member.
                        }

                        break;
                    }
            }
        }

        public void SetTitle_Callback(Mobile from, string text)
        {
            if (from is not PlayerMobile pm || m_Member == null)
            {
                return;
            }

            if (m_Member.Guild is not Guild g || !IsMember(pm, g) ||
                !(pm.GuildRank.GetFlag(RankFlags.CanSetGuildTitle) &&
                  (pm.GuildRank.Rank > m_Member.GuildRank.Rank || pm == m_Member)))
            {
                if (m_Member.GuildTitle == null || m_Member.GuildTitle.Length <= 0)
                {
                    pm.SendLocalizedMessage(1070746); // You don't have the permission to set that member's guild title.
                }
                else
                {
                    pm.SendLocalizedMessage(1063148); // You don't have permission to change this member's guild title.
                }

                return;
            }

            var title = text.AsSpan().Trim().FixHtml();

            if (title.Length > 20)
            {
                from.SendLocalizedMessage(501178); // That title is too long.
            }
            else if (!CheckProfanity(title))
            {
                from.SendLocalizedMessage(501179); // That title is disallowed.
            }
            else
            {
                if (title.InsensitiveEquals("none"))
                {
                    m_Member.GuildTitle = null;
                }
                else
                {
                    m_Member.GuildTitle = title;
                }

                pm.SendLocalizedMessage(1063156, m_Member.Name); // The guild information for ~1_val~ has been updated.
            }
        }
    }
}
