using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class ArenaCameraSystem : ISystem
	{
		private const float ArenaPadding = 0.5f;

		private readonly Vector2 _arenaHalfSize;
		private readonly Camera _camera;
		private float _initialSize;

		public World World
		{
			get;
			set;
		}

		public ArenaCameraSystem(Camera camera, Vector2 movementBounds)
		{
			_camera = camera;
			_arenaHalfSize = new Vector2(Mathf.Abs(movementBounds.x), Mathf.Abs(movementBounds.y)) +
			                 Vector2.one * ArenaPadding;
		}

		public void OnAwake()
		{
			if (_camera != null)
				_initialSize = _camera.orthographicSize;
		}

		public void OnUpdate(float deltaTime)
		{
			if (_camera == null || !_camera.orthographic || _camera.aspect <= 0f)
				return;

			Vector3 position = _camera.transform.position;
			float halfWidth = _arenaHalfSize.x + Mathf.Abs(position.x);
			float halfHeight = _arenaHalfSize.y + Mathf.Abs(position.y);

			_camera.orthographicSize = Mathf.Max(_initialSize, Mathf.Max(halfHeight, halfWidth / _camera.aspect));
		}

		public void Dispose()
		{
			if (_camera != null)
				_camera.orthographicSize = _initialSize;
		}
	}
}
