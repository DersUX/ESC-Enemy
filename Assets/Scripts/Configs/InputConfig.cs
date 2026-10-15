using UnityEngine;
using UnityEngine.InputSystem;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/InputConfig")]
	public sealed class InputConfig : GameplayConfig
	{
		public const string MoveAction = "Gameplay/Move";
		public const string AttackAction = "Gameplay/Attack";

		[SerializeField] private InputActionAsset _actions;
		public InputActionAsset Actions => _actions;

		public override void Validate()
		{
			Require(Actions != null, "Input Actions asset is required.");
			InputAction move = Actions.FindAction(MoveAction);
			InputAction attack = Actions.FindAction(AttackAction);
			Require(move != null && move.type == InputActionType.Value && move.expectedControlType == "Vector2",
				$"{MoveAction} must be a Value action with Vector2 control type.");
			Require(attack != null && attack.type == InputActionType.Button,
				$"{AttackAction} must be a Button action.");
		}
	}
}
