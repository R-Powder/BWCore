using UnityEngine;

namespace BWCore;

public class BWModuleSetting
{
    public virtual void ExposeData()
    { }
    public virtual void DoWindowContents(ref Rect inRect)
    { }
}