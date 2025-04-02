using UnityEngine;
using Verse;

namespace BWCore;

[StaticConstructorOnStartup]
internal class BWCore : Mod
{
    public BWCore(ModContentPack mcp):base(mcp)
    {
        bwcSetting=GetSettings<BWCSetting>();
    }

    public BWCSetting bwcSetting;
    public override void WriteSettings()
    {
        base.WriteSettings();
    }
    public override void DoSettingsWindowContents(Rect inRect) => bwcSetting.DoWindowContents(inRect);
    public override string SettingsCategory() => "[BW]BWCore";
}