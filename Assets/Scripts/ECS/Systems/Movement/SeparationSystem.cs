using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class SeparationSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private readonly SpatialGrid _grid;
		private readonly List<(Entity A, Entity B)> _pairs = new();

		private Filter _characters;
		private Stash<Position> _positionStash;
		private Stash<CollisionRadius> _collisionRadiusStash;
		private Stash<Movement> _movementStash;

		public SeparationSystem(SpatialGrid grid)
		{
			_grid = grid;
		}

		public void OnAwake()
		{
			_characters = World.Filter
				.With<Position>()
				.With<CollisionRadius>()
				.Build();

			_positionStash = World.GetStash<Position>();
			_collisionRadiusStash = World.GetStash<CollisionRadius>();
			_movementStash = World.GetStash<Movement>();
		}

		public void OnUpdate(float deltaTime)
		{
			CollectPairs();

			foreach ((Entity A, Entity B) pair in _pairs)
			{
				Separate(pair.A, pair.B);
			}
		}

		private void CollectPairs()
		{
			_pairs.Clear();

			foreach (Entity entity in _characters)
			{
				Vector2Int cell = _grid.GetCell(_positionStash.Get(entity).Value);

				for (int y = -1; y <= 1; y++)
				{
					for (int x = -1; x <= 1; x++)
					{
						Vector2Int neighborCell = cell + new Vector2Int(x, y);

						if (!_grid.TryGetCell(neighborCell, out List<Entity> neighbors))
							continue;

						foreach (Entity other in neighbors)
						{
							if (entity.Id >= other.Id)
								continue;

							_pairs.Add((entity, other));
						}
					}
				}
			}
		}

		private void Separate(Entity a, Entity b)
		{
			ref Position positionA = ref _positionStash.Get(a);
			ref Position positionB = ref _positionStash.Get(b);

			Vector2 offset = positionB.Value - positionA.Value;
			float minDistance = _collisionRadiusStash.Get(a).Radius + _collisionRadiusStash.Get(b).Radius;
			float distanceSquared = offset.sqrMagnitude;

			if (distanceSquared >= minDistance * minDistance)
				return;

			float distance = Mathf.Sqrt(distanceSquared);

			Vector2 normal = distance > 0f
				? offset / distance
				: Vector2.right;

			float overlap = minDistance - distance;
			Vector2 oldA = positionA.Value;
			Vector2 oldB = positionB.Value;
			positionA.Value = ClampToBounds(a, oldA - normal * (overlap * 0.5f));
			positionB.Value = ClampToBounds(b, oldB + normal * (overlap * 0.5f));

			float movedA = Vector2.Dot(oldA - positionA.Value, normal);
			float movedB = Vector2.Dot(positionB.Value - oldB, normal);
			float blockedA = Mathf.Max(0f, overlap * 0.5f - movedA);
			float blockedB = Mathf.Max(0f, overlap * 0.5f - movedB);
			positionA.Value = ClampToBounds(a, positionA.Value - normal * blockedB);
			positionB.Value = ClampToBounds(b, positionB.Value + normal * blockedA);
		}

		private Vector2 ClampToBounds(Entity entity, Vector2 position)
		{
			if (!_movementStash.Has(entity))
				return position;

			return MovementBounds.Clamp(position, MovementBounds.GetCenterLimits(
				_movementStash.Get(entity).MapHalfSize, _collisionRadiusStash.Get(entity).Radius));
		}

		public void Dispose()
		{
			_pairs.Clear();
		}
	}
}
