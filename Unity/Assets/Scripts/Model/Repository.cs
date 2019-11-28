using System.Runtime.InteropServices;
using UnityEngine;

namespace Model
{
	public class ModelBall
	{
		public Color Color;
		public float Radius;
		public Vector2 Position;
		public Vector2 Speed;
	}

	public class ModelReflector
	{
		public Vector2 Position;
		public float HalfSize;
		public ModelSegment[] Shape;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ModelSegment
	{
		public Vector2 V0;
		public Vector2 V1;

		public ModelSegment(Vector2 v0, Vector2 v1)
		{
			V0 = v0;
			V1 = v1;
		}
	}

	public class ModelCourt
	{
		public float HalfDistance;
		public ModelSegment[] LeftSide;
		public ModelSegment[] RightSide;
		public ModelSegment[] RemoteGoal;
		public ModelSegment[] LocalGoal;
	}

	public class ModelTime
	{
		public float SimStep => Time.deltaTime;
		public float SimAbs => Time.time;
	}

	public class Repository
	{
		// match
		public ModelBall DataBall;
		public ModelReflector DataReflectorLocal;
		public ModelReflector DataReflectorRemote;
		public ModelCourt DataCourt;
		public ModelTime DataTime;

		// meta
		public ModelMeta DataMetaLocal;
	}
}
