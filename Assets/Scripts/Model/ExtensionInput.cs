using UnityEngine;

namespace Model
{
	public static class ExtensionInput
	{
		private static readonly Bounds[] _localLeft;
		private static readonly Bounds[] _localRight;
		private static readonly Bounds[] _remoteLeft;
		private static readonly Bounds[] _remoteRight;

		static ExtensionInput()
		{
			_remoteLeft = new[]
			{
				new Bounds(0f, .25f, 0f, .5f),
				new Bounds(.5f, .75f, .5f, 1f),
			};
			_remoteRight = new[]
			{
				new Bounds(.25f, .5f, 0f, .5f),
				new Bounds(.75f, 1f, .5f, 1f),
			};
			_localLeft = new[]
			{
				new Bounds(0f, .25f, .5f, 1f),
				new Bounds(.5f, .75f, 0f, .5f),
			};
			_localRight = new[]
			{
				new Bounds(.25f, .5f, .5f, 1f),
				new Bounds(.75f, 1f, 0f, .5f),
			};
		}

		private static bool IsInZone(Vector2 position, Bounds[] zone)
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
				Input.touchCount > 0 && Input.GetTouch(0).IsRemoteLeft() ||
				Input.touchCount > 1 && Input.GetTouch(1).IsRemoteLeft();
		}

		public static bool IsRemoteRight()
		{
			return
				Input.touchCount > 0 && Input.GetTouch(0).IsRemoteRight() ||
				Input.touchCount > 1 && Input.GetTouch(1).IsRemoteRight();
		}

		public static bool IsLocalLeft()
		{
			return
				Input.touchCount > 0 && Input.GetTouch(0).IsLocalLeft() ||
				Input.touchCount > 1 && Input.GetTouch(1).IsLocalLeft();
		}

		public static bool IsLocalRight()
		{
			return
				Input.touchCount > 0 && Input.GetTouch(0).IsLocalRight() ||
				Input.touchCount > 1 && Input.GetTouch(1).IsLocalRight();
		}
	}
}
