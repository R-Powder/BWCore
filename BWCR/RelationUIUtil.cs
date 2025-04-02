using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace BWCR;

public static class RelationUIUtil
{
    
    
    private static List<Ideo> _ideos = [];
    public static float DrawFactionCol(Faction itself, Faction faction, float colX, Rect fillRect)
    {
	    var rect = new Rect(colX, 90f, 80f, 80f);
		Text.Font = GameFont.Small;
		Text.Anchor = TextAnchor.MiddleCenter;
		GUI.color = faction.Color;
		GUI.DrawTexture(new Rect(colX + (rect.width - 42f) / 2f, 24f, 42f, 42f), faction.def.FactionIcon);
		GUI.color = Color.white;
		Widgets.Label(rect, (string)(faction.Name.CapitalizeFirst() + "\n" + faction.def.LabelCap) );
		
		var factionRect = new Rect(colX, 0f, 80f, rect.yMax);
	    if (Mouse.IsOver(factionRect)) 
	    {
		    var tipSignal = new TipSignal(() => string.Concat(faction.Name.Colorize(ColoredText.TipSectionTitleColor), "\n", faction.def.LabelCap.Resolve(), "\n\n", faction.def.Description), faction.loadID ^ 1938473043);
				TooltipHandler.TipRegion(factionRect, tipSignal);
				Widgets.DrawHighlight(factionRect);
	    }
	    if (Widgets.ButtonInvisible(factionRect, false)) Find.WindowStack.Add(new Dialog_InfoCard(faction));
	    
	    //info
	    var infoRect = new Rect(colX, rect.yMax, 80f, 40f);
	    Widgets.InfoCardButtonCentered(infoRect, faction);
	    
	    //ideology
	    var ideoRect = new Rect(colX, infoRect.yMax, 80f, 60f);
	    /*
	    if (ModsConfig.IdeologyActive && !Find.IdeoManager.classicMode)
	    {
		    if (faction.ideos != null)
		    {
			    _ideos.Clear();
			    var num2 = ideoRect.x; var num3 = ideoRect.y;
			    if (faction.ideos.PrimaryIdeo != null)
			    {
				    if (num2 + 40f > ideoRect.xMax)
				    {
					    num2 = ideoRect.x; num3 += 45f;
				    }
				    var rect6 = new Rect(num2, num3 + (ideoRect.height - 40f) / 2f, 40f, 40f);
				    IdeoUIUtility.DoIdeoIcon(rect6, faction.ideos.PrimaryIdeo, true, delegate
				    {
					    IdeoUIUtility.OpenIdeoInfo(faction.ideos.PrimaryIdeo);
				    });
				    num2 += rect6.width + 5f;
				    num2 = ideoRect.x;
				    num3 += 45f;
			    }
			    _ideos = faction.ideos.IdeosMinorListForReading;
			    int k;
			    int i;
			    for (i = 0; i < _ideos.Count; i = k + 1)
			    {
				    if (num2 + 22f > ideoRect.xMax)
				    {
					    num2 = ideoRect.x;
					    num3 += 27f;
				    }
				    if (num3 + 22f > ideoRect.yMax) break;
				    var rect7 = new Rect(num2, num3 + (ideoRect.height - 22f) / 2f, 22f, 22f);
				    IdeoUIUtility.DoIdeoIcon(rect7, _ideos[i], true, delegate
				    { IdeoUIUtility.OpenIdeoInfo(_ideos[i]); });
				    num2 += rect7.width + 5f;
				    k = i;
			    }
		    }
	    }else
	    */
	    ideoRect.height = 0f;
	    
	    
	    var rect8 = new Rect(colX, ideoRect.yMax, 80f, 70f);
	    var text2 = faction.RelationKindWith(itself).GetLabelCap();
		if (faction.defeated) text2 = text2.Colorize(ColorLibrary.Grey);
	    GUI.color = faction.RelationKindWith(itself).GetColor();
	    Text.Anchor = TextAnchor.MiddleCenter; 
	    if (faction.HasGoodwill && !faction.def.permanentEnemy)
	    {
		    Widgets.Label(new Rect(rect8.x, rect8.y - 10f, rect8.width, rect8.height), text2);
		    Text.Font = GameFont.Medium;
		    Widgets.Label(new Rect(rect8.x, rect8.y + 10f, rect8.width, rect8.height), faction.GoodwillWith(itself).ToStringWithSign());
		    Text.Font = GameFont.Small;
	    }
	    else Widgets.Label(rect8, text2);
	    GenUI.ResetLabelAlign();
	    GUI.color = Color.white;
	    if (Mouse.IsOver(rect8))
	    {
		    TaggedString taggedString = "";
		    if (faction.def.permanentEnemy) taggedString = "CurrentGoodwillTip_PermanentEnemy".Translate();
		    else if (faction.HasGoodwill)
		    {
			    taggedString = "Goodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + (faction.GoodwillWith(itself).ToStringWithSign() + ", " + faction.RelationKindWith(itself).GetLabel()).Colorize(faction.RelationKindWith(itself).GetColor());
			    //var ongoingEvents = GetOngoingEvents(faction);
			    //if (!ongoingEvents.NullOrEmpty()) taggedString += "\n\n" + "OngoingEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + ongoingEvents;
			    //var recentEvents = FactionUIUtility.GetRecentEvents(faction);
			    //if (!recentEvents.NullOrEmpty()) taggedString += "\n\n" + "RecentEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + recentEvents;
			    var text3 = "";
			    switch (faction.RelationKindWith(itself))
			    {
				    case FactionRelationKind.Hostile:
					    text3 = "CurrentGoodwillTip_Hostile".Translate(0.ToString("F0"));
					    break;
				    case FactionRelationKind.Neutral:
					    text3 = "CurrentGoodwillTip_Neutral".Translate((-75).ToString("F0"), 75.ToString("F0"));
					    break;
				    case FactionRelationKind.Ally:
					    text3 = "CurrentGoodwillTip_Ally".Translate(0.ToString("F0"));
					    break;
			    }
			    taggedString += "\n\n" + text3.Colorize(ColoredText.SubtleGrayColor);
		    }
		    if (taggedString != "") TooltipHandler.TipRegion(rect8, taggedString);
		    Widgets.DrawHighlight(rect8);
	    }
	    
	    /*
	    var rect9 = new Rect(colX, rect8.yMax, 80f, 54f);
	    if (faction.HasGoodwill && !faction.def.permanentEnemy)
	    {
		    var relationKindForGoodwill = GetRelationKindForGoodwill(faction.NaturalGoodwill);
		    GUI.color = relationKindForGoodwill.GetColor();
		    var rect10 = rect9.ContractedBy(7f);
		    rect10.x = colX + 30f;
		    rect10.width = 20f;
		    Text.Anchor = TextAnchor.UpperCenter;
		    Widgets.DrawRectFast(rect10, Color.black);
		    Widgets.Label(rect10, faction.NaturalGoodwill.ToStringWithSign());
		    GenUI.ResetLabelAlign();
		    GUI.color = Color.white;
		    if (Mouse.IsOver(rect9))
		    {
			    TaggedString taggedString2 = "NaturalGoodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + faction.NaturalGoodwill.ToStringWithSign().Colorize(relationKindForGoodwill.GetColor());
			    var num4 = Mathf.Clamp(faction.NaturalGoodwill - 50, -100, 100);
			    var num5 = Mathf.Clamp(faction.NaturalGoodwill + 50, -100, 100);
			    taggedString2 += string.Concat("\n", "NaturalGoodwillRange".Translate().Colorize(ColoredText.TipSectionTitleColor), ": ", num4.ToString().Colorize(GetRelationKindForGoodwill(num4).GetColor()), " ") + "RangeTo".Translate() + " " + num5.ToString().Colorize(GetRelationKindForGoodwill(num5).GetColor());
			    var naturalGoodwillExplanation = GetNaturalGoodwillExplanation(faction);
			    if (!naturalGoodwillExplanation.NullOrEmpty()) taggedString2 += "\n\n" + "AffectedBy".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n" + naturalGoodwillExplanation;
			    taggedString2 += "\n\n" + "NaturalGoodwillDescription".Translate(1.25f.ToStringPercent()).Colorize(ColoredText.SubtleGrayColor);
			    TooltipHandler.TipRegion(rect9, taggedString2);
			    Widgets.DrawHighlight(rect9);
		    }
	    }*/
	    Text.Anchor = TextAnchor.UpperLeft;
	    return 80f;
	}
    private static FactionRelationKind GetRelationKindForGoodwill(int goodwill) => 
	    goodwill <= -75 ? FactionRelationKind.Hostile : goodwill >= 75 ? FactionRelationKind.Ally : FactionRelationKind.Neutral;

    private static TaggedString GetOngoingEvents(Faction other)
    {
	    var sb = new StringBuilder();
	    var situations = Find.GoodwillSituationManager.GetSituations(other);
	    for (var index = 0; index < situations.Count; ++index)
	    {
		    if (situations[index].maxGoodwill >= 100) continue;
		    var text = "- " + situations[index].def.Worker.GetPostProcessedLabelCap(other) + ": " + (situations[index].maxGoodwill.ToStringWithSign() + " " + "max".Translate()).Colorize(FactionRelationKind.Hostile.GetColor());
		    sb.AppendInNewLine(text);
	    }
	    return (TaggedString) sb.ToString();
    }
    private static TaggedString GetNaturalGoodwillExplanation(Faction other)
    {
	    var sb = new StringBuilder();
	    var situations = Find.GoodwillSituationManager.GetSituations(other);
	    for (var index = 0; index < situations.Count; ++index)
	    {
		    if (situations[index].naturalGoodwillOffset == 0) continue;
		    var text = "- " + situations[index].def.Worker.GetPostProcessedLabelCap(other) + ": " + situations[index].naturalGoodwillOffset.ToStringWithSign().Colorize(situations[index].naturalGoodwillOffset >= 0 ? FactionRelationKind.Ally.GetColor() : FactionRelationKind.Hostile.GetColor());
		    sb.AppendInNewLine(text);
	    }
	    return (TaggedString) sb.ToString();
    }

    public static float DrawRelationCol(RelationItemDef itemDef,Relation relation, float colX, Rect rect2)
    {
	    var faction = relation.relationItems[itemDef];
	    var rect = new Rect(colX, 90f, 80f, 80f);
		Text.Font = GameFont.Small;
		Text.Anchor = TextAnchor.MiddleCenter;
		GUI.color = faction.Color;
		GUI.DrawTexture(new Rect(colX + (rect.width - 42f) / 2f, 24f, 42f, 42f), faction.def.FactionIcon);
		GUI.color = Color.white;
		Widgets.Label(rect, faction.Name.CapitalizeFirst() + "\n" + faction.def.LabelCap );
		
		var factionRect = new Rect(colX, 0f, 80f, rect.yMax);
	    if (Mouse.IsOver(factionRect)) 
	    {
		    var tipSignal = new TipSignal(() => string.Concat(faction.Name.Colorize(ColoredText.TipSectionTitleColor), "\n", faction.def.LabelCap.Resolve(), "\n\n", faction.def.Description), faction.loadID ^ 1938473043);
				TooltipHandler.TipRegion(factionRect, tipSignal);
				Widgets.DrawHighlight(factionRect);
	    }
	    if (Widgets.ButtonInvisible(factionRect, false)) Find.WindowStack.Add(new Dialog_InfoCard(faction));

	    var infoRect = new Rect(colX, rect.yMax, 80f, 40f);
	    Widgets.InfoCardButtonCentered(infoRect, faction);

	    var ideoRect = new Rect(colX, infoRect.yMax, 80f, 60f);
	    ideoRect.height = 0f;
	    
	    var rect8 = new Rect(colX, ideoRect.yMax, 80f, 80f);
	    var text2 = itemDef.LabelCap;
		if (faction.defeated) text2 = text2.Colorize(ColorLibrary.Grey);
		GUI.color = Color.white; //faction.RelationKindWith(itself).GetColor();
	    Text.Anchor = TextAnchor.MiddleCenter; 
	    /*
	    if (faction.HasGoodwill && !faction.def.permanentEnemy)
	    {
		    Widgets.Label(new Rect(rect8.x, rect8.y - 10f, rect8.width, rect8.height), text2);
		    Text.Font = GameFont.Medium;
		    //Widgets.Label(new Rect(rect8.x, rect8.y + 10f, rect8.width, rect8.height), faction.GoodwillWith(itself).ToStringWithSign());
		    Text.Font = GameFont.Small;
	    }
	    else*/ 
	    GUI.color = RelationFactionUtil.GetRFRComponent().OriginalColors.ContainsKey(faction)? RelationFactionUtil.GetRFRComponent().OriginalColors[faction]:faction.Color;
	    GUI.DrawTexture(new Rect(colX + (rect8.width - 42f) / 2f, rect8.y+24f, 42f, 42f), itemDef.ExpandingIconTexture ?? Texture2D.blackTexture);
	    GUI.color = Color.white;
	    Widgets.Label(new Rect(rect8.x, rect8.y, rect8.width, 24f), text2);
	    GenUI.ResetLabelAlign();
	    GUI.color = Color.white;
	    if (Mouse.IsOver(rect8))
	    {
		    TaggedString taggedString = "";
		    /*
		    if (faction.def.permanentEnemy) taggedString = "CurrentGoodwillTip_PermanentEnemy".Translate();
		    else if (faction.HasGoodwill)
		    {
			    taggedString = "Goodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + (faction.GoodwillWith(itself).ToStringWithSign() + ", " + faction.RelationKindWith(itself).GetLabel()).Colorize(faction.RelationKindWith(itself).GetColor());
			    var text3 = "";
			    taggedString += "\n\n" + text3.Colorize(ColoredText.SubtleGrayColor);
		    }*/
		    if (taggedString != "") TooltipHandler.TipRegion(rect8, taggedString);
		    Widgets.DrawHighlight(rect8);
	    }

	    Text.Anchor = TextAnchor.UpperLeft;
	    return 80f;
    }
}