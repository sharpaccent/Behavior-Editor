using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA;
using UnityEditor;
using UnityEditorInternal;

namespace SA.CustomUI
{
	[CustomEditor(typeof(State))]
	public class StateGUI : Editor
	{
		SerializedObject serializedState;
		ReorderableList onFixedList;
		ReorderableList onUpdateList;
		ReorderableList onEnterList;
		ReorderableList onExitList;
		ReorderableList Transitions;

		bool showDefaultGUI = false;
		bool showActions = true;
		bool showTransitions = true;

		private void OnEnable()
		{
			serializedState = null;
		}

		public override void OnInspectorGUI()
		{
			showDefaultGUI = EditorGUILayout.Toggle("DefaultGUI", showDefaultGUI);
			if (showDefaultGUI)
			{
				base.OnInspectorGUI();
				return;
			}

			showActions = EditorGUILayout.Toggle("Show Actions", showActions);

			if(serializedState == null)
				SetupReordableLists();

			serializedState.Update();

			if (showActions)
			{	
				EditorGUILayout.LabelField("Actions that execute on FixedUpdate()");
				onFixedList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute on Update()");
				onUpdateList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute when entering this State");
				onEnterList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute when exiting this State");
				onExitList.DoLayoutList();	
			}

			showTransitions = EditorGUILayout.Toggle("Show Transitions", showTransitions);

			if (showTransitions)
			{
				EditorGUILayout.LabelField("Conditions to exit this State");
				Transitions.DoLayoutList();
			}

			serializedState.ApplyModifiedProperties();
		}

		void SetupReordableLists()
		{
			State curState = (State)target;
			serializedState = new SerializedObject(curState);
			onFixedList = new ReorderableList(serializedState,serializedState.FindProperty("onFixed"), true, true, true, true);
			onUpdateList = new ReorderableList(serializedState,serializedState.FindProperty("onUpdate"), true, true, true, true);
			onEnterList = new ReorderableList(serializedState,serializedState.FindProperty("onEnter"), true, true, true, true);
			onExitList = new ReorderableList(serializedState,serializedState.FindProperty("onExit"), true, true, true, true);
			Transitions = new ReorderableList(serializedState, serializedState.FindProperty("transitions"), true, true, true, true);

			HandleReordableList(onFixedList, "On Fixed");
			HandleReordableList(onUpdateList, "On Update");
			HandleReordableList(onEnterList, "On Enter");
			HandleReordableList(onExitList, "On Exit");
			HandleTransitionReordable(Transitions, "Condition --> New State");
		}

		void HandleReordableList(ReorderableList list, string targetName)
		{
			list.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, targetName);
			};

			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};
		}

		void HandleTransitionReordable(ReorderableList list, string targetName)
		{
			list.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, targetName);
			};

			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width * .3f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("condition"), GUIContent.none);
				EditorGUI.ObjectField(new Rect(rect.x + + (rect.width *.35f), rect.y, rect.width * .3f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("targetState"), GUIContent.none);
				EditorGUI.LabelField(new Rect(rect.x + +(rect.width * .75f), rect.y, rect.width * .2f, EditorGUIUtility.singleLineHeight), "Disable");
				EditorGUI.PropertyField(new Rect(rect.x + +(rect.width * .90f), rect.y, rect.width * .2f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("disable"), GUIContent.none);

			};
		}

	}
}
