using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/CircleAttackConfig")]
	public sealed class CircleAttackConfig : GameplayConfig
	{
		[SerializeField, Min(0)] private int _damage = 25;
		public int Damage => _damage;
		[SerializeField, Min(0f)] private float _radius = 2f;
		public float Radius => _radius;
		[SerializeField, Min(0f)] private float _cooldown = 0.25f;
		public float Cooldown => _cooldown;

		public override void Validate()
		{
			Require(Damage >= 0, "Damage must be non-negative.");
			Number(Radius, nameof(Radius));
			Number(Cooldown, nameof(Cooldown));
		}
	}
}
