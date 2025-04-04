using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BWCore;

public class BWCSetting :ModSettings
{
	List<BWModuleSetting> moduleSettings;
	/// <summary>
	/// 待测试
	/// </summary>
	List<BWModuleSetting> ModuleSettings => moduleSettings??=typeof(BWModuleSetting).AllSubclasses().Select(t => (BWModuleSetting)Activator.CreateInstance(t)).ToList();
	public override void ExposeData()
	{
		base.ExposeData();
		foreach (var setting in ModuleSettings) setting.ExposeData();
	}
	public void DoWindowContents(Rect inRect)
	{
		var viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + 500f);
		var listing_Standard = new Listing_Standard();
		Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);
		listing_Standard.Begin(viewRect);
		var tempRect = viewRect;
		Text.Font = GameFont.Small;
		listing_Standard.Gap();
		foreach (var setting in ModuleSettings) setting.DoWindowContents(ref tempRect);
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