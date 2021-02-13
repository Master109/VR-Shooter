﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevJourney
{
	[RequireComponent(typeof(Toggle))]
	public class _Toggle : _Selectable
	{
		[HideInInspector]
		public Toggle toggle;
		[HideInInspector]
		public List<_ToggleGroup> toggleGroups;
		
		public virtual void Awake ()
		{
			toggle = GetComponent<Toggle>();
			toggle.onValueChanged.Invoke (toggle.isOn);
		}
		
		public virtual void OnToggle ()
		{
			foreach (_ToggleGroup toggleGroup in toggleGroups)
				toggleGroup.OnUpdate ();
		}
		
		public void SetIsOn (bool isOn)
		{
			this.toggle.isOn = isOn;
		}
	}
}