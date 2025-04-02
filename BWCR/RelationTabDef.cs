using Verse;

namespace BWCR;

public class RelationTabDef :Def
{
    [MustTranslate]
    public string generalTitle = "";
    [MustTranslate]
    public string generalDescription = "";
    public bool visibleByDefault = true;
    public int minMonolithLevelVisible = -1;
    public string tutorTag;
    public DrawTabUI drawTabUI = Dialog_RelationInfo.DrawFactionList;
}