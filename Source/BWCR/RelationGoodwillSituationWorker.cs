using RimWorld;
using Verse;

namespace BWCR;

public abstract class RelationGoodwillSituationWorker
{
    public virtual string GetPostProcessedLabel(Faction other) => def.label;
    public string GetPostProcessedLabelCap(Faction other) => GetPostProcessedLabel(other).CapitalizeFirst(def);
    public virtual int GetMaxGoodwill(Faction other) => 100;
    public virtual int GetNaturalGoodwillOffset(Faction other) => 0;

    public RelationGoodwillSituationDef def;
}