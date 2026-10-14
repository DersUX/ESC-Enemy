using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class CircleAttackSystem : ISystem
	{
		public World World
		{
			get;
			set;
		}

		private Filter _attackers;

		private Stash<CircleAttack> _attackStash;
		private Stash<InputAttack> _inputStash;
		private Stash<Position> _positionStash;
		private Stash<CircleAttackRequest> _requestStash;
		private Stash<AttackVisualRequest> _visualStash;

		public void OnAwake()
		{
			_attackers = World.Filter
				.With<CircleAttack>()
				.With<InputAttack>()
				.With<Position>()
				.Build();

			_attackStash = World.GetStash<CircleAttack>();
			_inputStash = World.GetStash<InputAttack>();
			_positionStash = World.GetStash<Position>();
			_requestStash = World.GetStash<CircleAttackRequest>();
			_visualStash = World.GetStash<AttackVisualRequest>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _attackers)
			{
				ref CircleAttack attack = ref _attackStash.Get(entity);

				attack.CooldownRemaining = Mathf.Max(0f, attack.CooldownRemaining - deltaTime);

				if (!_inputStash.Get(entity).Pressed)
					continue;

				if (attack.CooldownRemaining > 0f)
					continue;

				Entity requestEntity = World.CreateEntity();

				_requestStash.Set(requestEntity, new CircleAttackRequest
				{
					Center = _positionStash.Get(entity).Value,
					Radius = attack.Radius,
					Damage = attack.Damage
				});

				attack.CooldownRemaining = attack.Cooldown;
				Entity visualEntity = World.CreateEntity();
				_visualStash.Set(visualEntity, new AttackVisualRequest
				{
					Center = _positionStash.Get(entity).Value,
					Radius = attack.Radius
				});
			}
		}

		public void Dispose()
		{
		}
	}
}
