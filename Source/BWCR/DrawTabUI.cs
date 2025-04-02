using System.Collections.Generic;
using UnityEngine;

namespace BWCR;

public delegate void DrawTabUI(Rect rect,ref Vector2 scrollPosition,ref float scrollViewWidth, List<Relation> relations,object other,bool showAll);