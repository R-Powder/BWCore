using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace BWCR;

public class Relation :IExposable,ILoadReferenceable
{
    public RelationTypeDef relationType;
    public Faction Item1 => relationItems[relationType.relationItems[0]];
    public Faction Item2 => relationItems[relationType.relationItems[1]];
    public Dictionary<RelationItemDef,Faction> relationItems;
    public virtual void ExposeData()
    {
        Scribe_Defs.Look(ref relationType, "relationType");
        Scribe_Collections.Look(ref relationItems, "relationItems", LookMode.Def, LookMode.Reference);
    }
    public virtual void PostMake()
    {
    }

    [NotNull]
    public string GetUniqueLoadID()
    {
        var text = "BW_Relation_" + relationType.defName;
        return relationItems.Values.Aggregate(text, (current, f) => current + $"_{f.loadID}");
    }
}

public class OverlordVassalRelation :Relation
{
    public OverlordVassalRelation()
    { 
        kind = new Dictionary<Faction, byte> {{Vassal,1},{Overlord,1}};
        baseGoodwill = new Dictionary<Faction, int> {{Vassal,0},{Overlord,0}};
        naturalGoodwill =new Dictionary<Faction, int> {{Vassal,0},{Overlord,0}};
    }
   
    public Faction Overlord => Item1;
    public Faction Vassal => Item2;
    public void CheckKindThresholds(out bool sentLetter)
    {
        //var item = relationItems[RFRDefOf.BWCR_Overlord];
        sentLetter = false;
        if (kind[Vassal] != (byte)VassalRelationKind.Rebellious && baseGoodwill[Vassal] <= -75)
            kind[Vassal] = (byte)VassalRelationKind.Rebellious;
        if (kind[Vassal] != (byte)VassalRelationKind.Loyal && baseGoodwill[Vassal] >= 75)
            kind[Vassal] = (byte)VassalRelationKind.Loyal;
        if (kind[Vassal] == (byte)VassalRelationKind.Rebellious && baseGoodwill[Vassal] >= 0)
            kind[Vassal] = (byte)VassalRelationKind.Reserve;
        if (kind[Vassal] == (byte)VassalRelationKind.Loyal && baseGoodwill[Vassal] <= 0)
            kind[Vassal] = (byte)VassalRelationKind.Reserve;
        
        if (kind[Overlord] != (byte)OverlordRelationKind.Domineering && baseGoodwill[Overlord] <= -75)
            kind[Overlord] = (byte)OverlordRelationKind.Domineering;
        if (kind[Overlord] != (byte)OverlordRelationKind.Protective && baseGoodwill[Overlord] >= 75)
            kind[Overlord] = (byte)OverlordRelationKind.Protective;
        if (kind[Overlord] == (byte)OverlordRelationKind.Domineering && baseGoodwill[Overlord] >= 0)
            kind[Overlord] = (byte)OverlordRelationKind.Tolerance;
        if (kind[Overlord] == (byte)OverlordRelationKind.Protective && baseGoodwill[Overlord] <= 0)
            kind[Overlord] = (byte)OverlordRelationKind.Tolerance;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref kind, "kind",LookMode.Reference,LookMode.Value);
        Scribe_Collections.Look(ref baseGoodwill, "basegoodwill",LookMode.Reference,LookMode.Value);
        Scribe_Values.Look(ref goodWillLocked, "goodWillLocked");
        Scribe_Collections.Look(ref naturalGoodwill, "naturalGoodwill",LookMode.Reference,LookMode.Value);
        BackCompatibility.PostExposeData(this);
    }

    public override string ToString() => string.Concat("(overlord=", Overlord, ", vassal=", Vassal, ")");
    public bool goodWillLocked;
    public Dictionary<Faction,byte> kind;
    public Dictionary<Faction,int> baseGoodwill;
    public Dictionary<Faction,int> naturalGoodwill;


    public enum VassalRelationKind :byte
    {
        Loyal,Reserve,Rebellious
    }
    public enum OverlordRelationKind :byte
    {
        Protective,Tolerance,Domineering
    }

    public bool GoodWillLocked => goodWillLocked;
}
public class GroupPartRelation:Relation
{
    public Faction Group => Item1;
    public Faction Part => Item2;
    
    public override void ExposeData()
    {
        base.ExposeData();
        BackCompatibility.PostExposeData(this);
    }
    public override string ToString() => string.Concat("(group=", Group, ", part=", Part, ")");
}
public class ParentChildRelation:Relation
{
    public Faction Parent => Item1;
    public Faction Child => Item2;
    public override void ExposeData()
    {
        base.ExposeData();
        BackCompatibility.PostExposeData(this);
    }

    public override string ToString() => string.Concat("(parent=", Parent, ", child=", Child, ")");
}