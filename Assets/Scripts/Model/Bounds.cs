﻿namespace Model
{
	public struct Bounds
	{
		public Bounds(float left, float right, float down, float up)
		{
			Left = left;
			Right = right;
			Down = down;
			Up = up;
		}

		public readonly float Up;
		public readonly float Down;
		public readonly float Left;
		public readonly float Right;
	}
}
