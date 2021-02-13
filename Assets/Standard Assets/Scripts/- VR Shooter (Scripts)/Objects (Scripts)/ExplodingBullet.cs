using UnityEngine;
using GameDevJourney;

namespace VRShooter
{
	public class ExplodingBullet : Bullet
	{
		void OnCollisionEnter (Collision coll)
		{
			ObjectPool.instance.Despawn (prefabIndex, gameObject, trs);
		}
	}
}