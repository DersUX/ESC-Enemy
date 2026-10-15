using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/GameConfig")]
	public sealed class GameConfig : GameplayConfig
	{
		[SerializeField] private PlayerConfig _player;
		public PlayerConfig Player => _player;
		[SerializeField] private EnemyConfig _enemy;
		public EnemyConfig Enemy => _enemy;
		[SerializeField] private EnemySpawnConfig _enemySpawn;
		public EnemySpawnConfig EnemySpawn => _enemySpawn;
		[SerializeField] private ArenaConfig _arena;
		public ArenaConfig Arena => _arena;
		[SerializeField] private KnockbackConfig _knockback;
		public KnockbackConfig Knockback => _knockback;
		[SerializeField] private EyeAnimationConfig _eyeAnimation;
		public EyeAnimationConfig EyeAnimation => _eyeAnimation;
		[SerializeField] private DamageFeedbackConfig _damageFeedback;
		public DamageFeedbackConfig DamageFeedback => _damageFeedback;
		[SerializeField] private CombatEffectsConfig _combatEffects;
		public CombatEffectsConfig CombatEffects => _combatEffects;
		[SerializeField] private SimulationConfig _simulation;
		public SimulationConfig Simulation => _simulation;
		[SerializeField] private InputConfig _input;
		public InputConfig Input => _input;

		public override void Validate()
		{
			GameplayConfig[] configs = { Player, Enemy, EnemySpawn, Arena, Knockback, EyeAnimation, DamageFeedback, CombatEffects, Simulation, Input };
			foreach (GameplayConfig config in configs)
			{
				Require(config != null, "All config references are required.");
				config.Validate();
			}
			float maxRadius = Mathf.Max(Player.Radius, Enemy.Radius);
			Require(Arena.HalfSize.x >= maxRadius && Arena.HalfSize.y >= maxRadius, "Arena must fit both character radii.");
			Require(EnemySpawn.Spacing >= Enemy.Radius * 2f, "Spawn spacing must be at least the enemy diameter.");
		}
	}
}
