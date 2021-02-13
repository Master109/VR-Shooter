using UnityEngine;
using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace VRShooter
{
	public class InputManager : GameDevJourney.InputManager
	{
		public new static InputManager instance;
		public new static InputManager Instance
		{
			get
			{
				return instance;
			}
			set
			{
				instance = value;
			}
		}
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
		public static bool RightShootInput
		{
			get
			{
				return GetRightShootInput(MathfExtensions.NULL_INT);
			}
		}
		public bool _RightShootInput
		{
			get
			{
				return RightShootInput;
			}
		}
		bool rightShootInput;
		public InputAction rightShootInputAction;

		public override void Start ()
		{
			base.Start ();
			instance = this;
			leftShootInputAction.performed += LeftShootInputActionUpdate;
			leftShootInputAction.Enable();
			rightShootInputAction.performed += RightShootInputActionUpdate;
			rightShootInputAction.Enable();
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			leftShootInputAction.Disable();
			leftShootInputAction.performed -= LeftShootInputActionUpdate;
			rightShootInputAction.Disable();
			rightShootInputAction.performed -= RightShootInputActionUpdate;
		}

		public static bool GetLeftShootInput (int playerIndex)
		{
			return Instance.leftShootInput;
		}

		void LeftShootInputActionUpdate (InputAction.CallbackContext context)
		{
			leftShootInput = context.ReadValue<float>() > settings.defaultDeadzoneMin;
		}

		public static bool GetRightShootInput (int playerIndex)
		{
			return Instance.rightShootInput;
		}

		void RightShootInputActionUpdate (InputAction.CallbackContext context)
		{
			rightShootInput = context.ReadValue<float>() > settings.defaultDeadzoneMin;
		}
	}
}