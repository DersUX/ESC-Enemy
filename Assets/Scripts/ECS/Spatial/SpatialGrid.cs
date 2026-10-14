using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class SpatialGrid
	{
		private const float DefaultCellSize = 0.8f;
		private const float MinCellSize = 0.01f;

		private readonly Dictionary<Vector2Int, List<Entity>> _cells = new();
		private readonly Stack<List<Entity>> _pool = new();

		public float CellSize { get; private set; } = DefaultCellSize;

		public void Clear(float cellSize)
		{
			foreach (List<Entity> entities in _cells.Values)
			{
				entities.Clear();
				_pool.Push(entities);
			}

			_cells.Clear();
			CellSize = Mathf.Max(MinCellSize, cellSize);
		}

		public Vector2Int GetCell(Vector2 position)
		{
			return new Vector2Int(
				Mathf.FloorToInt(position.x / CellSize),
				Mathf.FloorToInt(position.y / CellSize)
			);
		}

		public void Add(Entity entity, Vector2 position)
		{
			Vector2Int cell = GetCell(position);

			if (!_cells.TryGetValue(cell, out List<Entity> entities))
			{
				entities = _pool.Count > 0
					? _pool.Pop()
					: new List<Entity>();

				_cells.Add(cell, entities);
			}

			entities.Add(entity);
		}

		public bool TryGetCell(Vector2Int cell, out List<Entity> entities)
		{
			return _cells.TryGetValue(cell, out entities);
		}
	}
}
