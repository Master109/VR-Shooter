using UnityEngine;
using GameDevJourney;

namespace VRShooter
{
	public class Player : SingletonMonoBehaviour<Player>
	{
		public Transform headTrs;
		public Rigidbody rigid;
		public Gun leftGun;
		public Gun rightGun;
	}
}