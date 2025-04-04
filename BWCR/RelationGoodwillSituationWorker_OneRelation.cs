using System.Linq;
using RimWorld;
using Verse;

namespace BWCR;

public class RelationGoodwillSituationWorker_OneRelation:RelationGoodwillSituationWorker
{
    public override string GetPostProcessedLabel(Faction other)
    {
        if (def != null) return base.GetPostProcessedLabel(other);
        return Applies(Faction.OfPlayer, other)
            ? "RelationGoodwillImpact_Player".Translate(base.GetPostProcessedLabel(other))
            : "RelationGoodwillImpact_Other".Translate(base.GetPostProcessedLabel(other));
    }

    public override int GetNaturalGoodwillOffset(Faction other) => 
        !Applies(other) ? 0 : def.naturalGoodwillOffset;

    private bool Applies(Faction other) => 
        Applies(Faction.OfPlayer, other) || Applies(other, Faction.OfPlayer);

    private bool Applies(Faction a, Faction b)
    {
        return RelationFactionUtil.GetRFRComponent().GetRelation(a,def.item).ToList().
            Any(x=> x.relationType == def.type && x.relationItems.
                Any(y=>y.Key == def.otherItem&&y.Value==b));
    }
}