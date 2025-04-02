using UnityEngine;
using Verse;

namespace BWCore;

[StaticConstructorOnStartup]
internal class BWCore : Mod
{
    public BWCore(ModContentPack mcp):base(mcp) => GetSettings<BWCSetting>();
    public override void WriteSettings()
    {
        base.WriteSettings();
    }
    public override void DoSettingsWindowContents(Rect inRect) => BWCSetting.DoWindowContents(inRect);
    public override string SettingsCategory() => "[BW]BWCore";
}