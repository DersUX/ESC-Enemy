using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class ApplyDamageSystem : ISystem
	{
		private readonly KnockbackConfig _config;

		public ApplyDamageSystem(KnockbackConfig config) => _config = config;

		public World World
		{
			get;
			set;
		}

		private Filter _damageRequests;

		private Stash<DamageRequest> _damageRequestStash;
		private Stash<Health> _healthStash;
		private Stash<Position> _positions;
		private Stash<DamageVisualRequest> _visuals;
		private Stash<Knockback> _knockbacks;

		public void OnAwake()
		{
			_damageRequests = World.Filter.With<DamageRequest>().Build();

			_damageRequestStash = World.GetStash<DamageRequest>();
			_healthStash = World.GetStash<Health>();
			_positions = World.GetStash<Position>();
			_visuals = World.GetStash<DamageVisualRequest>();
			_knockbacks = World.GetStash<Knockback>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _damageRequests)
			{
				ref DamageRequest damageRequest = ref _damageRequestStash.Get(entity);
				Entity target = damageRequest.Target;

				if (!World.IsDisposed(target) && _healthStash.Has(target))
				{
					ref Health health = ref _healthStash.Get(target);
					if (health.Value <= 0 || damageRequest.Amount <= 0)
					{
						World.RemoveEntity(entity);
						continue;
					}

					health.Value = Mathf.Max(0, health.Value - damageRequest.Amount);
					if (_positions.Has(target))
					{
						Entity visual = World.CreateEntity();
						_visuals.Set(visual, new DamageVisualRequest
						{
							Position = _positions.Get(target).Value,
							Target = target,
							Fatal = health.Value == 0
						});

						if (health.Value > 0 && damageRequest.Impulse.sqrMagnitude > 0f)
							_knockbacks.Set(target,
								new Knockback { Velocity = damageRequest.Impulse, Remaining = _config.Duration });
					}
				}

				World.RemoveEntity(entity);
			}
		}

		public void Dispose()
		{
		}
	}
}
