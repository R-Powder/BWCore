using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BWCR;

public class Dialog_RelationInfo : Window
{
	private Vector2 scrollPosition = Vector2.zero;
	private RelationTabDef curTabInt;
	private float viewWidth;
	private Faction curFactionInt;
	private List<RelationTabDef> tempRelationTabs = [];
	private List<Relation> tempRelationsList = [];
	List<TabRecord> tabRecords = [];
	private float scrollViewWidth;
	private bool fixSize;
	private ScrollPositioner scrollPositioner = new();
	
	public RelationTabDef CurTab
	{
		get => curTabInt;
		set
		{
			Log.Message($"pre tab:{curTabInt}");
			if (value == curTabInt) return;
			curTabInt = value;
			Log.Message($"aft tab:{curTabInt}");
			scrollPosition = Vector2.zero;
			if (value==BWCRDefOf.Vanilla)return;
			tempRelationsList.Clear();
			foreach (var ri in RelationFactionUtil.RelationItems)
			{
				if (ri.tab != value) continue;
				var reDict = RelationFactionUtil.GetRFRComponent().Relations;
				if (!reDict.ContainsKey(ri)||!reDict[ri].ContainsKey(CurFaction)||reDict[ri][CurFaction].NullOrEmpty())continue;
				tempRelationsList.AddRange(reDict[ri][CurFaction]);
				//Log.Message($"now add:{reDict[ri][CurFaction]}");
			}
		}
	}

	private Faction CurFaction
	{
		get => curFactionInt;
		set
		{
			//Log.Message($"value : {value} , curFactionInt : {curFactionInt}");
			if (value == curFactionInt) return;
            curFactionInt = value;
            tempRelationTabs=RelationFactionUtil.GetRFRComponent().Relations.ToList().FindAll(x => x.Value.ContainsKey(value)).Select(pair => pair.Key.tab).ToList();
            Log.Message($"tempRelationTabs :{tempRelationTabs.Count}");
            CurTab = BWCRDefOf.Vanilla;
            tabRecords.Clear();
            tabRecords.Add(new TabRecord(BWCRDefOf.Vanilla.label.Translate(), delegate { CurTab = BWCRDefOf.Vanilla; },()=>CurTab == BWCRDefOf.Vanilla));
            if (!tempRelationTabs.NullOrEmpty())
	            foreach (var tab in tempRelationTabs) 
		            tabRecords.Add(new TabRecord(tab.label.Translate(), delegate{ CurTab = tab; }, ()=>CurTab == tab));
            //Log.Message($"tabRecords :{tabRecords.Count}");
            
		}
	}
	public Dialog_RelationInfo()
	{
		doCloseButton = true;
		forcePause = true;
		doCloseX = true;
		CurFaction = Faction.OfPlayer;
	}
	public override Vector2 InitialSize => new(1000f, 400f);
	private bool showAll;
	public override void DoWindowContents(Rect inRect)
	{
		if (fixSize is false)
		{
			windowRect.yMin -= 80f;
			windowRect.yMax += 80f;
			fixSize = true;
		}
		var layoutRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - CloseButSize.y);
		var font = Text.Font;
		var anchor = Text.Anchor;
		
		var rect1 = new Rect(0.0f, 0.0f, layoutRect.width, layoutRect.height);
		Widgets.BeginGroup(rect1);
		TabDrawer.DrawTabs(new Rect(90f, 34, inRect.width-210, inRect.height), tabRecords);
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		
		if (Prefs.DevMode) Widgets.CheckboxLabeled(new Rect(rect1.width - 120f, 0.0f, 120f, 24f), "DEV: Show all", ref showAll);
		else showAll = false;
		var rectBottom = new Rect(0, 0, 90f,50f);
		
		if (Widgets.ButtonText(rectBottom, CurFaction.Name))
		{
			var menuOptions = !showAll?Find.FactionManager.AllFactionsVisibleInViewOrder.Select(faction => new FloatMenuOption(faction.Name, () => CurFaction = faction,faction.def.FactionIcon, faction.Color)).ToList()
				:Find.FactionManager.AllFactions.Select(faction => new FloatMenuOption(faction.Name, () => CurFaction = faction,faction.def.FactionIcon, faction.Color)).ToList();
			Find.WindowStack.Add(new FloatMenu(menuOptions));
		}
		var outRect = new Rect(0.0f, 80f, rect1.width, rect1.height - 80f);
		
		//Log.Message($"{CurTab} , {CurTab.drawTabUI.Method} , {CurFaction} , {tempRelationsList} , {tempRelationTabs}");
		if (CurTab == BWCRDefOf.Vanilla)
			CurTab.drawTabUI(outRect, ref scrollPosition,ref scrollViewWidth,null, CurFaction , showAll);
		else CurTab.drawTabUI(outRect,ref scrollPosition,ref scrollViewWidth, tempRelationsList,CurTab, showAll);
		
		Widgets.EndGroup();
		Text.Font = font;
		Text.Anchor = anchor;
	}

	public static void DrawFactionList(Rect inRect,ref Vector2 scrollPosition,ref float scrollViewWidth,List<Relation> relationList,object itemTabDef = null,bool showAll = false)
	{
		var rect2 = new Rect(0.0f, 0.0f,scrollViewWidth , inRect.height - 16f);
		if (relationList.Count > 0)
		{
			inRect.yMin += Text.LineHeight;
			Widgets.ScrollHorizontal(inRect, ref scrollPosition, rect2);
			Widgets.BeginScrollView(inRect, ref scrollPosition, rect2);
			var num1 = 0.0f;
			var num2 = 0;
			foreach (var relation in relationList)
			{
				if (num2 % 2 == 1)
					Widgets.DrawLightHighlight(new Rect(num1,rect2.x, 80f,rect2.height));
				var itemDef = relation.relationType.relationItems.FirstOrDefault(x=>x.tab!=itemTabDef);
				num1 += RelationUIUtil.DrawRelationCol(itemDef, relation, num1, rect2);
				++num2;
			}
			if (Event.current.type == EventType.Layout)
				scrollViewWidth = num1;
			Widgets.EndScrollView();
		}
		else
		{
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "NoRelation".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
	public static void DrawFactionListWithDetail(Rect inRect,ref Vector2 scrollPosition,ref float scrollViewWidth ,List<Relation> relationList,object itemTabDef = null,bool showAll = false)
	{
		var rect2 = new Rect(0.0f, 0.0f,scrollViewWidth , inRect.height - 16f);
		if (relationList.Count > 0)
		{
			inRect.yMin += Text.LineHeight;
			Widgets.ScrollHorizontal(inRect, ref scrollPosition, rect2);
			Widgets.BeginScrollView(inRect, ref scrollPosition, rect2);
			var num1 = 0.0f;
			var num2 = 0;
			foreach (var relation in relationList)
			{
				if (num2 % 2 == 1)
					Widgets.DrawLightHighlight(new Rect(num1,rect2.x, 80f,rect2.height ));
				var itemDef = relation.relationType.relationItems.FirstOrDefault(x=>x.tab==itemTabDef);
				num1 += RelationUIUtil.DrawRelationCol(itemDef,relation, num1, rect2);
				++num2;
			}
			if (Event.current.type == EventType.Layout)
				scrollViewWidth = num1;
			Widgets.EndScrollView();
		}
		else
		{
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "NoRelation".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
	public static void DrawFactionListWithDetailVanilla(Rect inRect,ref Vector2 scrollPosition,ref float scrollViewWidth,List<Relation> _,object other,bool showAll = false)
	{
		var rect2 = new Rect(0.0f, 0.0f,scrollViewWidth , inRect.height - 16f);
		var visibleFactions= Find.FactionManager.AllFactionsInViewOrder.Where(faction => !faction.Hidden || showAll).ToList();
		if (visibleFactions.Count > 0 && !(visibleFactions.Count==1 && visibleFactions[0] == other as Faction))
		{
			inRect.yMin += Text.LineHeight;
			Widgets.ScrollHorizontal(inRect, ref scrollPosition, rect2);
			Widgets.BeginScrollView(inRect, ref scrollPosition, rect2);
			var num1 = 0.0f;
			var num2 = 0;
			foreach (var visibleFaction in visibleFactions.Where(visibleFaction => visibleFaction != other as Faction).Where(visibleFaction => !visibleFaction.Hidden || showAll))
			{
				if (num2 % 2 == 1)
					Widgets.DrawLightHighlight(new Rect(num1,rect2.y, 80f,rect2.height ));
				num1 += RelationUIUtil.DrawFactionCol(other as Faction, visibleFaction, num1, rect2);
				++num2;
			}
			if (Event.current.type == EventType.Layout)
				scrollViewWidth = num1;
			Widgets.EndScrollView();
		}
		else
		{
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(inRect, "NoFactions".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}