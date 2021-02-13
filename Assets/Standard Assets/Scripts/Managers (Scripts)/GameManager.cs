using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameDevJourney
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
#if UNITY_EDITOR
		public static int[] UniqueIds
		{
			get
			{
				int[] output = new int[0];
				string[] uniqueIdsString = EditorPrefs.GetString("Unique ids").Split(UNIQUE_ID_SEPERATOR);
				int uniqueIdParsed;
				foreach (string uniqueIdString in uniqueIdsString)
				{
					if (int.TryParse(uniqueIdString, out uniqueIdParsed))
						output = output.Add(uniqueIdParsed);
				}
				return output;
			}
			set
			{
				string uniqueIdString = "";
				foreach (int uniqueId in value)
					uniqueIdString += uniqueId + UNIQUE_ID_SEPERATOR;
				EditorPrefs.SetString("Unique ids", uniqueIdString);
			}
		}
		public static bool doEditorUpdates = false;
#endif
		public static IUpdatable[] updatables = new IUpdatable[0];
		public static IUpdatable[] pausedUpdatables = new IUpdatable[0];
		public static float UnscaledDeltaTime
		{
			get
			{
				if (paused || framesSinceLoadedScene <= LAG_FRAMES_AFTER_LOAD_SCENE)
					return 0;
				else
					return Time.unscaledDeltaTime;
			}
		}
		public static bool paused;
		public GameObject[] registeredGos = new GameObject[0];
		[SaveAndLoadValue(false)]
		public static string enabledGosString = "";
		[SaveAndLoadValue(false)]
		public static string disabledGosString = "";
		public const string STRING_SEPERATOR = "|";
		public float timeScale;
		public const char UNIQUE_ID_SEPERATOR = ',';
		public static int framesSinceLoadedScene;
		public const int LAG_FRAMES_AFTER_LOAD_SCENE = 2;
		public RectTransform gameViewRectTrs;
		public Canvas gameViewCanvas;
		public static bool isFocused;
		public CursorEntry[] cursorEntries;
		public static Dictionary<string, CursorEntry> cursorEntriesDict = new Dictionary<string, CursorEntry>();
		public static CursorEntry activeCursorEntry;
		public RectTransform cursorCanvasRectTrs;
		public float cursorMoveSpeedPerScreenArea;
		public static float cursorMoveSpeed;
		public static Vector2 previousMousePosition;
		public GameModifier[] gameModifiers;
		public static Dictionary<string, GameModifier> gameModifierDict = new Dictionary<string, GameModifier>();
		public static Vector2 mousePosition;
		
		public virtual void Update ()
		{
			// try
			// {
				InputSystem.Update ();
				mousePosition = InputManager.GetMousePosition(MathfExtensions.NULL_INT);
				for (int i = 0; i < updatables.Length; i ++)
				{
					IUpdatable updatable = updatables[i];
					updatable.DoUpdate ();
				}
				Physics2D.Simulate(Time.deltaTime);
				ObjectPool.Instance.DoUpdate ();
				framesSinceLoadedScene ++;
				previousMousePosition = mousePosition;
			// }
			// catch (Exception e)
			// {
			// 	print(e.Message + "\n" + e.StackTrace);
			// }
		}
		
		public void PauseGame (bool pause)
		{
			paused = pause;
			Time.timeScale = timeScale * (1 - paused.GetHashCode());
			AudioListener.pause = paused;
		}

		public void LoadScene (string name)
		{
			if (Instance != this)
			{
				instance.LoadScene (name);
				return;
			}
			framesSinceLoadedScene = 0;
			SceneManager.LoadScene(name);
		}

		public void LoadSceneAdditive (string name)
		{
			if (Instance != this)
			{
				instance.LoadSceneAdditive (name);
				return;
			}
			SceneManager.LoadScene(name, LoadSceneMode.Additive);
		}

		public void LoadScene (int index)
		{
			LoadScene (SceneManager.GetSceneByBuildIndex(index).name);
		}

		public void UnloadScene (string name)
		{
			AsyncOperation unloadGameScene = SceneManager.UnloadSceneAsync(name);
			unloadGameScene.completed += OnGameSceneUnloaded;
		}

		public void OnGameSceneUnloaded (AsyncOperation unloadGameScene)
		{
			unloadGameScene.completed -= OnGameSceneUnloaded;
		}

		public void ReloadActiveScene ()
		{
			LoadScene (SceneManager.GetActiveScene().name);
		}

		public void OnApplicationFocus (bool isFocused)
		{
			GameManager.isFocused = isFocused;
			if (isFocused)
			{
				foreach (IUpdatable pausedUpdatable in pausedUpdatables)
					updatables = updatables.Add(pausedUpdatable);
				pausedUpdatables = new IUpdatable[0];
				foreach (Timer runningTimer in Timer.runningInstances)
					runningTimer.pauseIfCan = false;
				foreach (TemporaryActiveObject tempActiveObject in TemporaryActiveObject.activeInstances)
					tempActiveObject.Do ();
			}
			else
			{
				IUpdatable updatable;
				for (int i = 0; i < updatables.Length; i ++)
				{
					updatable = updatables[i];
					if (!updatable.PauseWhileUnfocused)
					{
						pausedUpdatables = pausedUpdatables.Add(updatable);
						updatables = updatables.RemoveAt(i);
						i --;
					}
				}
				foreach (Timer runningTimer in Timer.runningInstances)
					runningTimer.pauseIfCan = true;
				foreach (TemporaryActiveObject tempActiveObject in TemporaryActiveObject.activeInstances)
					tempActiveObject.Do ();
			}
		}

		public void SetGosActive ()
		{
			if (Instance != this)
			{
				instance.SetGosActive ();
				return;
			}
			string[] stringSeperators = { STRING_SEPERATOR };
			string[] enabledGos = enabledGosString.Split(stringSeperators, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < enabledGos.Length; i ++)
			{
				string goName = enabledGos[i];
				for (int i2 = 0; i2 < registeredGos.Length; i2 ++)
				{
					GameObject registeredGo = registeredGos[i2];
					if (goName == registeredGo.name)
					{
						registeredGo.SetActive(true);
						break;
					}
				}
			}
			string[] disabledGos = disabledGosString.Split(stringSeperators, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < disabledGos.Length; i ++)
			{
				string goName = disabledGos[i];
				GameObject go = GameObject.Find(goName);
				if (go != null)
					go.SetActive(false);
			}
		}
		
		public void ActivateGoForever (GameObject go)
		{
			go.SetActive(true);
			ActivateGoForever (go.name);
		}
		
		public void DeactivateGoForever (GameObject go)
		{
			go.SetActive(false);
			DeactivateGoForever (go.name);
		}
		
		public void ActivateGoForever (string goName)
		{
			disabledGosString = disabledGosString.Replace(STRING_SEPERATOR + goName, "");
			if (!enabledGosString.Contains(goName))
				enabledGosString += STRING_SEPERATOR + goName;
		}
		
		public void DeactivateGoForever (string goName)
		{
			enabledGosString = enabledGosString.Replace(STRING_SEPERATOR + goName, "");
			if (!disabledGosString.Contains(goName))
				disabledGosString += STRING_SEPERATOR + goName;
		}

		public void SetGameObjectActive (string name)
		{
			GameObject.Find(name).SetActive(true);
		}

		public void SetGameObjectInactive (string name)
		{
			GameObject.Find(name).SetActive(false);
		}

		public void Quit ()
		{
			Application.Quit();
		}

		public void _LogError (string str)
		{
			LogError (str);
		}

		public static void LogError (object o)
		{
			Debug.LogError(o);
		}

		public void _Log (string str)
		{
			Log (str);
		}

		public static void Log (object obj)
		{
			print(obj);
		}

		public static Object Clone (Object obj)
		{
			return Instantiate(obj);
		}

		public static Object Clone (Object obj, Transform parent)
		{
			return Instantiate(obj, parent);
		}

		public static Object Clone (Object obj, Vector3 position, Quaternion rotation)
		{
			return Instantiate(obj, position, rotation);
		}

		public static void _Destroy (Object obj)
		{
			Destroy(obj);
		}

		public static void _DestroyImmediate (Object obj)
		{
			DestroyImmediate(obj);
		}

		public void ToggleGo (GameObject go)
		{
			go.SetActive(!go.activeSelf);
		}

		public void PressButton (Button button)
		{
			button.onClick.Invoke();
		}

		// public static T GetSingleton<T> ()
		// {
		// 	object obj = null;
		// 	if (!singletons.TryGetValue(typeof(T), out obj))
		// 		obj = GetSingleton<T>(FindObjectsOfType<Object>());
		// 	return (T) obj;
		// }

		// public static T GetSingleton<T> (Object[] objects)
		// {
		// 	if (typeof(T).IsSubclassOf(typeof(Object)))
		// 	{
		// 		for (int i = 0; i < objects.Length; i ++)
		// 		{
		// 			Object obj = objects[i];
		// 			if (obj is T)
		// 			{
		// 				if (singletons.ContainsKey(typeof(T)))
		// 					singletons[typeof(T)] = obj;
		// 				else
		// 					singletons.Add(typeof(T), obj);
		// 				return (T) (object) obj;
		// 			}
		// 		}
		// 	}
		// 	return (T) (object) null;
		// }

		public static bool ModifierIsActiveAndExists (string name)
		{
			GameModifier gameModifier;
			if (gameModifierDict.TryGetValue(name, out gameModifier))
				return gameModifier.isActive;
			else
				return false;
		}

		public static bool ModifierIsActive (string name)
		{
			return gameModifierDict[name].isActive;
		}

		public static bool ModifierExists (string name)
		{
			return gameModifierDict.ContainsKey(name);
		}

		[Serializable]
		public class CursorEntry
		{
			public string name;
			public RectTransform rectTrs;

			public void SetAsActive ()
			{
				if (activeCursorEntry != null)
					activeCursorEntry.rectTrs.gameObject.SetActive(false);
				rectTrs.gameObject.SetActive(true);
				activeCursorEntry = this;
			}
		}

		[Serializable]
		public class GameModifier
		{
			public string name;
			public bool isActive;
		}
	}
}