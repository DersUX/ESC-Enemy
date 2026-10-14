using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public struct InputMovementDirection : IComponent
	{
		public Vector2 Direction;
	}

	public struct Position : IComponent
	{
		public Vector2 Value;
	}

	public struct Movement : IComponent
	{
		public float Speed;
		public Vector2 MapHalfSize;
	}

	public struct Knockback : IComponent
	{
		public Vector2 Velocity;
		public float Remaining;
	}

	public struct CollisionRadius : IComponent
	{
		public float Radius;
	}
}
