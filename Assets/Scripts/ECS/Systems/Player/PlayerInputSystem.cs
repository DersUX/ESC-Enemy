using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EcsEnemy
{
	public class PlayerInputSystem : ISystem
	{
		private readonly InputConfig _config;
		private InputActionAsset _actions;
		private InputAction _move;
		private InputAction _attack;

		public PlayerInputSystem(InputConfig config) => _config = config;

		public World World
		{
			get;
			set;
		}

		private Filter _players;
		private Stash<InputMovementDirection> _inputMovementStash;
		private Stash<InputAttack> _inputAttackStash;

		public void OnAwake()
		{
			_config.Validate();
			_actions = Object.Instantiate(_config.Actions);
			_move = _actions.FindAction(InputConfig.MoveAction, true);
			_attack = _actions.FindAction(InputConfig.AttackAction, true);
			_move.actionMap.Enable();

			_players = World.Filter
				.With<PlayerTag>()
				.With<InputMovementDirection>()
				.With<InputAttack>()
				.Build();

			_inputMovementStash = World.GetStash<InputMovementDirection>();
			_inputAttackStash = World.GetStash<InputAttack>();
		}

		public void OnUpdate(float deltaTime)
		{
			Vector2 direction = Vector2.zero;

			bool isAttackPressed = false;

			if (Application.isFocused)
			{
				direction = _move.ReadValue<Vector2>();
				isAttackPressed = _attack.WasPressedThisFrame();
			}

			direction = Vector2.ClampMagnitude(direction, 1f);

			foreach (Entity entity in _players)
			{
				_inputMovementStash.Get(entity).Direction = direction;
				_inputAttackStash.Get(entity).Pressed = isAttackPressed;
			}
		}

		public void Dispose()
		{
			if (_actions == null)
				return;
			_actions.Disable();
			Object.Destroy(_actions);
			_actions = null;
			_move = null;
			_attack = null;
		}
	}
}
