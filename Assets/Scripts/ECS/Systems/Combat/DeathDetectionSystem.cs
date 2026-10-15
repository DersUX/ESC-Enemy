using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class DeathDetectionSystem : ISystem
	{

		public World World
		{
			get;
			set;
		}

		private readonly EnemySpawnConfig _config;

		private Filter _characters;
		private Stash<Health> _healthStash;
		private Stash<DeadTag> _deadStash;
		private Stash<DespawnRequest> _despawnRequestStash;
		private Stash<DelayedEnemySpawnRequest> _delayedSpawnRequestStash;
		private Stash<Position> _positionStash;
		private Stash<FollowTarget> _followTargetStash;

		public DeathDetectionSystem(EnemySpawnConfig config)
		{
			_config = config;
		}

		public void OnAwake()
		{
			_characters = World.Filter
				.With<Health>()
				.With<EnemyTag>()
				.Without<DeadTag>()
				.Build();

			_healthStash = World.GetStash<Health>();
			_deadStash = World.GetStash<DeadTag>();
			_despawnRequestStash = World.GetStash<DespawnRequest>();
			_delayedSpawnRequestStash = World.GetStash<DelayedEnemySpawnRequest>();
			_positionStash = World.GetStash<Position>();
			_followTargetStash = World.GetStash<FollowTarget>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _characters)
			{
				ref Health health = ref _healthStash.Get(entity);

				if (health.Value > 0)
					continue;

				_deadStash.Set(entity);

				if (_positionStash.Has(entity) && _followTargetStash.Has(entity))
				{
					Entity respawnEntity = World.CreateEntity();
					_delayedSpawnRequestStash.Set(respawnEntity, new DelayedEnemySpawnRequest
					{
						Delay = _config.RespawnDelay,
						Position = _positionStash.Get(entity).Value,
						Target = _followTargetStash.Get(entity).Target
					});
				}

				Entity despawnEntity = World.CreateEntity();

				_despawnRequestStash.Set(despawnEntity, new DespawnRequest
				{
					Target = entity
				});
			}
		}

		public void Dispose()
		{
		}
	}
}
