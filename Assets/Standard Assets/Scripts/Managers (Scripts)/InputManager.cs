using UnityEngine;
using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace GameDevJourney
{
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public InputSettings settings;
		public static InputSettings Settings
		{
			get
			{
				return InputManager.Instance.settings;
			}
		}
		public static bool UsingGamepad
		{
			get
			{
				return Gamepad.current != null;
			}
		}
		public static bool UsingMouse
		{
			get
			{
				return Mouse.current != null;
			}
		}
		public static bool UsingKeyboard
		{
			get
			{
				return Keyboard.current != null;
			}
		}
		public Vector2 UIMovementInput
		{
			get
			{
				return GetUIMovementInput(MathfExtensions.NULL_INT);
			}
		}
		public Vector2 _UIMovementInput
		{
			get
			{
				return UIMovementInput;
			}
		}
		public static bool SubmitInput
		{
			get
			{
				return GetSubmitInput(MathfExtensions.NULL_INT);
			}
		}
		public bool _SubmitInput
		{
			get
			{
				return SubmitInput;
			}
		}
		public static Vector2 MousePosition
		{
			get
			{
				return GetMousePosition(MathfExtensions.NULL_INT);
			}
		}
		public Vector2 _MousePosition
		{
			get
			{
				return MousePosition;
			}
		}
		
		public virtual void Start ()
		{
			InputSystem.onDeviceChange += OnDeviceChanged;
		}
		
		public virtual void OnDeviceChanged (InputDevice device, InputDeviceChange change)
		{
			if (device is Gamepad)
			{
				if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
				{
					GameManager.activeCursorEntry.rectTrs.gameObject.SetActive(false);
					if (VirtualKeyboard.Instance != null)
						VirtualKeyboard.instance.outputToInputField.readOnly = true;
				}
				else if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected)
				{
					GameManager.activeCursorEntry.rectTrs.gameObject.SetActive(true);
					if (VirtualKeyboard.Instance != null)
						VirtualKeyboard.instance.outputToInputField.readOnly = false;
				}
				foreach (_Text text in _Text.instances)
					text.UpdateText ();
			}
		}
		
		public virtual void OnDestroy ()
		{
			InputSystem.onDeviceChange -= OnDeviceChanged;
		}

		public static Vector2 GetUIMovementInput (int playerIndex)
		{
			if (UsingGamepad)
				return Vector2.ClampMagnitude(GetGamepad(playerIndex).leftStick.ReadValue(), 1);
			else
			{
				Keyboard keyboard = GetKeyboard(playerIndex);
				int x = 0;
				if (keyboard.dKey.isPressed)
					x ++;
				if (keyboard.aKey.isPressed)
					x --;
				if (x == 0)
				{
					if (keyboard.leftArrowKey.isPressed)
						x --;
					if (keyboard.rightArrowKey.isPressed)
						x ++;
				}
				int y = 0;
				if (keyboard.wKey.isPressed)
					y ++;
				if (keyboard.sKey.isPressed)
					y --;
				if (x == 0)
				{
					if (keyboard.downArrowKey.isPressed)
						y --;
					if (keyboard.upArrowKey.isPressed)
						y ++;
				}
				return Vector2.ClampMagnitude(new Vector2(x, y), 1);
			}
		}

		public static bool GetSubmitInput (int playerIndex)
		{
			if (UsingGamepad)
				return GetGamepad(playerIndex).aButton.isPressed;
			else
				return GetKeyboard(playerIndex).enterKey.isPressed;
		}

		public static bool GetLeftClickInput (int playerIndex)
		{
			if (UsingMouse)
				return GetMouse(playerIndex).leftButton.isPressed;
			else
				return false;
		}

		public static Vector2 GetMousePosition (int playerIndex)
		{
			if (UsingMouse)
				return GetMouse(playerIndex).position.ReadValue();
			else
				return GameManager.activeCursorEntry.rectTrs.position;
		}

		public static Vector2 GetWorldMousePosition (int playerIndex)
		{
			Rect gameViewRect = GameManager.Instance.gameViewRectTrs.GetWorldRect();
			return CameraScript.Instance.camera.ViewportToWorldPoint(GetMousePosition(playerIndex));
		}

		public static Gamepad GetGamepad (int playerIndex)
		{
			Gamepad gamepad = Gamepad.current;
			if (Gamepad.all.Count > playerIndex)
				gamepad = Gamepad.all[playerIndex];
			return gamepad;
		}

		public static Mouse GetMouse (int playerIndex)
		{
			Mouse mouse = Mouse.current;
			// if (Mouse.all.Count > playerIndex)
			// 	mouse = (Mouse) Mouse.all[playerIndex];
			return mouse;
		}

		public static Keyboard GetKeyboard (int playerIndex)
		{
			Keyboard keyboard = Keyboard.current;
			// if (Keyboard.all.Count > playerIndex)
			// 	keyboard = (Keyboard) Keyboard.all[playerIndex];
			return keyboard;
		}
	}
}