using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/DamageFeedbackConfig")]
	public sealed class DamageFeedbackConfig : GameplayConfig
	{
		[SerializeField, Min(0.001f)] private float _duration = 0.22f;
		public float Duration => _duration;
		[SerializeField, Range(0f, 0.5f)] private float _squashAmplitude = 0.22f;
		public float SquashAmplitude => _squashAmplitude;
		[SerializeField, Range(0f, 1f)] private float _flashStrength = 1f;
		public float FlashStrength => _flashStrength;
		[SerializeField, Min(0.001f)] private float _flashFadeExponent = 3f;
		public float FlashFadeExponent => _flashFadeExponent;

		public override void Validate()
		{
			Number(Duration, nameof(Duration), true);
			Number(SquashAmplitude, nameof(SquashAmplitude));
			Require(SquashAmplitude <= 0.5f, "SquashAmplitude must be at most 0.5.");
			Number(FlashStrength, nameof(FlashStrength));
			Require(FlashStrength <= 1f, "FlashStrength must be at most 1.");
			Number(FlashFadeExponent, nameof(FlashFadeExponent), true);
		}
	}
}
