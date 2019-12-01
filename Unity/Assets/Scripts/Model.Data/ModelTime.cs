using UnityEngine;

namespace Model.Data
{
	public class ModelTime
	{
		public long AbsStartTime;
		public long AbsCorrection;
		public float SimStep => Time.deltaTime;
		public float SimAbs => Time.time;
	}
}
