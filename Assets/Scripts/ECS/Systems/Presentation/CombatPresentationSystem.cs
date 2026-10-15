using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class CombatPresentationSystem : ISystem
	{
		private readonly CombatEffectsConfig _config;

		public CombatPresentationSystem(CombatEffectsConfig config) => _config = config;

		public World World
		{
			get;
			set;
		}

		private Filter _attacks;
		private Filter _hits;
		private Stash<AttackVisualRequest> _attackStash;
		private Stash<DamageVisualRequest> _hitStash;
		private Stash<TransformView> _transformStash;
		private CombatEffects _effects;

		public void OnAwake()
		{
			_attacks = World.Filter.With<AttackVisualRequest>().Build();
			_hits = World.Filter.With<DamageVisualRequest>().Build();
			_attackStash = World.GetStash<AttackVisualRequest>();
			_hitStash = World.GetStash<DamageVisualRequest>();
			_transformStash = World.GetStash<TransformView>();
			_effects = new CombatEffects(_config);
		}

		public void OnUpdate(float deltaTime)
		{
			_effects.Update();

			foreach (Entity entity in _attacks)
			{
				AttackVisualRequest attack = _attackStash.Get(entity);
				_effects.Play(attack.Center, attack.Radius, CombatEffectKind.Attack);
				World.RemoveEntity(entity);
			}

			foreach (Entity entity in _hits)
			{
				DamageVisualRequest hit = _hitStash.Get(entity);

				if (!World.IsDisposed(hit.Target) && _transformStash.Has(hit.Target))
				{
					Transform view = _transformStash.Get(hit.Target).Value;

					if (view != null && view.TryGetComponent(out CharacterDamageView damageView) && damageView.isActiveAndEnabled)
						damageView.PlayHit();
				}

				_effects.Play(hit.Position, hit.Fatal ? _config.DeathRadius : _config.HitRadius,
					hit.Fatal ? CombatEffectKind.Death : CombatEffectKind.Hit);
				World.RemoveEntity(entity);
			}
		}

		public void Dispose() => _effects?.Dispose();
	}
}
