using Scellecs.Morpeh;

namespace EcsEnemy
{
	public class EnemyRespawnSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private Filter _requests;
		private Stash<DelayedEnemySpawnRequest> _delayedRequestStash;
		private Stash<EnemySpawnRequest> _spawnRequestStash;
		private Stash<Position> _positionStash;

		public void OnAwake()
		{
			_requests = World.Filter.With<DelayedEnemySpawnRequest>().Build();
			_delayedRequestStash = World.GetStash<DelayedEnemySpawnRequest>();
			_spawnRequestStash = World.GetStash<EnemySpawnRequest>();
			_positionStash = World.GetStash<Position>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _requests)
			{
				ref DelayedEnemySpawnRequest request = ref _delayedRequestStash.Get(entity);
				if (World.IsDisposed(request.Target) || !_positionStash.Has(request.Target))
				{
					World.RemoveEntity(entity);
					continue;
				}

				request.Delay -= deltaTime;
				if (request.Delay > 0f)
					continue;

				_spawnRequestStash.Set(entity, new EnemySpawnRequest
				{
					Position = request.Position,
					Target = request.Target
				});

				_delayedRequestStash.Remove(entity);
			}
		}

		public void Dispose()
		{
		}
	}
}
