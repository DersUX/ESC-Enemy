using Presenters;
using ProjectUtils;
using Scellecs.Morpeh;
using UnityEngine;
using Views;

namespace EcsEnemy
{
	[DisallowMultipleComponent]
	public class MorpehBootstrap : MonoBehaviour
	{
		[SerializeField] private Transform _playerView;
		[SerializeField] private EnemyPoolView _enemyPoolView;

		[SerializeField] private GameConfig _config;

		private EnemyPoolPresenter _enemyPool;

		public World World
		{
			get;
			private set;
		}

		private void Awake()
		{
			Utils.AssertIsNotNull(_config);
			_config.Validate();
			Utils.AssertIsNotNull(_enemyPoolView);
		}

		private void OnEnable()
		{
			_config.Validate();
			_enemyPoolView.Configure(_config.Enemy, _config.DamageFeedback);
			_enemyPool = new EnemyPoolPresenter(_enemyPoolView);
			World = World.Create();
			World.UpdateByUnity = false;

			SystemsGroup systems = World.CreateSystemsGroup();
			systems.AddInitializer(new SceneInitializer(_playerView, _config));
			GameplaySystems.Register(systems, _enemyPool, _config);

			World.AddSystemsGroup(0, systems);

			World.Commit();
			World.Update(0f);
		}

		private void Update() => World?.Update(Time.deltaTime);

		private void FixedUpdate() => World?.FixedUpdate(Time.fixedDeltaTime);

		private void LateUpdate()
		{
			World?.LateUpdate(Time.deltaTime);
			World?.CleanupUpdate(Time.deltaTime);
		}

		private void OnDisable()
		{
			World?.Dispose();
			World = null;
			_enemyPool?.Dispose();
			_enemyPool = null;
		}
	}
}
