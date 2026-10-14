using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class MovementSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private Filter _movers;

		private Stash<InputMovementDirection> _inputMovementStash;
		private Stash<Position> _positionStash;
		private Stash<Movement> _movementStash;
		private Stash<Knockback> _knockbackStash;
		private Stash<CollisionRadius> _collisionRadiusStash;

		public void OnAwake()
		{
			_movers = World.Filter
				.With<InputMovementDirection>()
				.With<Position>()
				.With<Movement>()
				.Build();

			_inputMovementStash = World.GetStash<InputMovementDirection>();
			_positionStash = World.GetStash<Position>();
			_movementStash = World.GetStash<Movement>();
			_knockbackStash = World.GetStash<Knockback>();
			_collisionRadiusStash = World.GetStash<CollisionRadius>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _movers)
			{
				float movementTime = deltaTime;

				if (_knockbackStash.Has(entity))
					movementTime = Mathf.Max(0f, deltaTime - _knockbackStash.Get(entity).Remaining);

				ref Position position = ref _positionStash.Get(entity);
				ref Movement movement = ref _movementStash.Get(entity);

				position.Value += _inputMovementStash.Get(entity).Direction * movement.Speed * movementTime;
				float radius = _collisionRadiusStash.Has(entity) ? _collisionRadiusStash.Get(entity).Radius : 0f;

				position.Value = MovementBounds.Clamp(position.Value,
					MovementBounds.GetCenterLimits(movement.MapHalfSize, radius));
			}
		}

		public void Dispose()
		{
		}
	}
}
