using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace GameDevJourney
{
	public static class PhysicsUtilities
	{
		public static RaycastHit2D LinecastWithWidth (Vector2 start, Vector2 end, float width, int layerMask)
		{
			float distance = Vector2.Distance(start, end);
			return Physics2D.BoxCast((start + end) / 2, new Vector2(distance, width), (end - start).GetFacingAngle(), end - start, distance, layerMask);
		}

		public static RaycastHit2D[] LinecastAllWithWidth (Vector2 start, Vector2 end, float width, int layerMask)
		{
			float distance = Vector2.Distance(start, end);
			return Physics2D.BoxCastAll((start + end) / 2, new Vector2(distance, width), (end - start).GetFacingAngle(), end - start, distance, layerMask);
		}
		
		public static int LinecastWithWidth (Vector2 start, Vector2 end, float width, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			float distance = Vector2.Distance(start, end);
			return Physics2D.BoxCast((start + end) / 2, new Vector2(distance, width), (end - start).GetFacingAngle(), end - start, contactFilter, results, distance);
		}
		
		public static int LinecastWithWidth (Vector2 start, Vector2 end, float width, ContactFilter2D contactFilter, List<RaycastHit2D> results)
		{
			float distance = Vector2.Distance(start, end);
			return Physics2D.BoxCast((start + end) / 2, new Vector2(distance, width), (end - start).GetFacingAngle(), end - start, contactFilter, results, distance);
		}

		public static RaycastHit2D[] LinecastAllWithWidthAndOrder (Vector2 start, Vector2 end, float width, int layerMask)
		{
			LineSegment2D lineSegment = new LineSegment2D(start, end);
			List<RaycastHit2D> hits = new List<RaycastHit2D>();
			RaycastHit2D hit;
			do
			{
				hit = LinecastWithWidth(lineSegment.start, end, width, layerMask);
				if (hit.collider != null)
				{
					lineSegment.start = lineSegment.GetPointWithDirectedDistance(lineSegment.GetDirectedDistanceAlongParallel(hit.point));
					hits.Add(hit);
				}
			} while (hit.collider != null);
			return hits.ToArray();
		}

		public static Vector3 GetPointVelocity (Vector3 velocity, Vector3 angularVelocity, Vector3 localPoint)
		{
			return velocity + Vector3.Cross(angularVelocity, localPoint);
		}

		public static Vector3 GetPointVelocity (Vector3 previousPosition, Quaternion previousRotation, Transform trs, Vector3 localPoint)
		{
			return GetPointVelocity(GetVelocity(trs.position, previousPosition), GetAngularVelocity(previousRotation, trs.rotation), localPoint);
		}

		public static Vector3 GetPointVelocity (Vector3 previousPosition, Quaternion previousRotation, Transform trs, Vector3 localPoint, float deltaTime)
		{
			return GetPointVelocity(GetVelocity(trs.position, previousPosition, deltaTime), GetAngularVelocity(previousRotation, trs.rotation, deltaTime), localPoint);
		}

		public static Vector3 GetAngularVelocity (Quaternion fromRotation, Quaternion toRotation, float deltaTime)
		{
			Quaternion q = toRotation * Quaternion.Inverse(fromRotation);
			if (Mathf.Abs(q.w) > 1023.5f / 1024.0f)
				return new Vector3(0 ,0, 0);
			float gain;
			if (q.w < 0.0f)
			{
				float angle = Mathf.Acos(-q.w);
				gain = -2.0f * angle / (Mathf.Sin(angle) * deltaTime);
			}
			else
			{
				float angle = Mathf.Acos(q.w);
				gain = 2.0f * angle / (Mathf.Sin(angle) * deltaTime);
			}
			return new Vector3(q.x * gain, q.y * gain, q.z * gain);
		}

		public static Vector3 GetAngularVelocity (Quaternion fromRotation, Quaternion toRotation)
		{
			return GetAngularVelocity(fromRotation, toRotation, Time.deltaTime);
		}

		public static Vector3 GetVelocity (Vector3 fromPosition, Vector3 toPosition, float deltaTime)
		{
			return (toPosition - fromPosition) / deltaTime;
		}

		public static Vector3 GetVelocity (Vector3 fromPosition, Vector3 toPosition)
		{
			return GetVelocity(fromPosition, toPosition, Time.deltaTime);
		}
	}
}