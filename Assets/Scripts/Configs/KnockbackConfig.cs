using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/KnockbackConfig")]
	public sealed class KnockbackConfig : GameplayConfig
	{
		[SerializeField, Min(0f)] private float _impulse = 9f;
		public float Impulse => _impulse;
		[SerializeField, Min(0f)] private float _duration = 0.18f;
		public float Duration => _duration;
		[SerializeField, Min(0.001f)] private float _decayRate = 14f;
		public float DecayRate => _decayRate;

		public override void Validate()
		{
			Number(Impulse, nameof(Impulse));
			Number(Duration, nameof(Duration));
			Number(DecayRate, nameof(DecayRate), true);
		}
	}
}
