using UnityEngine;
using GameDevJourney;

namespace VRShooter
{
	public class Gun : UpdateWhileEnabled
	{
		public Transform trs;
		public Transform shootTrs;
		public Timer reloadTimer;
		public float shootSpeed;
		public Bullet bulletPrefab;
		public Collider collider;
		Vector3 previousShootTrsPosition;
		Quaternion previousShootTrsRotation;
		float deltaTime;

		public override void DoUpdate ()
		{
			previousShootTrsPosition = shootTrs.position;
			previousShootTrsRotation = shootTrs.rotation;
			deltaTime = Time.deltaTime;
		}

		public virtual bool Shoot ()
		{
			if (reloadTimer.timeRemaining <= 0)
			{
				reloadTimer.Reset ();
				reloadTimer.Start ();
				if (bulletPrefab != null)
				{
					Bullet bullet = ObjectPool.instance.SpawnComponent<Bullet>(bulletPrefab.prefabIndex, shootTrs.position, shootTrs.rotation);
					Vector3 velocity = PhysicsUtilities.GetVelocity(previousShootTrsPosition, shootTrs.position, deltaTime);
					Vector3 angularVelocity = PhysicsUtilities.GetAngularVelocity(previousShootTrsRotation, shootTrs.rotation, deltaTime);
					bullet.rigid.velocity = shootTrs.forward * shootSpeed + PhysicsUtilities.GetPointVelocity(velocity, angularVelocity, Vector3.zero);
					Physics.IgnoreCollision(bullet.collider, collider, true);
					bullet.shootingGun = this;
					bullet.collider.enabled = true;
				}
				return true;
			}
			return false;
		}
	}
}