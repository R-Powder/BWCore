using System;

namespace BWCR;

public static class RelationMaker
{
    public static Relation MakeRelation(RelationTypeDef def)
    {
        var instance = Activator.CreateInstance(def.relationClass) as Relation;
        instance.relationType = def;
        instance.PostMake();
        return instance;
    }
}