using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/SimulationConfig")]
	public sealed class SimulationConfig : GameplayConfig
	{
		[SerializeField, Min(1)] private int _separationPassCount = 2;
		public int SeparationPassCount => _separationPassCount;

		public override void Validate()
		{
			Require(SeparationPassCount > 0, "SeparationPassCount must be positive.");
		}
	}
}
