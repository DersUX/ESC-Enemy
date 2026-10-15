using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class KnockbackSystem : ISystem
	{
		private readonly KnockbackConfig _config;

		public KnockbackSystem(KnockbackConfig config) => _config = config;

		public World World
		{
			get;
			set;
		}

		private Filter _targets;
		private Stash<Knockback> _knockbacks;
		private Stash<Position> _positions;
		private Stash<Movement> _movements;
		private Stash<CollisionRadius> _collisionRadiusStash;

		public void OnAwake()
		{
			_targets = World.Filter.With<Knockback>().With<Position>().With<Movement>().Build();
			_knockbacks = World.GetStash<Knockback>();
			_positions = World.GetStash<Position>();
			_movements = World.GetStash<Movement>();
			_collisionRadiusStash = World.GetStash<CollisionRadius>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _targets)
			{
				ref Knockback hit = ref _knockbacks.Get(entity);
				ref Position position = ref _positions.Get(entity);

				float step = Mathf.Min(deltaTime, hit.Remaining);
				float decay = Mathf.Exp(-_config.DecayRate * step);

				position.Value += hit.Velocity * ((1f - decay) / _config.DecayRate);
				hit.Velocity *= decay;
				hit.Remaining -= step;

				float radius = _collisionRadiusStash.Has(entity) ? _collisionRadiusStash.Get(entity).Radius : 0f;
				position.Value = MovementBounds.Clamp(position.Value,
					MovementBounds.GetCenterLimits(_movements.Get(entity).MapHalfSize, radius));

				if (hit.Remaining <= 0f)
					_knockbacks.Remove(entity);
			}
		}

		public void Dispose()
		{
		}
	}
}
