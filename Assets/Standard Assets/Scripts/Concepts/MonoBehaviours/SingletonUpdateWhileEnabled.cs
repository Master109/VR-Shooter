using UnityEngine;

namespace GameDevJourney
{
	public class SingletonUpdateWhileEnabled<T> : UpdateWhileEnabled where T : UpdateWhileEnabled
	{
		public static T instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<T>();
				return instance;
			}
		}
		public MultipleInstancesHandlingType handleMultipleInstances;
		public bool persistant;
		
		public virtual void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (handleMultipleInstances != MultipleInstancesHandlingType.KeepAll && Instance != null && instance != this)
			{
				if (handleMultipleInstances == MultipleInstancesHandlingType.DestroyNew)
				{
					Destroy(gameObject);
					return;
				}
				else
					Destroy(instance.gameObject);
			}
			if (persistant)
				DontDestroyOnLoad(gameObject);
		}

		public enum MultipleInstancesHandlingType
		{
			KeepAll,
			DestroyNew,
			DestroyOld
		}
	}
}