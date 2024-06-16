using ModernUO.Serialization;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Collector;

[SerializationGenerator(0, false)]
public partial class GabrielPiete : BaseQuester
{
    [Constructible]
    public GabrielPiete() : base("the renowned minstrel")
    {
    }

    public override string DefaultName => "Gabriel Piete";

    public override void InitBody()
    {
        InitStats(100, 100, 25);

        Hue = 0x83EF;

        Female = false;
        Body = 0x190;
    }

    public override void InitOutfit()
    {
        AddItem(new FancyShirt());
        AddItem(new LongPants(0x5F7));
        AddItem(new Shoes(0x5F7));

        HairItemID = 0x2049; // Pig Tails
        HairHue = 0x460;

        FacialHairItemID = 0x2041; // Mustache
        FacialHairHue = 0x460;
    }

    public override bool CanTalkTo(PlayerMobile to) =>
        to.Quest is CollectorQuest qs && (qs.IsObjectiveInProgress(typeof(FindGabrielObjective))
                                          || qs.IsObjectiveInProgress(typeof(FindSheetMusicObjective))
                                          || qs.IsObjectiveInProgress(typeof(ReturnSheetMusicObjective))
                                          || qs.IsObjectiveInProgress(typeof(ReturnAutographObjective)));

    public override void OnTalk(PlayerMobile player, bool contextMenu)
    {
        var qs = player.Quest;

        if (qs is not CollectorQuest)
        {
            return;
        }

        Direction = GetDirectionTo(player);

        if (qs.FindObjective<FindGabrielObjective>() is { Completed: false } obj1)
        {
            obj1.Complete();
            return;
        }

        if (qs.IsObjectiveInProgress(typeof(FindSheetMusicObjective)))
        {
            qs.AddConversation(new GabrielNoSheetMusicConversation());
            return;
        }

        if (qs.FindObjective<ReturnSheetMusicObjective>() is { Completed: false } obj2)
        {
            obj2.Complete();
            return;
        }

        if (qs.IsObjectiveInProgress(typeof(ReturnAutographObjective)))
        {
            qs.AddConversation(new GabrielIgnoreConversation());
        }
    }
}
