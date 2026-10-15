using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/EnemyConfig")]
	public sealed class EnemyConfig : GameplayConfig
	{
		[SerializeField] private CharacterEyesView _prefab;
		public CharacterEyesView Prefab => _prefab;
		[SerializeField, Min(0f)] private float _moveSpeed = 1f;
		public float MoveSpeed => _moveSpeed;
		[SerializeField, Min(0.001f)] private float _radius = 0.4f;
		public float Radius => _radius;
		[SerializeField, Min(1)] private int _initialHealth = 100;
		public int InitialHealth => _initialHealth;
		[SerializeField, Min(0f)] private float _stopDistance = 0.9f;
		public float StopDistance => _stopDistance;

		public override void Validate()
		{
			Require(Prefab != null, "Prefab is required.");
			Require(Prefab != null && Prefab.TryGetComponent<CharacterDamageView>(out _), "Prefab must have CharacterDamageView.");
			Number(MoveSpeed, nameof(MoveSpeed));
			Number(Radius, nameof(Radius), true);
			Number(StopDistance, nameof(StopDistance));
			Require(InitialHealth > 0, "InitialHealth must be positive.");
		}
	}
}
