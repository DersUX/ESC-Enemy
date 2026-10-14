using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public struct EnemyTag : IComponent
	{
	}

	public struct EnemySpawnRequest : IComponent
	{
		public Vector2 Position;
		public Entity Target;
	}

	public struct FollowTarget : IComponent
	{
		public Entity Target;
		public float StopDistance;
	}

	public struct DelayedEnemySpawnRequest : IComponent
	{
		public float Delay;
		public Vector2 Position;
		public Entity Target;
	}
}
