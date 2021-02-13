using UnityEngine;
using GameDevJourney;

namespace VRShooter
{
	public class Gun : MonoBehaviour
	{
		public Transform trs;
		public Transform shootTrs;
		public Timer reloadTimer;
		public float shootSpeed;
		public Bullet bulletPrefab;

		public virtual bool Shoot ()
		{
			if (reloadTimer.timeRemaining <= 0)
			{
				reloadTimer.Reset ();
				reloadTimer.Start ();
				if (bulletPrefab != null)
				{
					Bullet bullet = ObjectPool.instance.SpawnComponent<Bullet>(bulletPrefab.prefabIndex, shootTrs.position, shootTrs.rotation);
					bullet.rigid.velocity = bullet.trs.forward * shootSpeed;
				}
				return true;
			}
			return false;
		}
	}
}