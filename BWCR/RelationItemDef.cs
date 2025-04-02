using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BWCR;

public class RelationItemDef : Def
{
    public RelationTabDef tab;
    public List<RelationItemDef> conflictItems;
    public List<RelationItemDef> needItems;
    
    
    [NoTranslate]
    private string expandingIconTexture;
    [Unsaved]
    private Texture2D expandingIconTextureInt;
    public Texture2D ExpandingIconTexture
    {
        get
        {
            if (expandingIconTextureInt != null) return expandingIconTextureInt;
            if (expandingIconTexture.NullOrEmpty()) return null;
            expandingIconTextureInt = ContentFinder<Texture2D>.Get(expandingIconTexture);
            return expandingIconTextureInt;
        }
    }
}