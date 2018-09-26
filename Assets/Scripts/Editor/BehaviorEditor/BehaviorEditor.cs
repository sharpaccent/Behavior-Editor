using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.BehaviorEditor
{
    public class BehaviorEditor : EditorWindow
    {

        #region Variables
        Vector3 mousePosition;
        bool clickedOnWindow;
        BaseNode selectedNode;

        public static EditorSettings settings;
        int transitFromId;
        Rect mouseRect = new Rect(0, 0, 1, 1);
        Rect all = new Rect(-5, -5, 10000, 10000);
        GUIStyle style;
		GUIStyle activeStyle;
		Vector2 scrollPos;
		Vector2 scrollStartPos;
		static BehaviorEditor editor;
		public static StateManager currentStateManager;
		public static bool forceSetDirty;
		static StateManager prevStateManager;
		static State previousState;
		int nodesToDelete;


		public enum UserActions
        {
            addState,addTransitionNode,deleteNode,commentNode,makeTransition,makePortal,resetPan
        }
        #endregion

        #region Init
        [MenuItem("Behavior Editor/Editor")]
        static void ShowEditor()
        {
            editor = EditorWindow.GetWindow<BehaviorEditor>();
            editor.minSize = new Vector2(800, 600);
        
        }

        private void OnEnable()
        {
            settings = Resources.Load("EditorSettings") as EditorSettings;
            style = settings.skin.GetStyle("window");
			activeStyle = settings.activeSkin.GetStyle("window");

		}
		#endregion

		private void Update()
		{
			if (currentStateManager != null)
			{
				if (previousState != currentStateManager.currentState)
				{
					Repaint();
					previousState = currentStateManager.currentState;
				}
			}

			if (nodesToDelete > 0)
			{
				if (settings.currentGraph != null)
				{		
					settings.currentGraph.DeleteWindowsThatNeedTo();
					Repaint();
				}
				nodesToDelete = 0;
			}
		}

		#region GUI Methods
		private void OnGUI()
        {
			if (Selection.activeTransform != null)
			{
				currentStateManager = Selection.activeTransform.GetComponentInChildren<StateManager>();
				if (prevStateManager != currentStateManager)
				{
					prevStateManager = currentStateManager;
					Repaint();
				}
			}

		

			Event e = Event.current;
            mousePosition = e.mousePosition;
            UserInput(e);

            DrawWindows();

			if (e.type == EventType.MouseDrag)
			{
				if (settings.currentGraph != null)
				{
					//settings.currentGraph.DeleteWindowsThatNeedTo();
					Repaint();
				}
			}

			if (GUI.changed)
			{
				settings.currentGraph.DeleteWindowsThatNeedTo();
				Repaint();
			}

            if(settings.makeTransition)
            {
                mouseRect.x = mousePosition.x;
                mouseRect.y = mousePosition.y;
                Rect from = settings.currentGraph.GetNodeWithIndex(transitFromId).windowRect;
                DrawNodeCurve(from, mouseRect, true, Color.blue);
                Repaint();
            }

			if (forceSetDirty)
			{
				forceSetDirty = false;
				EditorUtility.SetDirty(settings);
				EditorUtility.SetDirty(settings.currentGraph);

				for (int i = 0; i < settings.currentGraph.windows.Count; i++)
				{
					BaseNode n = settings.currentGraph.windows[i];
					if(n.stateRef.currentState != null)
						EditorUtility.SetDirty(n.stateRef.currentState);
			
				}

			}
			
		}

		void DrawWindows()
        {
			GUILayout.BeginArea(all, style);
		
			BeginWindows();
            EditorGUILayout.LabelField(" ", GUILayout.Width(100));
            EditorGUILayout.LabelField("Assign Graph:", GUILayout.Width(100));
            settings.currentGraph = (BehaviorGraph)EditorGUILayout.ObjectField(settings.currentGraph, typeof(BehaviorGraph), false, GUILayout.Width(200));

			if (settings.currentGraph != null)
            {
                foreach (BaseNode n in settings.currentGraph.windows)
                {
                    n.DrawCurve();
                }

                for (int i = 0; i < settings.currentGraph.windows.Count; i++)
                {
					BaseNode b = settings.currentGraph.windows[i];

					if (b.drawNode is StateNode)
					{
						if (currentStateManager != null && b.stateRef.currentState == currentStateManager.currentState)
						{
							b.windowRect = GUI.Window(i, b.windowRect,
								DrawNodeWindow, b.windowTitle,activeStyle);
						}
						else
						{
							b.windowRect = GUI.Window(i, b.windowRect,
								DrawNodeWindow, b.windowTitle);
						}
					}
					else
					{
						b.windowRect = GUI.Window(i, b.windowRect,
							DrawNodeWindow, b.windowTitle);
					}
                }
            }
			EndWindows();

			GUILayout.EndArea();
			

		}

		void DrawNodeWindow(int id)
        {
            settings.currentGraph.windows[id].DrawWindow();
            GUI.DragWindow();
        }

        void UserInput(Event e)
        {
            if (settings.currentGraph == null)
                return;

            if(e.button == 1 && !settings.makeTransition)
            {
                if(e.type == EventType.MouseDown)
                {
                    RightClick(e);
					
                }
            }

            if (e.button == 0 && !settings.makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {

                }
            }

            if(e.button == 0 && settings.makeTransition)
            {
                if(e.type == EventType.MouseDown)
                {
                    MakeTransition();
                }
            }

			if (e.button == 2)
			{
				if (e.type == EventType.MouseDown)
				{
					scrollStartPos = e.mousePosition;
				}
				else if (e.type == EventType.MouseDrag)
				{
					HandlePanning(e);
				}
				else if (e.type == EventType.MouseUp)
				{

				}
			}
        }

		void HandlePanning(Event e)
		{
			Vector2 diff = e.mousePosition - scrollStartPos;
			diff *= .6f;
			scrollStartPos = e.mousePosition;
			scrollPos += diff;

			for (int i = 0; i < settings.currentGraph.windows.Count; i++)
			{
				BaseNode b = settings.currentGraph.windows[i];
				b.windowRect.x += diff.x;
				b.windowRect.y += diff.y;
			}
		}

		void ResetScroll()
		{
			for (int i = 0; i < settings.currentGraph.windows.Count; i++)
			{
				BaseNode b = settings.currentGraph.windows[i];
				b.windowRect.x -= scrollPos.x;
				b.windowRect.y -= scrollPos.y;
			}

			scrollPos = Vector2.zero;
		}

        void RightClick(Event e)
        {
            clickedOnWindow = false;
            for (int i = 0; i < settings.currentGraph.windows.Count; i++)
            {
                if (settings.currentGraph.windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = settings.currentGraph.windows[i];
                    break;
                }
            }

            if(!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e);
            }
        }
       
        void MakeTransition()
        {
            settings.makeTransition = false;
            clickedOnWindow = false;
            for (int i = 0; i < settings.currentGraph.windows.Count; i++)
            {
                if (settings.currentGraph.windows[i].windowRect.Contains(mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = settings.currentGraph.windows[i];
                    break;
                }
            }

            if(clickedOnWindow)
            {
                if(selectedNode.drawNode is StateNode || selectedNode.drawNode is PortalNode)
                {
                    if(selectedNode.id != transitFromId)
                    {
                        BaseNode transNode = settings.currentGraph.GetNodeWithIndex(transitFromId);
                        transNode.targetNode = selectedNode.id;

                        BaseNode enterNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(transNode.enterNode);
                        Transition transition = enterNode.stateRef.currentState.GetTransition(transNode.transRef.transitionId);

						transition.targetState = selectedNode.stateRef.currentState;
                    }
                }
            }
        }
        #endregion

        #region Context Menus
        void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            if (settings.currentGraph != null)
            {
                menu.AddItem(new GUIContent("Add State"), false, ContextCallback, UserActions.addState);
				menu.AddItem(new GUIContent("Add Portal"), false, ContextCallback, UserActions.makePortal);
				menu.AddItem(new GUIContent("Add Comment"), false, ContextCallback, UserActions.commentNode);
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, UserActions.resetPan);
			}
            else
            {
                menu.AddDisabledItem(new GUIContent("Add State"));
                menu.AddDisabledItem(new GUIContent("Add Comment"));
            }
            menu.ShowAsContext();
            e.Use();
        }

        void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            if (selectedNode.drawNode is StateNode)
            {
                if (selectedNode.stateRef.currentState != null)
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Add Condition"), false, ContextCallback, UserActions.addTransitionNode);
                }
                else
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("Add Condition"));
                }
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
            }

			if (selectedNode.drawNode is PortalNode)
			{
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
			}

			if (selectedNode.drawNode is TransitionNode)
            {
                if (selectedNode.isDuplicate || !selectedNode.isAssigned)
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("Make Transition"));
                }
                else
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, UserActions.makeTransition);
                }
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
            }

            if (selectedNode.drawNode is CommentNode)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
            }
            menu.ShowAsContext();
            e.Use();
        }
        
        void ContextCallback(object o)
        {
            UserActions a = (UserActions)o;
            switch (a)
            {
                case UserActions.addState:
                    settings.AddNodeOnGraph(settings.stateNode, 200, 100, "State", mousePosition);                
                    break;
				case UserActions.makePortal:
					settings.AddNodeOnGraph(settings.portalNode, 100, 80, "Portal", mousePosition);
					break;
                case UserActions.addTransitionNode:
					AddTransitionNode(selectedNode, mousePosition);

					break;           
                case UserActions.commentNode:
                    BaseNode commentNode = settings.AddNodeOnGraph(settings.commentNode, 200, 100, "Comment", mousePosition);
                    commentNode.comment = "This is a comment";           
                    break;
                default:
                    break;
                case UserActions.deleteNode:
					if (selectedNode.drawNode is TransitionNode)
					{
						BaseNode enterNode = settings.currentGraph.GetNodeWithIndex(selectedNode.enterNode);
						if (enterNode != null)
							enterNode.stateRef.currentState.RemoveTransition(selectedNode.transRef.transitionId);
					}

					nodesToDelete++;
                    settings.currentGraph.DeleteNode(selectedNode.id);
                    break;
                case UserActions.makeTransition:
                    transitFromId = selectedNode.id;
                    settings.makeTransition = true;
                    break;
				case UserActions.resetPan:
					ResetScroll();
					break;
            }

			forceSetDirty = true;
        
		}

		public static BaseNode AddTransitionNode(BaseNode enterNode, Vector3 pos)
		{
			BaseNode transNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 100, "Condition", pos);
			transNode.enterNode = enterNode.id;
			Transition t = settings.stateNode.AddTransition(enterNode);
			transNode.transRef.transitionId = t.id;
			return transNode;
		}

		public static BaseNode AddTransitionNodeFromTransition(Transition transition, BaseNode enterNode, Vector3 pos)
		{
			BaseNode transNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 100, "Condition", pos);
			transNode.enterNode = enterNode.id;
			transNode.transRef.transitionId = transition.id;
			return transNode;

		}

		#endregion

		#region Helper Methods
		public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3(
                (left) ? start.x + start.width : start.x,
                start.y + (start.height *.5f),
                0);

            Vector3 endPos = new Vector3(end.x + (end.width * .5f), end.y + (end.height * .5f), 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 1);
            for (int i = 0; i < 1; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, 4);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 3);
        }

        public static void ClearWindowsFromList(List<BaseNode>l)
        {
            for (int i = 0; i < l.Count; i++)
            {
          //      if (windows.Contains(l[i]))
            //        windows.Remove(l[i]);
            }
        }
        
        #endregion

    }
}
