using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public struct CircleAttack : IComponent
	{
		public int Damage;
		public float Radius;
		public float Cooldown;
		public float CooldownRemaining;
	}

	public struct CircleAttackRequest : IComponent
	{
		public Vector2 Center;
		public float Radius;
		public int Damage;
	}

	public struct DamageRequest : IComponent
	{
		public Entity Target;
		public int Amount;
		public Vector2 Impulse;
	}
}
