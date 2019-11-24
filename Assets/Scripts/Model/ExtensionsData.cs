using UnityEngine;

namespace Model
{
	public static class ExtensionsData
	{
		public static void GenerateMatch(this Repository target)
		{
			target.DataBall = new[]
			{
				new ModelBall
				{
					Color = Color.white,
					Position = Vector2.zero,
					Radius = 1f,
					Speed = Random.insideUnitCircle,
				}
			};

			target.DataBatLocal = new ModelBat
			{
				Speed = Vector2.zero,
				Position = Vector2.zero,
				Shape = new[]
				{
					new ModelBatSegment
					{
						Start = Vector2.left,
						End = Vector2.right,
					}
				}
			};

			target.DataBatRemote = new ModelBat
			{
				Speed = Vector2.zero,
				Position = Vector2.zero,
				Shape = new[]
				{
					new ModelBatSegment
					{
						Start = Vector2.left,
						End = Vector2.right,
					}
				}
			};
		}

		public static void GenerateMeta(this Repository target)
		{
			target.DataMetaLocal = new ModelMeta
			{
				DataNameLocal = "local",
				Scores = new[]
				{
					new ModelScore
					{
						Name = "local",
						Value = 0,
					}
				}
			};
		}
	}
}
