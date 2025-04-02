using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BWCR;

public class BWCRelationExtension : DefModExtension
{
    public bool activeFlavor = false;
    public List<RelationItemDef> allowItems = [];
    public List<RelationItemDef> disallowItems = [];
    public RelationItemDef onlyItems;
    public FactionDef defaultGroup = null;
}