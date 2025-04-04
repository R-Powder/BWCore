using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BWCR;
[StaticConstructorOnStartup]
public class RelationWorldComponent(World world) : WorldComponent(world)
{
	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Collections.Look(ref relations, "relations",LookMode.Def,LookMode.Deep);
		Scribe_Collections.Look(ref originalColors, "originalColors",LookMode.Reference,LookMode.Deep);
		Scribe_Collections.Look(ref lockAllys, "lockAllys",LookMode.Reference);
	}
	public override void FinalizeInit()
	{
		base.FinalizeInit();
	}

	public override void WorldComponentTick()
	{
		base.WorldComponentTick();
		if (Find.TickManager.TicksGame % 2000 == 0)
		{
			if (Find.TickManager.TicksGame % 20000 == 0)
			{
				if (Find.TickManager.TicksGame % 60000 == 0 && !LockAllys.NullOrEmpty())
					foreach (var relation in LockAllys)
						foreach (var fi in relation.relationItems.Values)
							foreach (var fo in relation.relationItems.Values.Where(fo => fo!=fi&&fi.RelationKindWith(fo)!=FactionRelationKind.Ally))
								fi.RelationWith(fo).kind = FactionRelationKind.Ally;
			}
		}
		
	}
	
	//=========================================================
	private Dictionary<RelationItemDef,Dictionary<Faction,List<Relation>>> relations;
	public Dictionary<RelationItemDef,Dictionary<Faction,List<Relation>>> Relations => relations ??= new();
	private List<Relation> lockAllys;
	public List<Relation> LockAllys => lockAllys ??= [];
	//=============================================================

	//============================================================
	[CanBeNull]
	public Relation GetRelationUnique(Faction faction,RelationItemDef item) =>
		faction == null ? null : Relations.TryGetValue(item, out var dictRelation) 
			? dictRelation.TryGetValue(faction,out var relation)
				? relation[0] : null : null;
	
	public IEnumerable<Relation> GetRelation(Faction faction,RelationItemDef item) =>
		faction == null ? Array.Empty<Relation>() : Relations.TryGetValue(item, out var dictRelation) 
			? dictRelation.TryGetValue(faction,out var getRelation)
				? getRelation : Array.Empty<Relation>() : Array.Empty<Relation>();
	public OverlordVassalRelation GetVassalRelation(Faction vassal) =>
		GetRelationUnique(vassal,BWCRDefOf.BWCR_Vassal) as OverlordVassalRelation;

	public IEnumerable<OverlordVassalRelation> GetOverlordRelations(Faction overlord) =>
		GetRelation(overlord, BWCRDefOf.BWCR_Overlord).OfType<OverlordVassalRelation>();
	public IEnumerable<GroupPartRelation> GetGroupRelations(Faction group) =>
		GetRelation(group, BWCRDefOf.BWCR_Group).OfType<GroupPartRelation>();

	public GroupPartRelation GetPartRelation(Faction part) =>
		GetRelationUnique(part, BWCRDefOf.BWCR_Part) as GroupPartRelation;
	public IEnumerable<ParentChildRelation> GetParentRelations(Faction parent) =>
		GetRelation(parent, BWCRDefOf.BWCR_Parent).OfType<ParentChildRelation>();

	public ParentChildRelation GetChildRelation(Faction child) =>
		GetRelationUnique(child, BWCRDefOf.BWCR_Child) as ParentChildRelation;
	//============================================================
	public bool IsItem(Faction faction, RelationItemDef item) => !GetRelation(faction,item).EnumerableNullOrEmpty();
	
	/// <summary> Returns the faction that is the group of the given faction </summary>
	public Faction GetGroup(Faction part) => GetPartRelation(part)?.Group;
	public Faction GetParent(Faction child) => GetChildRelation(child)?.Parent;
	public Faction GetOverlord(Faction vassal) => GetVassalRelation(vassal)?.Overlord;

	public IEnumerable<Faction> GetParts(Faction group) => 
		GetGroupRelations(group).Select(groupPartRelation => groupPartRelation.Part);

	public IEnumerable<Faction> GetChildren(Faction parent) => 
		GetParentRelations(parent).Select(parentChildRelation => parentChildRelation.Child); 
	public IEnumerable<Faction> GetVassals(Faction overlord) =>
		GetOverlordRelations(overlord).Select(overlordVassalRelation => overlordVassalRelation.Vassal);
	//=============================================================
	public void SetRelation(Relation relation)
	{
		if (relation == null)return;
		foreach (var relationItem in relation.relationType.relationItems)
		{
			var item = relation.relationItems[relationItem];
			if (!Relations.ContainsKey(relationItem)) Relations[relationItem] = new() { { item, [relation] } };
			else if (!Relations[relationItem].ContainsKey(item)) Relations[relationItem].Add(item, [relation]);
			else Relations[relationItem][item].Add(relation);
		}
		if (relation.relationType.createAlly) LockAllys.Add(relation);
	}

	public void DesRelation(Dictionary<RelationItemDef, Faction> itemDict)
	{
		if (itemDict.NullOrEmpty()) return;
		Relation targetRelation = null;
		foreach (var relation in itemDict.Where(relation => Relations.ContainsKey(relation.Key) && Relations[relation.Key].ContainsKey(relation.Value)))
			Relations[relation.Key][relation.Value].Remove(targetRelation??= Relations[relation.Key][relation.Value].First(x => x.relationItems==itemDict));
		if (targetRelation != null && targetRelation.relationType.createAlly) LockAllys.Remove(targetRelation);
	}
	public void DesRelation(Dictionary<RelationItemDef, Faction> itemDict,[CanBeNull]out Relation _relation)
	{
		_relation = null;
		if (itemDict.NullOrEmpty()) return;
		Relation targetRelation = null;
		foreach (var relation in itemDict.Where(relation => Relations.ContainsKey(relation.Key) && Relations[relation.Key].ContainsKey(relation.Value)))
			Relations[relation.Key][relation.Value].Remove(targetRelation??= Relations[relation.Key][relation.Value].First(x => x.relationItems==itemDict));
		if (targetRelation != null && targetRelation.relationType.createAlly) LockAllys.Remove(targetRelation);
		_relation = targetRelation;
	}

	public bool DesOverlordVassalRelation(Faction overlord,Faction vassal){
		if(overlord == null || vassal == null) return false;
		DesRelation(new Dictionary<RelationItemDef, Faction>{{BWCRDefOf.BWCR_Overlord, overlord}, {BWCRDefOf.BWCR_Vassal, vassal}});			
		return true;
	}
	public bool DesGroupPartRelation(Faction group,Faction part){
		if(group == null || part == null) return false;
		DesRelation(new Dictionary<RelationItemDef, Faction>{{BWCRDefOf.BWCR_Group, group}, {BWCRDefOf.BWCR_Part, part}});			
		return true;
	}
	public bool DesParentChildRelation(Faction parent,Faction child){
		if(parent == null || child == null) return false;
		DesRelation(new Dictionary<RelationItemDef, Faction>{{BWCRDefOf.BWCR_Parent, parent}, {BWCRDefOf.BWCR_Child, child}});
		return true;
	}
		
		
		
	//=============================================================
	public Dictionary<Faction,Color> originalColors ;
	public Dictionary<Faction,Color> OriginalColors => originalColors ??= new();
	

	public bool TryGetOriginalColor(Faction f,out Color c)
	{
		if (OriginalColors.TryGetValue(f, out var color))
		{
			c = color;
			return true;
		}
		Log.Error($"faction {f} do not has original color");
		c = Color.grey;
		return false;
	}
	public bool SetOriginalColor(Faction f, Color c)
	{
		if ( f == null) return false;
		OriginalColors[f] = c;
		return true;
	}
	public bool DesOriginalColor(Faction f) => OriginalColors.ContainsKey(f) && OriginalColors.Remove(f);
}