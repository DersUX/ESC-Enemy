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
		[SerializeField] private CharacterEyesView _playerEyes;
		[SerializeField] private EnemyPoolView _enemyPoolView;

		[Space] [SerializeField, Min(0f)] private float _moveSpeed = 4f;
		[SerializeField, Min(0f)] private float _enemySpeed = 2.5f;
		[SerializeField, Min(0f)] private float _enemyStopDistance = 0.9f;
		[SerializeField, Min(1)] private int _enemyCount = 20;
		[SerializeField, Min(0f)] private float _enemyRespawnDelay = 3f;

		[Space] [SerializeField] private Vector2 _movementBounds = new Vector2(7.5f, 4f);

		private EnemyPoolPresenter _enemyPool;

		public World World
		{
			get;
			private set;
		}

		private void Awake()
		{
			Utils.AssertIsNotNull(_playerView);
			Utils.AssertIsNotNull(_playerEyes);
			Utils.AssertIsNotNull(_enemyPoolView);
		}

		private void OnEnable()
		{
			_enemyPool = new EnemyPoolPresenter(_enemyPoolView);
			World = World.Create();
			World.UpdateByUnity = false;

			SystemsGroup systems = World.CreateSystemsGroup();
			systems.AddInitializer(new SceneInitializer(_playerView,
				_playerEyes,
				_moveSpeed,
				_enemyCount,
				_movementBounds));

			GameplaySystems.Register(systems,
				_enemyPool,
				_enemySpeed,
				_enemyStopDistance,
				_movementBounds,
				_enemyRespawnDelay);

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
