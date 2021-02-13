using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class QuaternionExtensions
	{
		public static Quaternion NULL = new Quaternion(MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT);

		public static Quaternion[] GetRotationsForTurn (Quaternion startRotation, Quaternion endRotation, int stepCount)
		{
			Quaternion[] rotations = new Quaternion[stepCount];
			float angle = Quaternion.Angle(startRotation, endRotation);
			for (int i = 0; i < stepCount; i ++)
			{
				Quaternion rotation = Quaternion.RotateTowards(startRotation, endRotation, angle / stepCount * (i + 1));
				rotations[i] = rotation;
			}
			return rotations;
		}
	}
}