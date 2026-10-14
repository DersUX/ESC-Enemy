using Presenters;
using Scellecs.Morpeh;

namespace EcsEnemy
{
	public class EnemyDespawnSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private Filter _enemies;
		private Stash<DespawnRequest> _despawnRequestStash;
		private Stash<EnemyPoolReference> _poolReferenceStash;
		private readonly IEnemyPool _pool;

		public EnemyDespawnSystem(IEnemyPool pool)
		{
			_pool = pool;
		}

		public void OnAwake()
		{
			_enemies = World.Filter.With<DespawnRequest>().Build();

			_despawnRequestStash = World.GetStash<DespawnRequest>();
			_poolReferenceStash = World.GetStash<EnemyPoolReference>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _enemies)
			{
				Entity target = _despawnRequestStash.Get(entity).Target;

				if (!World.IsDisposed(target) && _poolReferenceStash.Has(target))
				{
					CharacterEyesView view = _poolReferenceStash.Get(target).View;
					if (view != null)
						_pool.Return(view);

					World.RemoveEntity(target);
				}

				World.RemoveEntity(entity);
			}
		}

		public void Dispose()
		{
		}
	}
}
