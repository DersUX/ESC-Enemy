using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class CircleAttackHitSystem : ISystem
	{
		private const float MinImpulseDistanceSquared = 0.0001f;
		private const float KnockbackImpulse = 9f;

		public World World
		{
			get;
			set;
		}

		private Filter _attackRequests;
		private Filter _enemies;

		private Stash<CircleAttackRequest> _attackRequestStash;
		private Stash<Position> _positionStash;
		private Stash<CollisionRadius> _collisionRadiusStash;
		private Stash<DamageRequest> _damageRequestStash;

		public void OnAwake()
		{
			_attackRequests = World.Filter
				.With<CircleAttackRequest>()
				.Build();

			_enemies = World.Filter
				.With<EnemyTag>()
				.With<Position>()
				.With<CollisionRadius>()
				.With<Health>()
				.Build();

			_attackRequestStash = World.GetStash<CircleAttackRequest>();
			_positionStash = World.GetStash<Position>();
			_collisionRadiusStash = World.GetStash<CollisionRadius>();
			_damageRequestStash = World.GetStash<DamageRequest>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity requestEntity in _attackRequests)
			{
				CircleAttackRequest attack = _attackRequestStash.Get(requestEntity);

				foreach (Entity enemy in _enemies)
				{
					Vector2 enemyPosition = _positionStash.Get(enemy).Value;
					float reach = attack.Radius + _collisionRadiusStash.Get(enemy).Radius;

					if ((enemyPosition - attack.Center).sqrMagnitude > reach * reach)
						continue;

					Entity damageEntity = World.CreateEntity();

					_damageRequestStash.Set(damageEntity, new DamageRequest
					{
						Target = enemy,
						Amount = attack.Damage,
						Impulse = (enemyPosition - attack.Center).sqrMagnitude > MinImpulseDistanceSquared
							? (enemyPosition - attack.Center).normalized * KnockbackImpulse
							: Vector2.up * KnockbackImpulse
					});
				}

				World.RemoveEntity(requestEntity);
			}
		}

		public void Dispose()
		{
		}
	}
}
