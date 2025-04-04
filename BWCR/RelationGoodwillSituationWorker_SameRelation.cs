using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BWCR;

public class RelationGoodwillSituationWorker_SameRelation:RelationGoodwillSituationWorker
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
        Applies(Faction.OfPlayer, other);

    private bool Applies(Faction a, Faction b)
    {
        HashSet<Faction> hashSet = [];
        foreach (var relation in RelationFactionUtil.GetRFRComponent().GetRelation(a, def.item).ToList())
            hashSet.Add(relation.relationItems[def.otherItem]);
        return Enumerable.Any(RelationFactionUtil.GetRFRComponent().GetRelation(b, def.item).ToList(), relation => hashSet.Contains(relation.relationItems[def.otherItem]));
    }
}