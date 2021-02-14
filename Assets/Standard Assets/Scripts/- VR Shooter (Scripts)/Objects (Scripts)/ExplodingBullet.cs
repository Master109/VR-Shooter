using UnityEngine;
using GameDevJourney;
using Extensions;

namespace VRShooter
{
	public class ExplodingBullet : Bullet, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}
		public float lifetime;
		public WindExplosion windExplosionPrefab;
		float speedSqr;
		ObjectPool.DelayedDespawn delayedDespawn;

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
			delayedDespawn = ObjectPool.instance.DelayDespawn(prefabIndex, gameObject, trs, lifetime);
		}

		public void DoUpdate ()
		{
			speedSqr = rigid.velocity.sqrMagnitude;
		}

		public override void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
			if (collider.enabled)
				Explode (trs.position);
			base.OnDisable ();
		}

		void OnCollisionEnter (Collision coll)
		{
			ObjectPool.instance.CancelDelayedDespawn (delayedDespawn);
			Explode (coll.GetContact(0).point);
			ObjectPool.instance.Despawn (prefabIndex, gameObject, trs);
		}

		void Explode (Vector3 position)
		{
			WindExplosion windExplosion = ObjectPool.instance.SpawnComponent<WindExplosion>(windExplosionPrefab.prefabIndex, position);
			windExplosion.SetForce (Mathf.Sqrt(speedSqr));
		}
	}
}