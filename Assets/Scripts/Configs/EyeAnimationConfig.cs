using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/EyeAnimationConfig")]
	public sealed class EyeAnimationConfig : GameplayConfig
	{
		[SerializeField] private Vector2 _initialBlinkInterval = new Vector2(1.5f, 4f);
		public Vector2 InitialBlinkInterval => _initialBlinkInterval;
		[SerializeField] private Vector2 _blinkInterval = new Vector2(2f, 5f);
		public Vector2 BlinkInterval => _blinkInterval;
		[SerializeField, Min(0.001f)] private float _blinkDuration = 0.18f;
		public float BlinkDuration => _blinkDuration;
		[SerializeField, Min(0f)] private float _lookResponseRate = 12f;
		public float LookResponseRate => _lookResponseRate;
		[SerializeField, Range(0f, 1f)] private float _minOpenness = 0.06f;
		public float MinOpenness => _minOpenness;
		[SerializeField, Min(0f)] private float _minLookDisplacement = 0.001f;
		public float MinLookDisplacement => _minLookDisplacement;
		[SerializeField, Min(0f)] private float _maxEyeOffset = 0.025f;
		public float MaxEyeOffset => _maxEyeOffset;
		[SerializeField, Min(0f)] private float _maxPupilOffset = 0.2f;
		public float MaxPupilOffset => _maxPupilOffset;

		public override void Validate()
		{
			Number(InitialBlinkInterval.x, "InitialBlinkInterval.x", true);
			Number(InitialBlinkInterval.y, "InitialBlinkInterval.y", true);
			Require(InitialBlinkInterval.x <= InitialBlinkInterval.y, "Initial blink interval is reversed.");
			Number(BlinkInterval.x, "BlinkInterval.x", true);
			Number(BlinkInterval.y, "BlinkInterval.y", true);
			Require(BlinkInterval.x <= BlinkInterval.y, "Blink interval is reversed.");
			Number(BlinkDuration, nameof(BlinkDuration), true);
			Number(LookResponseRate, nameof(LookResponseRate));
			Number(MinOpenness, nameof(MinOpenness));
			Require(MinOpenness <= 1f, "MinOpenness must be at most 1.");
			Number(MinLookDisplacement, nameof(MinLookDisplacement));
			Number(MaxEyeOffset, nameof(MaxEyeOffset));
			Number(MaxPupilOffset, nameof(MaxPupilOffset));
		}
	}
}
