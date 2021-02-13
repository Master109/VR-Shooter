using UnityEngine;
using GameDevJourney;

namespace VRShooter
{
	public class Bullet : Spawnable
	{
		public Rigidbody rigid;
		public Collider collider;
		[HideInInspector]
		public Gun shootingGun;

		void OnDisable ()
		{
			collider.enabled = false;
			Physics.IgnoreCollision(shootingGun.collider, collider, false);
		}
	}
}