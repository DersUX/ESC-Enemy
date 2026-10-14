using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class SceneInitializer : IInitializer
	{
		private const float CharacterRadius = 0.4f;
		private const float SpawnSpacing = 1f;
		private const float PlayerAttackRadius = 2f;
		private const float PlayerAttackCooldown = 0.5f;
		private const float InitialAttackCooldownRemaining = 0f;
		private const int PlayerAttackDamage = 25;

		private readonly Transform _playerView;

		private readonly CharacterEyesView _playerEyes;

		private readonly float _moveSpeed;
		private readonly int _enemyCount;

		private readonly List<Transform> _spawnedViews = new();
		private readonly Vector2 _mapHalfSize;

		public World World
		{
			get;
			set;
		}

		public SceneInitializer(Transform playerView,
			CharacterEyesView playerEyes,
			float moveSpeed,
			int enemyCount,
			Vector2 movementBounds)
		{
			_playerView = playerView;
			_playerEyes = playerEyes;
			_moveSpeed = moveSpeed;
			_enemyCount = Mathf.Max(1, enemyCount);
			_mapHalfSize = new Vector2(Mathf.Abs(movementBounds.x), Mathf.Abs(movementBounds.y));
		}

		public void OnAwake()
		{
			List<Vector2> spawnPositions = GetEnemySpawnPositions(_playerView.position);
			Transform playerView = GetOrCreateSceneView(_playerView);
			CharacterEyesView playerEyes = playerView == _playerView
				? _playerEyes
				: playerView.GetComponent<CharacterEyesView>();

			Entity player = CreateCharacter(playerView, playerEyes, _moveSpeed);
			World.GetStash<PlayerTag>().Set(player);
			World.GetStash<InputAttack>().Set(player);

			World.GetStash<CircleAttack>().Set(player, new CircleAttack
			{
				Radius = PlayerAttackRadius,
				Damage = PlayerAttackDamage,
				Cooldown = PlayerAttackCooldown,
				CooldownRemaining = InitialAttackCooldownRemaining
			});

			CreateEnemySpawnRequests(player, spawnPositions);
		}

		private Transform GetOrCreateSceneView(Transform source)
		{
			if (source.gameObject.scene.IsValid())
				return source;

			Transform view = Object.Instantiate(source, source.position, source.rotation);
			view.name = source.name;
			_spawnedViews.Add(view);
			return view;
		}

		private List<Vector2> GetEnemySpawnPositions(Vector2 playerPosition)
		{
			Vector2 bounds = _mapHalfSize - Vector2.one * CharacterRadius;

			if (bounds.x < 0f || bounds.y < 0f)
				throw new System.InvalidOperationException("The arena is too small to fit a character.");

			int columns = Mathf.FloorToInt(bounds.x * 2f / SpawnSpacing) + 1;
			int rows = Mathf.FloorToInt(bounds.y * 2f / SpawnSpacing) + 1;
			var positions = new List<Vector2>();

			playerPosition = MovementBounds.Clamp(playerPosition, bounds);

			float minDistance = CharacterRadius * 2f;

			for (int row = 0; row < rows && positions.Count < _enemyCount; row++)
			{
				for (int column = 0; column < columns && positions.Count < _enemyCount; column++)
				{
					var position = new Vector2(
						-bounds.x + column * SpawnSpacing,
						-bounds.y + row * SpawnSpacing);

					if ((position - playerPosition).sqrMagnitude < minDistance * minDistance)
						continue;

					positions.Add(position);
				}
			}

			if (positions.Count < _enemyCount)
				throw new System.InvalidOperationException(
					$"Requested {_enemyCount} enemies, but the spawn grid only fits {positions.Count} " +
					"without overlapping the player. Reduce Enemy Count or increase Movement Bounds.");

			return positions;
		}

		private void CreateEnemySpawnRequests(Entity player, List<Vector2> positions)
		{
			foreach (Vector2 position in positions)
			{
				Entity request = World.CreateEntity();
				World.GetStash<EnemySpawnRequest>().Set(request, new EnemySpawnRequest
				{
					Position = position,
					Target = player,
				});
			}
		}

		private Entity CreateCharacter(Transform view, CharacterEyesView eyes, float speed)
		{
			Entity entity = World.CreateEntity();
			World.GetStash<InputMovementDirection>().Set(entity);

			Vector2 position = MovementBounds.Clamp(view.position,
				MovementBounds.GetCenterLimits(_mapHalfSize, CharacterRadius));
			World.GetStash<Position>().Set(entity, new Position { Value = position });

			World.GetStash<CollisionRadius>().Set(entity, new CollisionRadius { Radius = CharacterRadius });

			World.GetStash<Movement>().Set(entity, new Movement
			{
				Speed = speed,
				MapHalfSize = _mapHalfSize
			});

			World.GetStash<TransformView>().Set(entity, new TransformView { Value = view });
			World.GetStash<EyeAnimation>().Set(entity, eyes.CreateAnimation(position));

			return entity;
		}

		public void Dispose()
		{
			foreach (Transform view in _spawnedViews)
			{
				if (view == null)
					continue;

				view.gameObject.SetActive(false);
				Object.Destroy(view.gameObject);
			}

			_spawnedViews.Clear();
		}
	}
}
