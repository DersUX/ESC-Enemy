using Presenters;
using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public static class GameplaySystems
	{
		private const float DefaultEnemyRespawnDelay = 3f;
		private const int SeparationPassCount = 2;

		public static void Register(
			SystemsGroup systems,
			IEnemyPool pool,
			float enemySpeed,
			float enemyStopDistance,
			Vector2 movementBounds,
			float enemyRespawnDelay = DefaultEnemyRespawnDelay)
		{
			systems.AddSystem(new ArenaCameraSystem(Camera.main, movementBounds));
			systems.AddSystem(new EnemyRespawnSystem());
			systems.AddSystem(new EnemySpawnSystem(pool, enemySpeed, enemyStopDistance, movementBounds));
			systems.AddSystem(new PlayerInputSystem());
			systems.AddSystem(new EnemyFollowSystem());
			systems.AddSystem(new MovementSystem());
			systems.AddSystem(new CircleAttackSystem());
			systems.AddSystem(new CircleAttackHitSystem());
			systems.AddSystem(new ApplyDamageSystem());
			systems.AddSystem(new CombatPresentationSystem());
			systems.AddSystem(new KnockbackSystem());
			systems.AddSystem(new DeathDetectionSystem(enemyRespawnDelay));
			systems.AddSystem(new EnemyDespawnSystem(pool));

			SpatialGrid grid = new SpatialGrid();

			for (int i = 0; i < SeparationPassCount; i++)
			{
				systems.AddSystem(new SpatialGridBuildSystem(grid));
				systems.AddSystem(new SeparationSystem(grid));
			}

			systems.AddSystem(new TransformSyncSystem());
			systems.AddSystem(new EyeAnimationSystem());
		}
	}
}
