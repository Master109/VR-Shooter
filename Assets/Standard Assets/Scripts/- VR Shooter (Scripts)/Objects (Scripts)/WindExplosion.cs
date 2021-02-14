using System.Collections.Generic;
using UnityEngine;
using GameDevJourney;
using Extensions;

namespace VRShooter
{
	public class WindExplosion : UpdateWhileEnabled, ISpawnable
	{
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}
		public Transform trs;
		public Collider collider;
		public float duration;
		public float minForce;
		public float maxForce;
		public float forceMultiplier;
		public float forceToSizeMultiplier;
		List<Collider> hitColliders = new List<Collider>();
		float force;

		public override void DoUpdate ()
		{
			trs.localScale += Vector3.one * force * forceToSizeMultiplier * Time.deltaTime / duration;
			if (trs.localScale.x > force * forceToSizeMultiplier)
				ObjectPool.instance.Despawn (prefabIndex, gameObject, trs);
		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			trs.localScale = Vector3.zero;
			hitColliders.Clear();
		}

		public void SetForce (float force)
		{
			this.force = Mathf.Clamp(force * forceMultiplier, minForce, maxForce);
		}

		void OnTriggerEnter (Collider other)
		{
			if (hitColliders.Contains(other))
				return;
			hitColliders.Add(other);
			Vector3 addToVelocity = (other.GetComponent<Transform>().position - trs.position).normalized * force;
			Rigidbody rigid = other.GetComponent<Rigidbody>();
			if (rigid != null)
				rigid.AddForce(addToVelocity, ForceMode.Impulse);
		}
	}
}