using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Conditions/Is Dead")]
    public class IsDead : Condition
    {
		private void OnEnable()
		{
			description = "Is health 0 or less?";
		}

		public override bool CheckCondition(StateManager state)
        {
            return state.health <= 0;
        }

    }
}
