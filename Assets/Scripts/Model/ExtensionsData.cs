using System;
using System.Linq;
using UnityEngine;

namespace Model
{
	public static class ExtensionsData
	{
		private const float DIM = 1.0f;
		private const float AXIS_GAP = 0.7f;
		private const float DEFAULT_SIZE = .1f;

		public static void GenerateMatch(this Repository target)
		{
			target.DataBall = new ModelBall
			{
				Color = Color.white,
				Position = Vector2.zero,
				Radius = .03f,
				Speed = UnityEngine.Random.insideUnitCircle.normalized,
			};

			target.DataReflectorLocal = new ModelReflector
			{
				Position = Vector2.zero,
				HalfSize = .1f,
				Shape = new[] { new ModelSegment(new Vector2(-1f, -1f), new Vector2(1f, -1f)), }
			};

			target.DataReflectorRemote = new ModelReflector
			{
				Position = Vector2.zero,
				HalfSize = .1f,
				Shape = new[] { new ModelSegment(new Vector2(-1f, 1f), new Vector2(1f, 1f)), }
			};

			var halfDistance = 1f;
			target.DataCourt = new ModelCourt
			{
				HalfDistance = halfDistance,
				LeftSide = new[] { new ModelSegment(new Vector2(-1f, halfDistance), new Vector2(-1f, -halfDistance)) },
				RightSide = new[] { new ModelSegment(new Vector2(1f, halfDistance), new Vector2(1f, -halfDistance)) },
				RemoteGoal = new[] { new ModelSegment(new Vector2(-1f, halfDistance), new Vector2(1f, halfDistance)) },
				LocalGoal = new[] { new ModelSegment(new Vector2(-1f, -halfDistance), new Vector2(1f, -halfDistance)) },
			};

			target.DataTime = new ModelTime();
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

		public static bool Intersect(ModelSegment s0, ModelSegment s1, out Vector2 result)
		{
			// simplified

			const float EPSILON_POS_F = 0.000000001f;
			const float EPSILON_NEG_F = -0.000000001f;

			var denominator = (s1.V1.y - s1.V0.y) * (s0.V1.x - s0.V0.x) - (s1.V1.x - s1.V0.x) * (s0.V1.y - s0.V0.y);
			var uA = (s1.V1.x - s1.V0.x) * (s0.V0.y - s1.V0.y) - (s1.V1.y - s1.V0.y) * (s0.V0.x - s1.V0.x);
			var uB = (s0.V1.x - s0.V0.x) * (s0.V0.y - s1.V0.y) - (s0.V1.y - s0.V0.y) * (s0.V0.x - s1.V0.x);

			if(denominator < EPSILON_NEG_F || denominator > EPSILON_POS_F)
			{
				uA /= denominator;
				uB /= denominator;

				if(uA > 0f + EPSILON_POS_F && uA <= 1f - EPSILON_POS_F && uB > 0f + EPSILON_POS_F && uB <= 1f - EPSILON_POS_F)
				{
					// intersects
					result = new Vector2(
						s0.V0.x + uA * (s0.V1.x - s0.V0.x),
						s0.V0.y + uA * (s0.V1.y - s0.V0.y));
					return true;
				}
			}
			else
			{
				var aCheck = uA < EPSILON_POS_F && uA > EPSILON_NEG_F;
				var bCheck = uB < EPSILON_POS_F && uB > EPSILON_NEG_F;
				if(aCheck && bCheck)
				{
					// coincident or zero
					result = (s0.V0 + s0.V1) * .5f;
					return true;
				}
			}

			result = Vector2.zero;
			return false;
		}

		public static Vector2 Normal(ModelSegment s, Vector2 side)
		{
			var deltaX = s.V1.x - s.V0.x;
			var deltaY = s.V1.y - s.V0.y;
			var test = new Vector2(-deltaY, deltaX).normalized;
			return
				Vector2.Dot((side - s.V0).normalized, test) < 0
					? new Vector2(deltaY, -deltaX).normalized
					: test;
		}

		public static unsafe bool TestReflect(this Repository repository, ModelSegment* segments, int size)
		{
			var increment = repository.DataBall.Speed * repository.DataTime.SimStep;
			var predict = new ModelSegment(
				repository.DataBall.Position,
				repository.DataBall.Position + increment);

			var index = 0;
			var intersection = Vector2.zero;
			for(; index < size; index++)
			{
				if(Intersect(predict, segments[index], out intersection))
				{
					break;
				}
			}

			if(index == size)
			{
				return false;
			}

			// might be optimized probably, causes: speed, error
			var normal = Normal(segments[index], predict.V0);
			var distance = (intersection - predict.V0).magnitude;
			var dif = increment.magnitude - distance;
			var reflect = increment - 2f * Vector2.Dot(increment, normal) * normal;
			var bounce = reflect.normalized * dif;

			repository.DataBall.Speed = reflect.normalized;
			repository.DataBall.Position = intersection + bounce;

			return true;
		}

		public static void Limit(this Repository repository, ModelReflector reflector)
		{
			reflector.Position =
				reflector.Position.x - reflector.HalfSize < -1f
					? new Vector2(reflector.HalfSize - 1f, reflector.Position.y)
					: reflector.Position;
			reflector.Position =
				reflector.Position.x + reflector.HalfSize > 1f
					? new Vector2(1f - reflector.HalfSize, reflector.Position.y)
					: reflector.Position;
		}

		public static bool Limit(this Repository repository, ModelBall ball)
		{
			throw new NotImplementedException();
		}

		public static float TotalWidth(this ModelCourt source)
		{
			var left = source.LeftSide.Max(_ => _.V0.x > _.V1.x ? _.V1.x : _.V0.x);
			var right = source.RightSide.Max(_ => _.V0.x > _.V1.x ? _.V0.x : _.V1.x);
			return Mathf.Abs(left) + Mathf.Abs(right);
		}

		public static float TotalHeight(this ModelCourt source)
		{
			return source.HalfDistance * 2f;
		}

		public static void DebugDrawCross(this Vector3 position, Quaternion rotation, Color color, float size = DEFAULT_SIZE, float duration = 0f)
		{
			Debug.DrawLine(position + rotation * Vector3.up * size * AXIS_GAP, position + rotation * Vector3.up * size, Color.green * DIM, duration);
			Debug.DrawLine(position, position + rotation * Vector3.up * size * AXIS_GAP, color * DIM, duration);
			Debug.DrawLine(position, position - rotation * Vector3.up * size, color * DIM, duration);

			Debug.DrawLine(position + rotation * Vector3.right * size * AXIS_GAP, position + rotation * Vector3.right * size, Color.red * DIM, duration);
			Debug.DrawLine(position, position + rotation * Vector3.right * size * AXIS_GAP, color * DIM, duration);
			Debug.DrawLine(position, position - rotation * Vector3.right * size, color * DIM, duration);

			Debug.DrawLine(position + rotation * Vector3.forward * size * AXIS_GAP, position + rotation * Vector3.forward * size, Color.blue * DIM, duration);
			Debug.DrawLine(position, position + rotation * Vector3.forward * size * AXIS_GAP, color * DIM, duration);
			Debug.DrawLine(position, position - rotation * Vector3.forward * size, color * DIM, duration);
		}
	}
}
