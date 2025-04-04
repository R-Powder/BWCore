using System;
using Verse;

namespace BWCR;

public class RelationGoodwillSituationDef : Def
{
    public RelationGoodwillSituationWorker Worker
    {
        get
        {
            if (workerInt == null)
            {
                workerInt = (RelationGoodwillSituationWorker)Activator.CreateInstance(workerClass);
                workerInt.def = this;
            }
            return workerInt;
        }
    }
    public Type workerClass = typeof(RelationGoodwillSituationWorker); 
    public int baseMaxGoodwill = 100;
    public RelationTypeDef type; 
    public RelationItemDef item; 
    public RelationItemDef otherItem; 
    public int naturalGoodwillOffset; 
    public bool versusAll; 
    [Unsaved] 
    private RelationGoodwillSituationWorker workerInt;
}