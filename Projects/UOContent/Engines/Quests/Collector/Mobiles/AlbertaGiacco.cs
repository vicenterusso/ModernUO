using ModernUO.Serialization;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Collector;

[SerializationGenerator(0, false)]
public partial class AlbertaGiacco : BaseQuester
{
    [Constructible]
    public AlbertaGiacco() : base("the respected painter")
    {
    }

    public override string DefaultName => "Alberta Giacco";

    public override void InitBody()
    {
        InitStats(100, 100, 25);

        Hue = 0x83F2;

        Female = true;
        Body = 0x191;
    }

    public override void InitOutfit()
    {
        AddItem(new FancyShirt());
        AddItem(new Skirt(0x59B));
        AddItem(new Boots());
        AddItem(new FeatheredHat(0x59B));
        AddItem(new FullApron(0x59B));

        HairItemID = 0x203D; // Pony Tail
        HairHue = 0x457;
    }

    public override bool CanTalkTo(PlayerMobile to) =>
        to.Quest is CollectorQuest qs && (qs.IsObjectiveInProgress(typeof(FindAlbertaObjective))
                                          || qs.IsObjectiveInProgress(typeof(SitOnTheStoolObjective))
                                          || qs.IsObjectiveInProgress(typeof(ReturnPaintingObjective)));

    public override void OnTalk(PlayerMobile player, bool contextMenu)
    {
        var qs = player.Quest;

        if (qs is CollectorQuest)
        {
            Direction = GetDirectionTo(player);

            var obj = qs.FindObjective<FindAlbertaObjective>();

            if (obj?.Completed == false)
            {
                obj.Complete();
            }
            else if (qs.IsObjectiveInProgress(typeof(SitOnTheStoolObjective)))
            {
                qs.AddConversation(new AlbertaStoolConversation());
            }
            else if (qs.IsObjectiveInProgress(typeof(ReturnPaintingObjective)))
            {
                qs.AddConversation(new AlbertaAfterPaintingConversation());
            }
        }
    }
}
