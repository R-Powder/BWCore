using System.Collections.Generic;
using BWCore;
using UnityEngine;
using Verse;

namespace BWCR;

public class BWCRSetting:BWModuleSetting
{
	public override void ExposeData()
	{
		Scribe_Collections.Look(ref defaultAllowRelationItems, "defaultAllowRelationItems", LookMode.Def);
		Scribe_Collections.Look(ref defaultDisallowRelationItems, "defaultDisallowRelationItems", LookMode.Def);
	}
	public override void DoWindowContents(ref Rect inRect)
	{
		var listing_Standard = new Listing_Standard();
		listing_Standard.Begin(inRect);
		Text.Font = GameFont.Small;
		listing_Standard.Gap();
		listing_Standard.End();
	}
	public static HashSet<RelationItemDef> defaultAllowRelationItems = [BWCRDefOf.BWCR_Child, BWCRDefOf.BWCR_Parent, BWCRDefOf.BWCR_Overlord, BWCRDefOf.BWCR_Vassal];
	public static HashSet<RelationItemDef> defaultDisallowRelationItems = [BWCRDefOf.BWCR_Group];
	
}