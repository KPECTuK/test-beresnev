using System.Runtime.InteropServices;
using UnityEngine;

namespace Model.Data
{
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
}
