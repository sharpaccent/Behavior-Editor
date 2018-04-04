using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.BehaviorEditor
{
    public abstract class DrawNode : ScriptableObject
    {
        public abstract void DrawWindow(BaseNode b);
        public abstract void DrawCurve(BaseNode b);
    }
}
