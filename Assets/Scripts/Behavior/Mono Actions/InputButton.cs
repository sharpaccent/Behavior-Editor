using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Inputs/Button")]
	public class InputButton : Action
	{
		public string targetInput;
		public bool isPressed;
		public KeyState keyState;
		public bool updateBoolVar = true;
		public SO.BoolVariable targetBoolVariable;


		public override void Execute()
		{
			switch (keyState)
			{
				case KeyState.onDown:
					isPressed = Input.GetButtonDown(targetInput);
					break;
				case KeyState.onCurrent:
					isPressed = Input.GetButton(targetInput);
					break;
				case KeyState.onUp:
					isPressed = Input.GetButtonUp(targetInput);
					break;
				default:
					break;
			}

			if (updateBoolVar)
			{
				if (targetBoolVariable != null)
				{
					targetBoolVariable.value = isPressed;
				}
			}
		}

		public enum KeyState
		{
			onDown,onCurrent,onUp
		}
	}
}
