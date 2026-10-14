using Presenters;
using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class EnemySpawnSystem : ISystem
	{
		private const float EnemyRadius = 0.4f;
		private const int InitialEnemyHealth = 100;

		public World World
		{
			get;
			set;
		}

		private readonly IEnemyPool _pool;
		private readonly float _speed;
		private readonly float _stopDistance;
		private readonly Vector2 _mapHalfSize;
		private Filter _requests;
		private Stash<EnemySpawnRequest> _requestStash;

		public EnemySpawnSystem(IEnemyPool pool, float speed, float stopDistance, Vector2 movementBounds)
		{
			_pool = pool;
			_speed = speed;
			_stopDistance = stopDistance;
			_mapHalfSize = new Vector2(Mathf.Abs(movementBounds.x), Mathf.Abs(movementBounds.y));
		}

		public void OnAwake()
		{
			_requests = World.Filter.With<EnemySpawnRequest>().Build();
			_requestStash = World.GetStash<EnemySpawnRequest>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity requestEntity in _requests)
			{
				EnemySpawnRequest request = _requestStash.Get(requestEntity);
				if (!World.IsDisposed(request.Target) && World.GetStash<Position>().Has(request.Target))
					Spawn(request);

				World.RemoveEntity(requestEntity);
			}
		}

		private void Spawn(EnemySpawnRequest request)
		{
			CharacterEyesView view = _pool.Rent();
			Entity enemy = default;
			bool entityCreated = false;

			try
			{
				Vector2 spawnPosition = MovementBounds.Clamp(request.Position,
					MovementBounds.GetCenterLimits(_mapHalfSize, EnemyRadius));

				var position = new Vector3(spawnPosition.x, spawnPosition.y, view.transform.position.z);
				_pool.Prepare(view, position);

				if (!view.TryGetComponent<CharacterDamageView>(out _))
					throw new System.InvalidOperationException("Enemy prefab must have a configured CharacterDamageView.");

				enemy = World.CreateEntity();
				entityCreated = true;

				World.GetStash<EnemyTag>().Set(enemy);
				World.GetStash<EnemyPoolReference>().Set(enemy, new EnemyPoolReference { View = view });
				World.GetStash<InputMovementDirection>().Set(enemy);
				World.GetStash<Position>().Set(enemy, new Position { Value = spawnPosition });
				World.GetStash<CollisionRadius>().Set(enemy, new CollisionRadius { Radius = EnemyRadius });
				World.GetStash<Movement>().Set(enemy, new Movement { Speed = _speed, MapHalfSize = _mapHalfSize });
				World.GetStash<TransformView>().Set(enemy, new TransformView { Value = view.transform });
				World.GetStash<EyeAnimation>().Set(enemy, view.CreateAnimation(spawnPosition));

				World.GetStash<FollowTarget>().Set(enemy, new FollowTarget
				{
					Target = request.Target,
					StopDistance = _stopDistance
				});

				World.GetStash<Health>().Set(enemy, new Health { Value = InitialEnemyHealth });
				_pool.Activate(view);
			}
			catch
			{
				if (entityCreated)
					World.RemoveEntity(enemy);

				_pool.Return(view);
				throw;
			}
		}

		public void Dispose()
		{
		}
	}
}
