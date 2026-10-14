using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EcsEnemy
{
	public class PlayerInputSystem : ISystem
	{
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
			Keyboard keyboard = Keyboard.current;

			bool isAttackPressed = false;

			if (keyboard != null && Application.isFocused)
			{
				if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
					direction.x -= 1;

				if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
					direction.x += 1;

				if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
					direction.y -= 1;

				if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
					direction.y += 1;

				if (keyboard.spaceKey.wasPressedThisFrame)
					isAttackPressed = true;
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
		}
	}
}
