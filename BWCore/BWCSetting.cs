using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BWCore;

public class BWCSetting :ModSettings
{
	
	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref overlordFlavorFactor, "overlordFlavorFactor", 2f, true);
		Scribe_Values.Look(ref groupFlavorFactor, "groupFlavorFactor", 0f, true);
		Scribe_Values.Look(ref parentFlavorFactor, "groupFlavorFactor", 5f, true);
		Scribe_Values.Look(ref enemyStackArmDisable, "EnemyStackArmDisable", false, true);
		Scribe_Values.Look(ref maxExpansionLimit, "maxExpansionLimit", 144, true);
		Scribe_Values.Look(ref enableExpansion, "enableExpansion", true, true);
		
		Scribe_Values.Look(ref townPer100kTiles, "townPer100kTiles", new IntRange(10,15));
		Scribe_Values.Look(ref abandonedPer100kTiles, "abandonedPer100kTiles", new IntRange(5,10));
		Scribe_Values.Look(ref compromisedPer100kTiles, "compromisedPer100kTiles", new IntRange(3,6));
	}
	public static void DoWindowContents(Rect inRect)
	{
		var viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + 500f);
		var listing_Standard = new Listing_Standard();
		Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);
		listing_Standard.Begin(viewRect);
		Text.Font = GameFont.Small;
		listing_Standard.Gap();
		listing_Standard.Label($"{"FlavorFactor".Translate()}", -1f, "FlavorFactor".Translate());
		listing_Standard.Gap(4f);
		overlordFlavorFactor = (int)listing_Standard.Slider(overlordFlavorFactor, 0f, 100f);
		listing_Standard.Gap(4f);
		groupFlavorFactor  = (int)listing_Standard.Slider(groupFlavorFactor, 0f, 100f);
		listing_Standard.Gap(4f);
		parentFlavorFactor = (int)listing_Standard.Slider(parentFlavorFactor, 0f, 100f);
		listing_Standard.Gap(8f);
		listing_Standard.Label($"{"EnemyStackArmDisable".Translate()}");
		listing_Standard.Gap(4f);
		listing_Standard.CheckboxLabeled("EnemyStackArmDisable".Translate(), ref enemyStackArmDisable, "EnemyStackArmDisable".Translate());
		listing_Standard.Gap(8f);
		listing_Standard.Label($"{"Expansion".Translate()}", -1f, "Expansion".Translate());
		listing_Standard.Gap(4f);
		maxExpansionLimit = (int)listing_Standard.Slider(maxExpansionLimit, 0, 1000);
		listing_Standard.Gap(4f);
		listing_Standard.CheckboxLabeled("EnableExpansion".Translate(), ref enableExpansion, "EnableExpansion".Translate());
		listing_Standard.Gap(4f);
		expansionRadius = (int)listing_Standard.Slider(expansionRadius, 0, 100);
		listing_Standard.Gap(8f);
		listing_Standard.Label("CitiesPer100kTiles".Translate());
		listing_Standard.IntRange(ref townPer100kTiles, 0, 100);
		listing_Standard.Label("AbandonedPer100kTiles".Translate());
		listing_Standard.IntRange(ref abandonedPer100kTiles, 0, 100);
		listing_Standard.Label("CompromisedPer100kTiles".Translate());
		listing_Standard.IntRange(ref compromisedPer100kTiles, 0, 100);
		listing_Standard.End();
		Widgets.EndScrollView();
	}

	private static Vector2 scrollPosition;
	//public 
	public static bool enemyStackArmDisable;
	public static float overlordFlavorFactor = 2f;
	public static float groupFlavorFactor;
	public static float parentFlavorFactor = 5f;
	public static int maxExpansionLimit = 144;
	public static bool enableExpansion = true;
	public static int expansionRadius = 24;
	public static IntRange townPer100kTiles = new(10, 15);
	public static IntRange abandonedPer100kTiles = new(5, 10);
	public static IntRange compromisedPer100kTiles = new(3, 6); 
	public static bool enableEvents = true;

}