using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BWCR;

public class WorldGenStep_GroupPart :WorldGenStep
{
    public override int SeedPart => GetType().Name.GetHashCode();

    public override void GenerateFresh(string seed)
    {
        var partList = Find.FactionManager.AllFactions.ToList().FindAll(f =>
            f.def.HasModExtension<BWCRelationExtension>() &&
            f.def.GetModExtension<BWCRelationExtension>()?.defaultGroup != null);
        Log.Message($"Generate {partList.Count} group parts");
        if (partList.NullOrEmpty()) return;
        GenerateCities(partList);
    }

    public override void GenerateFromScribe(string seed) => GenerateFresh(seed);

    private void GenerateCities(List<Faction> partList)
    {
        Log.Message($"{partList}");
        //Log.Message($"equal1: {partList[0] == partList[1]}   equal2: {partList[0].Equals(partList[1])}");
        foreach (var part in partList)
        {
            var group = Find.FactionManager.AllFactions.FirstOrFallback(f => f.def == part.def.GetModExtension<BWCRelationExtension>().defaultGroup,fallback:null);
            if (group == null)
            {
                Log.Warning($"Can't find group for {part.def.defName}");
                continue;
            }
            Log.Message("Generate group part for " + part.def.defName);
            if (RelationFactionUtil.CreateGroupPartRelation(group, part)) Log.Message("finish");
            else Log.Message("error");
        }
    }
}