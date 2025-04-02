using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BWCR;

public static class RelationFactionUtil
{
	public static List<RelationTabDef> RelationTabs => relationTabs ??= DefDatabase<RelationTabDef>.AllDefs.ToList();
	private static List<RelationTabDef> relationTabs;
	/*
	public static void Notify_VassalRelationKindChanged(this Faction itself,Faction other,
		FactionRelationKind previousKind,
		bool canSendLetter,
		string reason,
		GlobalTargetInfo lookTarget,
		out bool sentLetter)
	{
		if (Current.ProgramState != ProgramState.Playing || other != Faction.OfPlayer) canSendLetter = false;
		sentLetter = false;
		ColoredText.ClearCache();
		var newKind = itself.VassalGoodwillWith(other);
		if (newKind == FactionRelationKind.Hostile)
		{
			if (Current.ProgramState == ProgramState.Playing)
			{
				foreach (var pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive.ToList<Pawn>().Where(pawn => pawn.Faction == itself && pawn.HostFaction == other || pawn.Faction == other && pawn.HostFaction == itself))
				{
					pawn.guest.SetGuestStatus(pawn.HostFaction, GuestStatus.Prisoner);
				}
			}
			if (other == Faction.OfPlayer) QuestUtility.SendQuestTargetSignals(itself.questTags, "BecameHostileToPlayer", itself.Named("SUBJECT"));
		}
		if (other == Faction.OfPlayer && !itself.HostileTo(Faction.OfPlayer))
		{
			var sites = Find.WorldObjects.Sites;
			var siteList = sites.Where(t => t.factionMustRemainHostile && t.Faction == itself && !t.HasMap).ToList();
			if (siteList.Any())
			{
				string label;
				string text;
				if (siteList.Count == 1)
				{
					label = "LetterLabelSiteNoLongerHostile".Translate();
					text = "LetterSiteNoLongerHostile".Translate(itself.NameColored, (NamedArgument) siteList[0].Label);
				}
				else
				{
					var stringBuilder = new StringBuilder();
					foreach (var t in siteList)
					{
						if (stringBuilder.Length != 0) stringBuilder.AppendLine();
						stringBuilder.Append("  - " + t.LabelCap);
						var component = t.GetComponent<ImportantPawnComp>();
						if (component != null && component.pawn.Any) stringBuilder.Append(" (" + component.pawn[0].LabelCap + ")");
					}
					label = "LetterLabelSiteNoLongerHostileMulti".Translate();
					text = "LetterSiteNoLongerHostileMulti".Translate(itself.NameColored) + ":\n\n" + stringBuilder;
				}
				Find.LetterStack.ReceiveLetter((TaggedString) label, (TaggedString) text, LetterDefOf.NeutralEvent, new LookTargets(siteList.Select(x => new GlobalTargetInfo(x.Tile))));
				foreach (var t in siteList) t.Destroy();
			}
		}
		if (other == Faction.OfPlayer && itself.HostileTo(Faction.OfPlayer))
		{
			var allWorldObjects = Find.WorldObjects.AllWorldObjects;
			foreach (var component in from t in allWorldObjects where t.Faction == itself select t.GetComponent<TradeRequestComp>() into component where component != null && component.ActiveRequest select component)
			{
				component.Disable();
			}
			foreach (var map in Find.Maps) map.passingShipManager.RemoveAllShipsOfFaction(itself);
		}
		if (canSendLetter)
		{
			var text = (TaggedString) "";
			itself.TryAppendRelationKindChangedInfo(ref text, previousKind, newKind, reason);
			switch (newKind)
			{
				case FactionRelationKind.Hostile:
					Find.LetterStack.ReceiveLetter("LetterLabelRelationsChange_Hostile".Translate((NamedArgument) itself.Name), text, LetterDefOf.NegativeEvent, (LookTargets) lookTarget, itself);
					break;
				case FactionRelationKind.Neutral:
					if (previousKind == FactionRelationKind.Hostile)
					{
						Find.LetterStack.ReceiveLetter("LetterLabelRelationsChange_NeutralFromHostile".Translate((NamedArgument) itself.Name), text, LetterDefOf.PositiveEvent, (LookTargets) lookTarget, itself);
						break;
					}
					Find.LetterStack.ReceiveLetter("LetterLabelRelationsChange_NeutralFromAlly".Translate((NamedArgument) itself.Name), text, LetterDefOf.NeutralEvent, (LookTargets) lookTarget, itself);
					break;
				case FactionRelationKind.Ally:
					Find.LetterStack.ReceiveLetter("LetterLabelRelationsChange_Ally".Translate((NamedArgument) itself.Name), text, LetterDefOf.PositiveEvent, (LookTargets) lookTarget, itself);
					break;
				default: throw new ArgumentOutOfRangeException();
			}
			sentLetter = true;
		}
		if (Current.ProgramState != ProgramState.Playing) return;
		var maps = Find.Maps;
		foreach (var t in maps)
		{
			t.attackTargetsCache.Notify_FactionHostilityChanged(itself, other);
			var lordManager = t.lordManager;
			foreach (var lord in lordManager.lords)
			{
				if (lord.faction == other) lord.Notify_FactionRelationsChanged(itself, previousKind);
				else if (lord.faction == itself) lord.Notify_FactionRelationsChanged(other, previousKind);
			}
		}
	}*/
	public static bool TryAffectOverlordVassalGoodwillWith(this Faction overlord, Faction vassal, int overlordGoodwillChange, int vassalGoodwillChange)
	{
		if (!CanChangeOverlordGoodwillFor(overlord, vassal, overlordGoodwillChange)) return false;
		if (!CanChangeVassalGoodwillFor(vassal, overlord, vassalGoodwillChange)) return false;
		if (overlordGoodwillChange == 0 && vassalGoodwillChange == 0) return true;
		overlordGoodwillChange = CalculateAdjustedOverlordGoodwillChange(overlord, vassal, overlordGoodwillChange);
		vassalGoodwillChange = CalculateAdjustedVassalGoodwillChange(vassal, overlord, vassalGoodwillChange);
		var onum2 = BaseOverlordGoodwillWith(overlord, vassal);
		var onum3 = Mathf.Clamp(onum2 + overlordGoodwillChange, -100, 100);
		
		var vnum2 = BaseOverlordGoodwillWith(overlord, vassal);
		var vnum3 = Mathf.Clamp(onum2 + overlordGoodwillChange, -100, 100);
		if (onum2 == onum3 && vnum2 == vnum3) return true;
		
		var relation = OverlordRelationWith(overlord, vassal);
		relation.baseGoodwill[overlord] = onum3;
		relation.baseGoodwill[vassal] = vnum3;
		relation.CheckKindThresholds(out _);
		return true;
	}
	public static bool TryAffectOverlordVassalGoodwillWith(this Faction overlord, Faction vassal, int goodwillChange)
	{
		if (!CanChangeOverlordGoodwillFor(overlord, vassal, goodwillChange)) return false;
		if (!CanChangeVassalGoodwillFor(vassal, overlord, goodwillChange)) return false;
		if (goodwillChange == 0) return true;
		goodwillChange = CalculateAdjustedOverlordGoodwillChange(overlord, vassal, goodwillChange);
		var num2 = BaseOverlordGoodwillWith(overlord, vassal);
		var num3 = Mathf.Clamp(num2 + goodwillChange, -100, 100);
		if (num2 == num3) return true;
		var relation = OverlordRelationWith(overlord, vassal);
		relation.baseGoodwill[overlord] = num3;
		relation.baseGoodwill[vassal] = num3;
		relation.CheckKindThresholds(out _);
		return true;
	}
	public static bool TryAffectOverlordGoodwillWith(this Faction itself, Faction other, int goodwillChange,
		bool canSendMessage = true, bool canSendHostilityLetter = true, HistoryEventDef reason = null,
		GlobalTargetInfo? lookTarget = null)
	{
		if (!CanChangeOverlordGoodwillFor(itself, other, goodwillChange)) return false;
		if (goodwillChange == 0) return true;
		goodwillChange = CalculateAdjustedOverlordGoodwillChange(itself, other, goodwillChange);
		var num2 = BaseOverlordGoodwillWith(itself, other);
		var num3 = Mathf.Clamp(num2 + goodwillChange, -100, 100);
		if (num2 == num3) return true;
		var relation = OverlordRelationWith(itself, other);
		relation.baseGoodwill[itself] = num3;
		relation.CheckKindThresholds(out _);
		return true;
	}

	public static bool TryAffectVassalGoodwillWith(this Faction itself, Faction other, int goodwillChange,
		bool canSendMessage = true, bool canSendHostilityLetter = true, HistoryEventDef reason = null,
		GlobalTargetInfo? lookTarget = null)
	{
		if (!CanChangeVassalGoodwillFor(itself, other, goodwillChange)) return false;
		if (goodwillChange == 0) return true;
		goodwillChange = CalculateAdjustedVassalGoodwillChange(itself, other, goodwillChange);
		var num2 = itself.BaseVassalGoodwillWith(other);
		var num3 = Mathf.Clamp(num2 + goodwillChange, -100, 100);
		if (num2 == num3) return true;
		var vassalRelation = VassalRelationWith(itself, other);
		vassalRelation.baseGoodwill[itself] = num3;
		vassalRelation.CheckKindThresholds(out _);
		return true;
	}

	public static bool CanChangeOverlordGoodwillFor(Faction itself, Faction other, int goodwillChange) =>
		HasOverlordVassalRelation(itself, other) && IsValuable(itself) && IsValuable(other) &&
		!itself.def.permanentEnemy && !other.def.permanentEnemy && !itself.defeated && !other.defeated &&
		other != itself && (!itself.def.permanentEnemyToEveryoneExceptPlayer || other.IsPlayer) &&
		(!other.def.permanentEnemyToEveryoneExceptPlayer || itself.IsPlayer) &&
		(itself.def.permanentEnemyToEveryoneExcept == null ||
		 itself.def.permanentEnemyToEveryoneExcept.Contains(other.def)) &&
		(other.def.permanentEnemyToEveryoneExcept == null ||
		 other.def.permanentEnemyToEveryoneExcept.Contains(itself.def)) && (goodwillChange <= 0 ||
		                                                                    ((!itself.IsPlayer || !SettlementUtility.IsPlayerAttackingAnySettlementOf(other)) &&
		                                                                     (!other.IsPlayer || !SettlementUtility.IsPlayerAttackingAnySettlementOf(itself)))) &&
		!itself.OverlordGoodWillLocked(other);

	public static bool CanChangeVassalGoodwillFor(Faction itself, Faction other, int goodwillChange) =>
		HasOverlordVassalRelation(other, itself) && IsValuable(itself) && IsValuable(other) &&
		!itself.def.permanentEnemy && !other.def.permanentEnemy && !itself.defeated && !other.defeated &&
		other != itself && (!itself.def.permanentEnemyToEveryoneExceptPlayer || other.IsPlayer) &&
		(!other.def.permanentEnemyToEveryoneExceptPlayer || itself.IsPlayer) &&
		(itself.def.permanentEnemyToEveryoneExcept == null ||
		 itself.def.permanentEnemyToEveryoneExcept.Contains(other.def)) &&
		(other.def.permanentEnemyToEveryoneExcept == null ||
		 other.def.permanentEnemyToEveryoneExcept.Contains(itself.def)) && (goodwillChange <= 0 ||
		                                                                    ((!itself.IsPlayer || !SettlementUtility.IsPlayerAttackingAnySettlementOf(other)) &&
		                                                                     (!other.IsPlayer || !SettlementUtility.IsPlayerAttackingAnySettlementOf(itself)))) &&
		!itself.VassalGoodWillLocked();

	private static bool IsValuable(Faction itself) => !itself.Hidden && !itself.temporary;
	public static bool VassalGoodWillLocked(this OverlordVassalRelation vassalRelation) => 
		vassalRelation.goodWillLocked;
	public static bool OverlordGoodWillLocked(this OverlordVassalRelation overlordRelation) =>
		overlordRelation.goodWillLocked;

	public static bool VassalGoodWillLocked(this Faction vassal) => 
		vassal.GetVassalRelation().GoodWillLocked;

	public static bool OverlordGoodWillLocked(this Faction overlord, Faction vassal) =>
		overlord.OverlordRelationWith(vassal).GoodWillLocked;

	//==========================================
	private static List<RelationTypeDef> relationKinds;
	public static List<RelationTypeDef> RelationKinds => relationKinds ??= DefDatabase<RelationTypeDef>.AllDefsListForReading;
	private static List<RelationItemDef> relationItems;
	public static List<RelationItemDef> RelationItems => relationItems ??= DefDatabase<RelationItemDef>.AllDefsListForReading;
	
	private static Dictionary<RelationItemDef, Predicate<Faction, RelationItemDef>> specialRelationItemConditions;

	public static Dictionary<RelationItemDef, Predicate<Faction, RelationItemDef>> SpecialRelationItemConditions =>
		specialRelationItemConditions ??= new Dictionary<RelationItemDef, Predicate<Faction, RelationItemDef>>();

	public static void AddSpecialRelationItemConditionDelegate(RelationItemDef item, Predicate<Faction, RelationItemDef> condition) => SpecialRelationItemConditions[item] = condition;

	public static bool CheckNotConflict(this Faction faction,RelationItemDef item)
	{
		var comp = GetRFRComponent();
		return item.conflictItems.NullOrEmpty()||item.conflictItems.All(i => !comp.IsItem(faction, i));
	}
	public static bool CheckAnyNeed(this Faction faction,RelationItemDef item)
	{
		var comp = GetRFRComponent();
		return item.needItems.NullOrEmpty() || GenCollection.Any(item.needItems, i => comp.IsItem(faction, i));
	}
	public static bool CheckAllNeed(this Faction faction,RelationItemDef item)
	{
		var comp = GetRFRComponent();
		return item.needItems.NullOrEmpty() ||item.needItems.All(i => comp.IsItem(faction, i));
	}
	
	public static bool CheckItemCondition(this Faction faction, RelationItemDef item)
	{
		return faction.CheckNotConflict(item) && faction.CheckAnyNeed(item);
	}
	public static byte CheckExtensionAllow(this Faction faction,RelationItemDef item)
	{
		if (!faction.def.HasModExtension<BWCRelationExtension>()) return 0;
		var ext = faction.def.GetModExtension<BWCRelationExtension>();
		if (ext.onlyItems != null) return ext.onlyItems == item ? (byte)1 : (byte)2;
		if (!ext.allowItems.NullOrEmpty() && ext.allowItems.Contains(item)) return 1;
		if (!ext.disallowItems.NullOrEmpty() && ext.disallowItems.Contains(item)) return 2;
		return 0;
	}
	public static byte CheckSettingAllow(RelationItemDef item)
	{
		if (!BWCRSetting.defaultAllowRelationItems.NullOrEmpty() && BWCRSetting.defaultAllowRelationItems.Contains(item)) return 1;
		if (!BWCRSetting.defaultDisallowRelationItems.NullOrEmpty() && BWCRSetting.defaultDisallowRelationItems.Contains(item)) return 2;
		return 0;
	}

	public static bool CheckAllRelationItemCondition(this Faction faction, RelationItemDef item)
	{
		if (SpecialRelationItemConditions.ContainsKey(item) &&
		    !SpecialRelationItemConditions[item].Invoke(faction, item)) return false; 
		Log.Message($"check : {faction}:{faction.CheckItemCondition(item)},{faction.CheckSettingCondition(item)}");
		return faction.CheckItemCondition(item) && faction.CheckSettingCondition(item);
	}
	public static bool CheckSettingCondition(this Faction faction, RelationItemDef item)
	{
		var check1 = faction.CheckExtensionAllow(item);
		return check1 == 1 || (check1 == 0 && CheckSettingAllow(item) == 1);
	}
	public static bool HasOverlordVassalRelation(Faction overlord, Faction vassal)
	{
		var relations = overlord.GetOverlordRelations().ToList();
		return !relations.NullOrEmpty() && relations.All(x => x.Vassal != vassal)
		                                      && vassal.GetVassalRelation()?.Overlord==overlord; 	       
	}
	public static bool HasGroupPartRelation(Faction group, Faction part)
	{
		var relations = group.GetGroupRelations().ToList();
		return !relations.NullOrEmpty() && relations.All(x => x.Part != part)
		                                && part.GetPartRelation()?.Group==group;
	}
	public static bool HasParentChildRelation(Faction parent, Faction child)
	{
		var relations = parent.GetParentRelations().ToList();
		return !relations.NullOrEmpty() && relations.All(x => x.Child != child)
		                                && child.GetChildRelation()?.Parent==parent;
	}		
	//========================================
	public static int BaseOverlordGoodwillWith(this Faction itself, Faction other) => 
		OverlordRelationWith(itself, other).baseGoodwill[itself];

	public static int BaseVassalGoodwillWith(this Faction itself, Faction other) => 
		VassalRelationWith(itself, other).baseGoodwill[itself];

	//=============================================================
	
	public static OverlordVassalRelation VassalRelationWith(this Faction itself, Faction other, bool allowNull = false)
	{
		if (other == itself)
		{
			Log.Error("Tried to get relation between faction " + itself + " and itself.");
			return new OverlordVassalRelation();
		}
		var v = GetVassalRelation(itself);
		if (v.Overlord == other) return v;
		if (allowNull) return null;
		Log.Error(string.Concat("Faction ", itself.Name, " has null relation with ", other, ". Returning dummy relation."));
		return new OverlordVassalRelation();
	}
	public static OverlordVassalRelation OverlordRelationWith(this Faction itself, Faction other, bool allowNull = false)
	{
		if (other == itself)
		{
			Log.Error("Tried to get relation between faction " + itself + " and itself.");
			return new OverlordVassalRelation();
		}
		var v = GetOverlordRelations(itself).FirstOrDefault(x => x.Vassal==other);
		if (v != null && v.Vassal == other) return v;
		if (allowNull) return null;
		Log.Error(string.Concat("Faction ", itself.Name, " has null relation with ", other, ". Returning dummy relation."));
		return new OverlordVassalRelation();
	}
	//===========================================================
	public static int CalculateAdjustedOverlordGoodwillChange(Faction itself, Faction other, int goodwillChange)
	{
		var num = BaseOverlordGoodwillWith(itself, other);
		if (!itself.IsPlayer && !other.IsPlayer) return goodwillChange;
		var num2 = NaturalOverlordGoodwill(itself,other);
		if ((num2 >= num || goodwillChange >= 0) && (num2 <= num || goodwillChange <= 0)) return goodwillChange;
		var num3 = Mathf.Min(Mathf.Abs(num - num2), Mathf.Abs(goodwillChange));
		var num4 = Mathf.RoundToInt(0.25f * num3);
		if (goodwillChange < 0) num4 = -num4;

		goodwillChange += num4;
		return goodwillChange;
	}
	public static int CalculateAdjustedVassalGoodwillChange(Faction itself, Faction other, int goodwillChange)
	{
		var num = BaseVassalGoodwillWith(itself, other);
		if (!itself.IsPlayer && !other.IsPlayer) return goodwillChange;
		var num2 = (itself.IsPlayer ? NaturalVassalGoodwill(other) : NaturalVassalGoodwill(itself));
		if ((num2 >= num || goodwillChange >= 0) && (num2 <= num || goodwillChange <= 0)) return goodwillChange;
		var num3 = Mathf.Min(Mathf.Abs(num - num2), Mathf.Abs(goodwillChange));
		var num4 = Mathf.RoundToInt(0.25f * num3);
		if (goodwillChange < 0) num4 = -num4;

		goodwillChange += num4;
		return goodwillChange;
	}
	public static int NaturalOverlordGoodwill(Faction itself,Faction vassal) => 
		itself.OverlordRelationWith(vassal).naturalGoodwill[itself];

	public static int NaturalVassalGoodwill(Faction itself) => 
		itself.GetVassalRelation().naturalGoodwill[itself];

	//==========================================================================
	private static List<Predicate<Faction>> subFactionConditions = [IsChild,IsPart,IsVassal];
	public static List<Predicate<Faction>> SubFactionConditions => subFactionConditions;
	public static void AddSubFactionCondition(Predicate<Faction> condition) => subFactionConditions.Add(condition);
	public static void RemoveSubFactionCondition(Predicate<Faction> condition) => subFactionConditions.Remove(condition);
	public static bool IsSubFaction(this Faction itself) => !Enumerable.Any(SubFactionConditions, subFactionCondition => !subFactionCondition.Invoke(itself));

	public static bool IsChild(this Faction itself) => 
		GetRFRComponent().Relations[BWCRDefOf.BWCR_Child].ContainsKey(itself);

	public static bool IsParent(this Faction itself) => 
		GetRFRComponent().Relations[BWCRDefOf.BWCR_Parent].ContainsKey(itself);

	public static bool IsVassal(this Faction itself) => 
		GetRFRComponent().Relations[BWCRDefOf.BWCR_Vassal].ContainsKey(itself);

	public static bool IsOverlord(this Faction itself) => 
		GetRFRComponent().Relations[BWCRDefOf.BWCR_Overlord].ContainsKey(itself);

	public static bool IsPart(this Faction itself) => 
		GetRFRComponent().Relations[BWCRDefOf.BWCR_Group].ContainsKey(itself);

	public static bool IsGroup(this Faction itself) => 
		GetRFRComponent().Relations[BWCRDefOf.BWCR_Part].ContainsKey(itself);

	//=================================================================
	public static OverlordVassalRelation GetVassalRelation(this Faction vassal) =>
		GetRFRComponent().GetVassalRelation(vassal);

	public static IEnumerable<OverlordVassalRelation> GetOverlordRelations(this Faction overlord) =>
		GetRFRComponent().GetOverlordRelations(overlord);
	
	public static GroupPartRelation GetPartRelation(this Faction part) =>
		GetRFRComponent().GetPartRelation(part);

	public static IEnumerable<GroupPartRelation> GetGroupRelations(this Faction group) =>
		GetRFRComponent().GetGroupRelations(group);
	public static ParentChildRelation GetChildRelation(this Faction child) =>
		GetRFRComponent().GetChildRelation(child);

	public static IEnumerable<ParentChildRelation> GetParentRelations(this Faction parent) =>
		GetRFRComponent().GetParentRelations(parent);

	public static Faction GetOverlord(this Faction vassal) => 
		GetRFRComponent().GetOverlord(vassal);

	public static Faction[] GetVassals(this Faction overlord) => 
		GetRFRComponent().GetVassals(overlord).ToArray();

	public static Faction GetParent(this Faction child) => 
		GetRFRComponent().GetParent(child);

	public static Faction GetGroup(this Faction part) => 
		GetRFRComponent().GetGroup(part);

	public static Faction[] GetChildren(this Faction parent) => 
		GetRFRComponent().GetChildren(parent).ToArray();

	public static Faction[] GetParts(this Faction group) => 
		GetRFRComponent().GetParts(group).ToArray();


	public static bool TryGetSuperFaction(Faction itself, out Faction superFaction)
	{
		superFaction = null;
		foreach (var condition in getSuperFactionConditions)
		{
			if (!condition(itself, out var tempSuperFaction)) continue;
			superFaction = tempSuperFaction;
			return true;
		}
		return false;
		//return !getSuperFactionConditions.NullOrEmpty() && getSuperFactionConditions.Select(getSuperFactionCondition => getSuperFactionCondition(itself,out superFaction)).Any(result => result);
	}

	public static List<GetSuperFactionCondition> getSuperFactionConditions = [TryGetOverlord, TryGetGroup, TryGetParent];

	public static bool TryGetSubFactions(Faction itself, out Faction[] subFactions)
	{
		subFactions = null;
		foreach (var condition in getSubFactionConditions)
		{
			if (!condition(itself, out var tempSubFaction)) continue;
			subFactions = tempSubFaction;
		}
		return true;
	}

	public static GetSubFactionCondition[] getSubFactionConditions = [TryGetVassal, TryGetPart, TryGetChildren];

	public static bool TryGetOverlord(Faction vassal, out Faction overlord)
	{
		overlord = null;
		if (!IsVassal(vassal)) return false;
		overlord = GetOverlord(vassal);
		return true;
	}

	public static bool TryGetParent(Faction child, out Faction parent)
	{
		parent = null;
		if (!IsChild(child)) return false;
		parent = GetParent(child);
		return true;
	}

	public static bool TryGetGroup(Faction part, out Faction group)
	{
		group = null;
		if (!IsPart(part)) return false;
		group = GetGroup(part);
		return true;
	}

	public static bool TryGetVassal(Faction overlord, out Faction[] vassals)
	{
		vassals = null;
		if (!IsOverlord(overlord)) return false;
		vassals = GetVassals(overlord);
		return true;
	}

	public static bool TryGetChildren(Faction parent, out Faction[] children)
	{
		children = null;
		if (!IsParent(parent)) return false;
		children = GetChildren(parent);
		return true;
	}

	public static bool TryGetPart(Faction group, out Faction[] parts)
	{
		parts = null;
		if (!IsGroup(group)) return false;
		parts = GetParts(group);
		return true;
	}

	//===============================================================
	public static OverlordVassalRelation.VassalRelationKind InitVassalKind(int num) =>
		num > 75 ? OverlordVassalRelation.VassalRelationKind.Loyal :
		num > -75 ? OverlordVassalRelation.VassalRelationKind.Reserve : OverlordVassalRelation.VassalRelationKind.Rebellious;

	public static OverlordVassalRelation.OverlordRelationKind InitOverlordKind(int num) =>
		num > 75 ? OverlordVassalRelation.OverlordRelationKind.Protective :
		num > -75 ? OverlordVassalRelation.OverlordRelationKind.Tolerance : OverlordVassalRelation.OverlordRelationKind.Domineering;

	public static RelationWorldComponent GetRFRComponent() =>
		Current.Game == null || Current.Game.World == null
			? Current.CreatingWorld.GetComponent<RelationWorldComponent>()
			: Current.Game.World.GetComponent<RelationWorldComponent>();

	public static void InitAlly(List<Faction> factions)
	{
		foreach(var f1 in factions)
			foreach (var f2 in factions.Where(f2 => f1 != f2))
			{
				f1.TryAffectGoodwillWith(f2, 100 - f1.GoodwillWith(f2));
				f2.TryAffectGoodwillWith(f1, 100 - f2.GoodwillWith(f1));
				f1.RelationWith(f2).kind = FactionRelationKind.Ally;
				f2.RelationWith(f1).kind = FactionRelationKind.Ally;
			}
		
	}
	public static void EndAlly(List<Faction> factions)
	{
		foreach (var f1 in factions)
			foreach (var f2 in factions.Where(f2 => f1 != f2))
			{
				if(f1.GoodwillWith(f2)>0) f1.TryAffectGoodwillWith(f2,0-f1.GoodwillWith(f2));
				if(f2.GoodwillWith(f1)>0) f2.TryAffectGoodwillWith(f1,0-f2.GoodwillWith(f1));
				f1.RelationWith(f2).kind = FactionRelationKind.Neutral;
				f2.RelationWith(f1).kind = FactionRelationKind.Neutral;
			}
	}
	public static bool CreateRelation(Dictionary<RelationItemDef,Faction> itemDict,RelationTypeDef def)
	{
		if (itemDict.NullOrEmpty() || def == null) return false;
		Log.Message($"itemDict:{itemDict},{itemDict.Count},def:{def}");
		if (itemDict.Any(itemPair => !itemPair.Value.CheckAllRelationItemCondition(itemPair.Key))) 
			return false;
		var relation = RelationMaker.MakeRelation(def);
		if (relation.relationType.createAlly) InitAlly(itemDict.Values.ToList());
		relation.relationItems = itemDict;
		GetRFRComponent().SetRelation(relation);
		return true;
	}
	public static bool CreateOverlordVassalRelation(Faction overlord, Faction vassal)
	{
		if (overlord == null || vassal == null) return false;
		return CreateRelation(new Dictionary<RelationItemDef, Faction>
		{ { BWCRDefOf.BWCR_Overlord, overlord }, { BWCRDefOf.BWCR_Vassal, vassal } }
			, BWCRDefOf.BWCR_Overlord_Vassal);
	}

	public static bool CreateParentChildRelation(Faction parent, Faction child)
	{
		if (parent == null || child == null) return false;
		child.ChangeColor(parent);
		return CreateRelation(new Dictionary<RelationItemDef, Faction>
		{ { BWCRDefOf.BWCR_Parent, parent }, { BWCRDefOf.BWCR_Child, child } }
			, BWCRDefOf.BWCR_Parent_Child);
	}

	public static bool CreateGroupPartRelation(Faction group, Faction part)
	{
		if (group == null || part == null) return false;
		Log.Message($"{BWCRDefOf.BWCR_Group},{group},{BWCRDefOf.BWCR_Part},{part}");
		return CreateRelation(new Dictionary<RelationItemDef, Faction>
		{ { BWCRDefOf.BWCR_Group, group }, { BWCRDefOf.BWCR_Part, part } }
			, BWCRDefOf.BWCR_Group_Part);
	}

	public static bool DeleteRelation(Dictionary<RelationItemDef,Faction> itemDict)
	{
		if (itemDict.NullOrEmpty()) return false;
		GetRFRComponent().DesRelation(itemDict,out var relation);
		if (relation != null && relation.relationType.createAlly) InitAlly(itemDict.Values.ToList());
		return true;
	}
	public static bool DeleteOverlordVassalRelation(Faction overlord, Faction vassal)
	{
		if (overlord == null || vassal == null) return false;
		//vassal.ChangeColorBack();
		return DeleteRelation(new Dictionary<RelationItemDef, Faction>{{BWCRDefOf.BWCR_Overlord,overlord}, {BWCRDefOf.BWCR_Vassal,vassal}});
	}

	public static bool DeleteParentChildRelation(Faction parent, Faction child)
	{
		if (parent == null || child == null) return false;
		child.ChangeColorBack();
		return DeleteRelation(new Dictionary<RelationItemDef, Faction>{{BWCRDefOf.BWCR_Parent,parent}, {BWCRDefOf.BWCR_Child,child}});
	}

	public static bool DeleteGroupPartRelation(Faction group, Faction part)
	{
		if (group == null || part == null) return false;
		return DeleteRelation(new Dictionary<RelationItemDef, Faction>{{BWCRDefOf.BWCR_Group,group}, {BWCRDefOf.BWCR_Part,part}});
	}
	

	public static Predicate<Faction> IS_NOT_CHILD = faction => !IsChild(faction);
	public static Predicate<Faction> IS_NOT_PARENT = faction => !IsParent(faction);
	public static Predicate<Faction> IS_NOT_VASSAL = faction => !IsVassal(faction);
	public static Predicate<Faction> IS_NOT_OVERLORD = faction => !IsOverlord(faction);
	public static Predicate<Faction> IS_NOT_PART = faction => !IsPart(faction);
	public static Predicate<Faction> IS_NOT_GROUP = faction => !IsGroup(faction);
	public static Predicate<Faction> NEVER = faction => false;
	

	//===================================================================
	public static bool RebelliousTo(this Faction fac, Faction other) =>
		fac != null && other != null && other != fac &&
		OverlordRelationWith(fac, other).kind[fac] == (byte)OverlordVassalRelation.VassalRelationKind.Rebellious;
	public static bool LoyalTo(this Faction fac, Faction other) =>
		fac != null && other != null && other != fac &&
		VassalRelationWith(fac, other).kind[fac] == (byte)OverlordVassalRelation.VassalRelationKind.Loyal;
	public static bool DomineeringTo(this Faction fac, Faction other) =>
		fac != null && other != null && other != fac &&
		OverlordRelationWith(fac, other).kind[fac] == (byte)OverlordVassalRelation.OverlordRelationKind.Domineering;

	public static bool ProtectiveTo(this Faction fac, Faction other) =>
		fac != null && other != null && other != fac &&
		OverlordRelationWith(fac, other).kind[fac] == (byte)OverlordVassalRelation.OverlordRelationKind.Protective;			
	
	
	//=============================================================
	//					ORIGINAL COLOR ZONE
	//=============================================================
	public static bool SetOriginalColor(this Faction f, Color c) => GetRFRComponent().SetOriginalColor(f, c);
	public static bool SetOriginalColor(this Faction f) => GetRFRComponent().SetOriginalColor(f, f.Color);

	public static bool TryGetOriginalColor(this Faction f, out Color c) => GetRFRComponent().TryGetOriginalColor(f, out c);
	public static bool RemoveOriginalColor(this Faction f) => GetRFRComponent().DesOriginalColor(f);

	public static bool ChangeColor(this Faction subFaction, Faction superFaction)
	{
		if (subFaction == null || superFaction == null) return false;
		if (!subFaction.SetOriginalColor()) return false;
		subFaction.color = superFaction.Color;
		return true;
	}
	public static bool ChangeColor(this Faction subFaction, Color color)
	{
		if (subFaction == null) return false;
		if (!subFaction.SetOriginalColor()) return false;
		subFaction.color = color;
		return true;
	}
	public static bool ChangeColorBack(this Faction subFaction)
	{
		if (subFaction == null) return false;
		if (!subFaction.TryGetOriginalColor(out var color)) return false;
		subFaction.color = color;
		if(!subFaction.RemoveOriginalColor()) Log.Error($"cannot remove original color of faction {subFaction}");
		return true;
	}
}
public delegate bool GetSuperFactionCondition(Faction faction,out Faction faction2);
public delegate bool GetSubFactionCondition(Faction faction,out Faction[] faction2);
public delegate bool GetSuperFactionFlavorFactor(Faction subFaction,out float factor);