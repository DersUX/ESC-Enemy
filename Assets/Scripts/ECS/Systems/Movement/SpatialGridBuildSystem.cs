using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class SpatialGridBuildSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private readonly SpatialGrid _grid;

		private Filter _characters;
		private Stash<Position> _positionsStash;
		private Stash<CollisionRadius> _collisionRadiusStash;
		private Stash<Movement> _movementStash;

		public SpatialGridBuildSystem(SpatialGrid grid)
		{
			_grid = grid;
		}

		public void OnAwake()
		{
			_characters = World.Filter
				.With<Position>()
				.With<CollisionRadius>()
				.Build();

			_positionsStash = World.GetStash<Position>();
			_collisionRadiusStash = World.GetStash<CollisionRadius>();
			_movementStash = World.GetStash<Movement>();
		}

		public void OnUpdate(float deltaTime)
		{
			float maxRadius = 0f;

			foreach (Entity entity in _characters)
			{
				maxRadius = Mathf.Max(maxRadius, _collisionRadiusStash.Get(entity).Radius);
			}

			_grid.Clear(maxRadius * 2f);

			foreach (Entity entity in _characters)
			{
				ref Position position = ref _positionsStash.Get(entity);

				if (_movementStash.Has(entity))
					position.Value = MovementBounds.Clamp(position.Value,
						MovementBounds.GetCenterLimits(_movementStash.Get(entity).MapHalfSize, _collisionRadiusStash.Get(entity).Radius));

				_grid.Add(entity, position.Value);
			}
		}

		public void Dispose()
		{
		}
	}
}
