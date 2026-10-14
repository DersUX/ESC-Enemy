using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class EnemyFollowSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private Filter _enemies;

		private Stash<FollowTarget> _followTargetStash;
		private Stash<Position> _positionStash;
		private Stash<InputMovementDirection> _inputMovementDirectionStash;
		private Stash<Movement> _movementStash;

		public void OnAwake()
		{
			_enemies = World.Filter
				.With<EnemyTag>()
				.With<FollowTarget>()
				.With<Position>()
				.With<InputMovementDirection>()
				.With<Movement>().Build();

			_followTargetStash = World.GetStash<FollowTarget>();
			_positionStash = World.GetStash<Position>();
			_inputMovementDirectionStash = World.GetStash<InputMovementDirection>();
			_movementStash = World.GetStash<Movement>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _enemies)
			{
				ref FollowTarget follow = ref _followTargetStash.Get(entity);
				ref InputMovementDirection input = ref _inputMovementDirectionStash.Get(entity);
				input.Direction = Vector2.zero;

				if (World.IsDisposed(follow.Target) || !_positionStash.Has(follow.Target))
					continue;

				Vector2 offset = _positionStash.Get(follow.Target).Value - _positionStash.Get(entity).Value;
				float distance = offset.magnitude;
				float remaining = distance - Mathf.Max(0f, follow.StopDistance);
				float step = _movementStash.Get(entity).Speed * deltaTime;

				if (remaining <= 0f || step <= 0f || distance <= 0f)
					continue;

				input.Direction = offset / distance * Mathf.Min(1f, remaining / step);
			}
		}

		public void Dispose()
		{
		}
	}
}
