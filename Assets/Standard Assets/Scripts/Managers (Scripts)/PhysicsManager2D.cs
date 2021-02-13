using System;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GameDevJourney
{
	[ExecuteInEditMode]
	public class PhysicsManager2D : SingletonMonoBehaviour<PhysicsManager2D>
	{
		public LayerCollision[] layerCollisions = new LayerCollision[0];
		public static Dictionary<string, string[]> layerCollisionsDict = new Dictionary<string, string[]>();
		public static string[] realLayerNames = new string[0];

		public override void Awake ()
		{
			base.Awake ();
			layerCollisionsDict.Clear();
			List<string> layerNamesList = new List<string>();
			for (int i = 0; i < layerCollisions.Length; i ++)
			{
				LayerCollision layerCollision = layerCollisions[i];
				layerCollisionsDict.Add(layerCollision.layerName, layerCollision.collidingLayers);
				layerNamesList.Add(layerCollision.layerName);
			}
			int realLayerCount = Mathf.Min(32, layerNamesList.Count);
			realLayerNames = layerNamesList.ToArray().GetCopy(realLayerCount);
			for (int i = 0; i < realLayerCount; i ++)
			{
				string realLayerName = realLayerNames[i];
				string[] collidingRealLayers = layerCollisionsDict[realLayerName];
				for (int i2 = 0; i2 < realLayerCount; i2 ++)
				{
					string realLayerName2 = realLayerNames[i2];
					Physics2D.IgnoreLayerCollision(i, i2, !collidingRealLayers.Contains(realLayerName2));
				}
			}
		}

		[Serializable]
		public struct LayerCollision
		{
			public string layerName;
			public string[] collidingLayers;
		}
	}
}