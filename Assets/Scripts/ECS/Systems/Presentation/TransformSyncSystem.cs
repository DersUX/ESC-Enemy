using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class TransformSyncSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private Filter _views;
		private Stash<Position> _positionStash;
		private Stash<TransformView> _transformStash;

		public void OnAwake()
		{
			_views = World.Filter.With<Position>().With<TransformView>().Build();
			_positionStash = World.GetStash<Position>();
			_transformStash = World.GetStash<TransformView>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _views)
			{
				Transform view = _transformStash.Get(entity).Value;

				if (view == null)
					continue;

				Vector2 position = _positionStash.Get(entity).Value;
				view.position = new Vector3(position.x, position.y, view.position.z);
			}
		}

		public void Dispose()
		{
		}
	}
}
