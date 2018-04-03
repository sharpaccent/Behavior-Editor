using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.BehaviorEditor
{
    [CreateAssetMenu]
    public class BehaviorGraph : ScriptableObject
    {
	[SerializeField]
        public List<BaseNode> windows = new List<BaseNode>();
	[SerializeField]
        public int idCount;
        List<int> indexToDelete = new List<int>();

        #region Checkers
        public BaseNode GetNodeWithIndex(int index)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].id == index)
                    return windows[i];
            }

            return null;
        }

        public void DeleteWindowsThatNeedTo()
        {
            for (int i = 0; i < indexToDelete.Count; i++)
            {
                BaseNode b = GetNodeWithIndex(indexToDelete[i]);
                if(b != null)
                    windows.Remove(b);
            }

            indexToDelete.Clear();
        }

        public void DeleteNode(int index)
        {
			if(!indexToDelete.Contains(index))
				indexToDelete.Add(index);
        }

        public bool IsStateDuplicate(BaseNode b)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].id == b.id)
                    continue;

                if (windows[i].stateRef.currentState == b.stateRef.currentState &&
                    !windows[i].isDuplicate)
                    return true;
            }
             
            return false;
        }

        public bool IsTransitionDuplicate(BaseNode b)
        {
            BaseNode enter = GetNodeWithIndex(b.enterNode);
            if (enter == null)
            {
                Debug.Log("false");
                return false;
            }
            for (int i = 0; i < enter.stateRef.currentState.transitions.Count; i++)
            {
                Transition t = enter.stateRef.currentState.transitions[i];
                if (t.condition == b.transRef.previousCondition && b.transRef.transitionId != t.id)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}
