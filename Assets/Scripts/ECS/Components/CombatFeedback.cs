using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public struct AttackVisualRequest : IComponent
	{
		public Vector2 Center;
		public float Radius;
	}

	public struct DamageVisualRequest : IComponent
	{
		public Vector2 Position;
		public Entity Target;
		public bool Fatal;
	}
}
