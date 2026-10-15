using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public class EyeAnimationSystem : ISystem
	{
		private readonly EyeAnimationConfig _config;

		public EyeAnimationSystem(EyeAnimationConfig config) => _config = config;

		private const float InactiveBlinkTime = -1f;
		private const float FullEyeOpenness = 1f;

		public World World
		{
			get;
			set;
		}

		private Filter _characters;
		private Stash<EyeAnimation> _eyeAnimationStash;
		private Stash<Position> _positionStash;

		public void OnAwake()
		{
			_characters = World.Filter
				.With<EyeAnimation>()
				.With<Position>()
				.Build();

			_eyeAnimationStash = World.GetStash<EyeAnimation>();
			_positionStash = World.GetStash<Position>();
		}

		public void OnUpdate(float deltaTime)
		{
			foreach (Entity entity in _characters)
			{
				ref EyeAnimation animation = ref _eyeAnimationStash.Get(entity);
				Vector2 position = _positionStash.Get(entity).Value;
				Vector2 displacement = position - animation.PreviousPosition;

				animation.PreviousPosition = position;

				if (displacement.sqrMagnitude > _config.MinLookDisplacement * _config.MinLookDisplacement)
				{
					animation.LookDirection = Vector2.Lerp(animation.LookDirection,
						displacement.normalized, 1f - Mathf.Exp(-_config.LookResponseRate * deltaTime));
				}

				animation.BlinkCountdown -= deltaTime;
				if (animation.BlinkCountdown <= 0f && animation.BlinkTime < 0f)
				{
					animation.BlinkTime = 0f;
					animation.BlinkCountdown = Random.Range(_config.BlinkInterval.x, _config.BlinkInterval.y);
				}

				float openness = FullEyeOpenness;
				if (animation.BlinkTime >= 0f)
				{
					animation.BlinkTime += deltaTime;

					if (animation.BlinkTime >= _config.BlinkDuration)
						animation.BlinkTime = InactiveBlinkTime;
					else
						openness = Mathf.Max(_config.MinOpenness,
							FullEyeOpenness - Mathf.Sin(animation.BlinkTime / _config.BlinkDuration * Mathf.PI));
				}

				Vector3 eyeOffset = animation.LookDirection * _config.MaxEyeOffset;
				Vector3 pupilOffset = animation.LookDirection * _config.MaxPupilOffset;

				ApplyEye(animation.LeftEye, animation.LeftEyePosition + eyeOffset, animation.LeftEyeScale, openness);
				ApplyEye(animation.RightEye, animation.RightEyePosition + eyeOffset, animation.RightEyeScale, openness);

				if (animation.LeftPupil != null)
					animation.LeftPupil.localPosition = animation.LeftPupilPosition + pupilOffset;

				if (animation.RightPupil != null)
					animation.RightPupil.localPosition = animation.RightPupilPosition + pupilOffset;
			}
		}

		private static void ApplyEye(Transform eye, Vector3 position, Vector3 scale, float openness)
		{
			if (eye == null)
				return;

			eye.localPosition = position;
			scale.y *= openness;
			eye.localScale = scale;
		}

		public void Dispose()
		{
			if (_characters == null)
				return;

			foreach (Entity entity in _characters)
			{
				ref EyeAnimation animation = ref _eyeAnimationStash.Get(entity);

				ApplyEye(animation.LeftEye, animation.LeftEyePosition, animation.LeftEyeScale, FullEyeOpenness);
				ApplyEye(animation.RightEye, animation.RightEyePosition, animation.RightEyeScale, FullEyeOpenness);

				if (animation.LeftPupil != null)
					animation.LeftPupil.localPosition = animation.LeftPupilPosition;

				if (animation.RightPupil != null)
					animation.RightPupil.localPosition = animation.RightPupilPosition;
			}
		}
	}
}
