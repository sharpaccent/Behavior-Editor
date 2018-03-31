using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.BehaviorEditor
{
    [CreateAssetMenu(menuName = "Editor/Comment Node")]
    public class CommentNode :DrawNode
    {
        
        public override void DrawWindow(BaseNode b)
        {
            b.comment = GUILayout.TextArea(b.comment, 200);
        }

        public override void DrawCurve(BaseNode b)
        {
        }
    }
}
