using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/EnemySpawnConfig")]
	public sealed class EnemySpawnConfig : GameplayConfig
	{
		[SerializeField, Min(1)] private int _count = 80;
		public int Count => _count;
		[SerializeField, Min(0.001f)] private float _spacing = 1f;
		public float Spacing => _spacing;
		[SerializeField, Min(0f)] private float _respawnDelay = 10f;
		public float RespawnDelay => _respawnDelay;

		public override void Validate()
		{
			Require(Count > 0, "Count must be positive.");
			Number(Spacing, nameof(Spacing), true);
			Number(RespawnDelay, nameof(RespawnDelay));
		}
	}
}
