using UnityEngine;
using GameDevJourney;

namespace VRShooter
{
	public class Gun : MonoBehaviour
	{
		public Transform trs;
		public Transform shootTrs;
		public Timer reloadTimer;

		public virtual bool Shoot ()
		{
			if (reloadTimer.timeRemaining <= 0)
			{
				reloadTimer.Reset ();
				reloadTimer.Start ();
				return true;
			}
			return false;
		}
	}
}