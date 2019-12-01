using System;
using UnityEngine;

namespace Model.Data
{
	[Serializable]
	public class ModelMeta
	{
		public Color BallColor;
		public float BallRadius;
		public string DataNameLocal;
		public ModelScore[] Scores;
	}
}
