using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BWCR;

[StaticConstructorOnStartup]
public static class HarmonyPatch
{
	static HarmonyPatch()
	{
		var harmonyInstance = new Harmony("rimworld.rpowder.BWRFR.mod");
		harmonyInstance.Patch(AccessTools.Method(typeof(FactionUIUtility), "DoWindowContents"), null, new HarmonyMethod(typeof(SenatorUIUtility), "DoRelationButton"));
	}
}

[StaticConstructorOnStartup]
public static class SenatorUIUtility
{
	public static void DoRelationButton()
	{
		var flag = RelationFactionUtil.RelationTabs.NullOrEmpty();
		var flag2 = !flag && Widgets.ButtonText(new Rect(130f, 10f, 120f, 30f), "BWRFR.Relation".Translate());
		if (flag2) Find.WindowStack.Add(new Dialog_RelationInfo());
	}
}