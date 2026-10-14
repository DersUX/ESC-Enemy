using UnityEngine;

namespace EcsEnemy
{
	public static class MovementBounds
	{
		public static Vector2 GetCenterLimits(Vector2 mapHalfSize, float radius)
		{
			radius = Mathf.Max(0f, radius);
			return new Vector2(Mathf.Max(0f, mapHalfSize.x - radius),
				Mathf.Max(0f, mapHalfSize.y - radius));
		}

		public static Vector2 Clamp(Vector2 position, Vector2 limits)
		{
			return new Vector2(Mathf.Clamp(position.x, -limits.x, limits.x),
				Mathf.Clamp(position.y, -limits.y, limits.y));
		}
	}
}
