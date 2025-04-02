using System;
using System.Collections.Generic;
using Verse;

namespace BWCR;

public class RelationTypeDef : Def
{
    public Type relationClass = typeof(Relation);
    public List<RelationItemDef> relationItems;
    public bool createAlly = false;
}