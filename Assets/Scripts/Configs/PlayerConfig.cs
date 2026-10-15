using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/PlayerConfig")]
	public sealed class PlayerConfig : GameplayConfig
	{
		[SerializeField] private CharacterEyesView _prefab;
		public CharacterEyesView Prefab => _prefab;
		[SerializeField, Min(0f)] private float _moveSpeed = 4f;
		public float MoveSpeed => _moveSpeed;
		[SerializeField, Min(0.001f)] private float _radius = 0.4f;
		public float Radius => _radius;
		[SerializeField] private CircleAttackConfig _attack;
		public CircleAttackConfig Attack => _attack;

		public override void Validate()
		{
			Require(Prefab != null, "Prefab is required.");
			Require(Attack != null, "Attack is required.");
			Number(MoveSpeed, nameof(MoveSpeed));
			Number(Radius, nameof(Radius), true);
			Attack.Validate();
		}
	}
}
