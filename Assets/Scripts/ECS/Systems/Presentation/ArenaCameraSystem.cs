using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class ArenaCameraSystem : ISystem
	{
		private readonly ArenaConfig _config;

		private readonly Vector2 _arenaHalfSize;
		private readonly Camera _camera;
		private float _initialSize;

		public World World
		{
			get;
			set;
		}

		public ArenaCameraSystem(Camera camera, ArenaConfig config)
		{
			_camera = camera;
			_config = config;
			_arenaHalfSize = config.HalfSize + Vector2.one * config.CameraPadding;
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

			_camera.orthographicSize = Mathf.Max(_config.MinCameraSize, Mathf.Max(halfHeight, halfWidth / _camera.aspect));
		}

		public void Dispose()
		{
			if (_camera != null)
				_camera.orthographicSize = _initialSize;
		}
	}
}
