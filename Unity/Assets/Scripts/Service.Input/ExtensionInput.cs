using Model.Data;
using UnityEngine;

namespace Service.Input
{
	public static class ExtensionInput
	{
		private static readonly ModelBounds[] _localLeft;
		private static readonly ModelBounds[] _localRight;
		private static readonly ModelBounds[] _remoteLeft;
		private static readonly ModelBounds[] _remoteRight;

		static ExtensionInput()
		{
			// zones setup
			_remoteLeft = new[]
			{
				new ModelBounds(0f, .25f, 0f, .5f),
				new ModelBounds(.5f, .75f, .5f, 1f),
			};
			_remoteRight = new[]
			{
				new ModelBounds(.25f, .5f, 0f, .5f),
				new ModelBounds(.75f, 1f, .5f, 1f),
			};
			_localLeft = new[]
			{
				new ModelBounds(0f, .25f, .5f, 1f),
				new ModelBounds(.5f, .75f, 0f, .5f),
			};
			_localRight = new[]
			{
				new ModelBounds(.25f, .5f, .5f, 1f),
				new ModelBounds(.75f, 1f, 0f, .5f),
			};
		}

		private static bool IsInZone(Vector2 position, ModelBounds[] zone)
		{
			var index = 0;
			for(; index < zone.Length; index++)
			{
				var resultL = zone[index].Left * Screen.width < position.x;
				var resultR = zone[index].Right * Screen.width > position.x;
				var resultD = zone[index].Down * Screen.height < position.y;
				var resultU = zone[index].Up * Screen.height > position.y;

				if(resultL && resultR && resultD && resultU)
				{
					break;
				}
			}
			return index < zone.Length;
		}

		public static bool IsRemoteLeft(this Touch touch)
		{
			return IsInZone(touch.position, _remoteLeft);
		}

		public static bool IsRemoteRight(this Touch touch)
		{
			return IsInZone(touch.position, _remoteRight);
		}

		public static bool IsLocalLeft(this Touch touch)
		{
			return IsInZone(touch.position, _localLeft);
		}

		public static bool IsLocalRight(this Touch touch)
		{
			return IsInZone(touch.position, _localRight);
		}

		public static bool IsRemoteLeft()
		{
			return
				UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).IsRemoteLeft() ||
				UnityEngine.Input.touchCount > 1 && UnityEngine.Input.GetTouch(1).IsRemoteLeft();
		}

		public static bool IsRemoteRight()
		{
			return
				UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).IsRemoteRight() ||
				UnityEngine.Input.touchCount > 1 && UnityEngine.Input.GetTouch(1).IsRemoteRight();
		}

		public static bool IsLocalLeft()
		{
			return
				UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).IsLocalLeft() ||
				UnityEngine.Input.touchCount > 1 && UnityEngine.Input.GetTouch(1).IsLocalLeft();
		}

		public static bool IsLocalRight()
		{
			return
				UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).IsLocalRight() ||
				UnityEngine.Input.touchCount > 1 && UnityEngine.Input.GetTouch(1).IsLocalRight();
		}

		public static bool IsGyroLeft()
		{
			return Vector3.Dot(GetGravity(), Vector3.down.normalized) > .1f;
		}

		public static bool IsGyroRight()
		{
			return Vector3.Dot(GetGravity(), Vector3.down.normalized) < -.1f;
		}

		public static Vector3 GetGravity()
		{
			return GetGyro() * Vector3.down;
		}

		public static Quaternion GetGyro()
		{
			var q = UnityEngine.Input.gyro.attitude;
			return new Quaternion(q.x, q.y, -q.z, -q.w);
		}
	}
}
