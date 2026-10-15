using Presenters;
using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public static class GameplaySystems
	{
		public static void Register(SystemsGroup systems, IEnemyPool pool, GameConfig config)
		{
			systems.AddSystem(new ArenaCameraSystem(Camera.main, config.Arena));
			systems.AddSystem(new EnemyRespawnSystem());
			systems.AddSystem(new EnemySpawnSystem(pool, config.Enemy, config.Arena, config.EyeAnimation));
			systems.AddSystem(new PlayerInputSystem(config.Input));
			systems.AddSystem(new EnemyFollowSystem());
			systems.AddSystem(new MovementSystem());
			systems.AddSystem(new CircleAttackSystem());
			systems.AddSystem(new CircleAttackHitSystem(config.Knockback));
			systems.AddSystem(new ApplyDamageSystem(config.Knockback));
			systems.AddSystem(new CombatPresentationSystem(config.CombatEffects));
			systems.AddSystem(new KnockbackSystem(config.Knockback));
			systems.AddSystem(new DeathDetectionSystem(config.EnemySpawn));
			systems.AddSystem(new EnemyDespawnSystem(pool));

			SpatialGrid grid = new SpatialGrid();

			for (int i = 0; i < config.Simulation.SeparationPassCount; i++)
			{
				systems.AddSystem(new SpatialGridBuildSystem(grid));
				systems.AddSystem(new SeparationSystem(grid));
			}

			systems.AddSystem(new TransformSyncSystem());
			systems.AddSystem(new EyeAnimationSystem(config.EyeAnimation));
		}
	}
}
