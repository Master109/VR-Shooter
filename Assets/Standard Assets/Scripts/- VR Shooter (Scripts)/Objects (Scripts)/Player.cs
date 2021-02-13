using UnityEngine;
using GameDevJourney;

namespace VRShooter
{
	public class Player : SingletonUpdateWhileEnabled<Player>
	{
		public Transform headTrs;
		public Rigidbody rigid;
		public Gun leftGun;
		public Gun rightGun;

		public override void DoUpdate ()
		{
			HandleShooting ();
		}

		void HandleShooting ()
		{
			if (InputManager.LeftShootInput)
				leftGun.Shoot ();
			if (InputManager.RightShootInput)
				rightGun.Shoot ();
		}
	}
}