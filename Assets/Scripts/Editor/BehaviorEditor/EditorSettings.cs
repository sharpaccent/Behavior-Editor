using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.BehaviorEditor
{
    [CreateAssetMenu(menuName ="Editor/Settings")]
    public class EditorSettings : ScriptableObject
    {
        public BehaviorGraph currentGraph;
        public StateNode stateNode;
		public PortalNode portalNode;
        public TransitionNode transitionNode;
        public CommentNode commentNode;
        public bool makeTransition;
        public GUISkin skin;
		public GUISkin activeSkin;
        
        public BaseNode AddNodeOnGraph(DrawNode type, float width,float height, string title, Vector3 pos)
        {
            BaseNode baseNode = new BaseNode();
            baseNode.drawNode = type;
            baseNode.windowRect.width = width;
            baseNode.windowRect.height = height;
            baseNode.windowTitle = title;
            baseNode.windowRect.x = pos.x;
            baseNode.windowRect.y = pos.y;
            currentGraph.windows.Add(baseNode);
            baseNode.transRef = new TransitionNodeReferences();
            baseNode.stateRef = new StateNodeReferences();
            baseNode.id = currentGraph.idCount;
            currentGraph.idCount++;
            return baseNode;
        }
    }
}
