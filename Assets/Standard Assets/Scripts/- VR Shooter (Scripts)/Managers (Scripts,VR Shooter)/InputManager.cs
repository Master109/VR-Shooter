using UnityEngine;
using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace VRShooter
{
	public class InputManager : GameDevJourney.InputManager
	{
		public static bool LeftShootInput
		{
			get
			{
				return GetLeftShootInput(MathfExtensions.NULL_INT);
			}
		}
		public bool _LeftShootInput
		{
			get
			{
				return LeftShootInput;
			}
		}
		bool leftShootInput;
		public InputAction leftShootInputAction;

		public override void Start ()
		{
			base.Start ();
			leftShootInputAction.performed += LeftShootInputActionUpdate;
		}

		public override void OnDestroy ()
		{
			leftShootInputAction.performed -= LeftShootInputActionUpdate;
		}

		public static bool GetLeftShootInput (int playerIndex)
		{
			return GetKeyboard(playerIndex).enterKey.isPressed;
		}

		void LeftShootInputActionUpdate (InputAction.CallbackContext context)
		{
			leftShootInput = context.ReadValue<bool>();
		}
	}
}