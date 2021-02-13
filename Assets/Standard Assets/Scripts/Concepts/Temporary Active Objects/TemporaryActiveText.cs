using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

namespace GameDevJourney
{
	[Serializable]
	public class TemporaryActiveText : TemporaryActiveObject
	{
		public TMP_Text text;
		public float durationPerCharacter;
		
		public override IEnumerator DoRoutine ()
		{
			duration = text.text.Length * durationPerCharacter;
			yield return base.DoRoutine ();
		}
	}
}