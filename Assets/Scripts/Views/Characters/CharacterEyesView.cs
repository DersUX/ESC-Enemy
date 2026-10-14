using ProjectUtils;
using UnityEngine;

namespace EcsEnemy
{
	public class CharacterEyesView : MonoBehaviour
	{
		private const float MinBlinkCountdown = 1.5f;
		private const float MaxBlinkCountdown = 4f;
		private const float InactiveBlinkTime = -1f;

		[SerializeField] private Transform _leftEye;
		[SerializeField] private Transform _rightEye;
		[SerializeField] private Transform _leftPupil;
		[SerializeField] private Transform _rightPupil;

		private bool _initialStateCaptured;
		private Vector3 _leftEyePosition;
		private Vector3 _rightEyePosition;
		private Vector3 _leftEyeScale;
		private Vector3 _rightEyeScale;
		private Vector3 _leftPupilPosition;
		private Vector3 _rightPupilPosition;

		private void Awake()
		{
			Utils.AssertIsNotNull(_leftEye);
			Utils.AssertIsNotNull(_rightEye);
			Utils.AssertIsNotNull(_leftPupil);
			Utils.AssertIsNotNull(_rightPupil);

			CaptureInitialState();
		}

		private void CaptureInitialState()
		{
			if (_initialStateCaptured)
				return;

			_leftEyePosition = _leftEye.localPosition;
			_rightEyePosition = _rightEye.localPosition;
			_leftEyeScale = _leftEye.localScale;
			_rightEyeScale = _rightEye.localScale;
			_leftPupilPosition = _leftPupil.localPosition;
			_rightPupilPosition = _rightPupil.localPosition;
			_initialStateCaptured = true;
		}

		public void ResetVisualState()
		{
			CaptureInitialState();
			_leftEye.localPosition = _leftEyePosition;
			_rightEye.localPosition = _rightEyePosition;
			_leftEye.localScale = _leftEyeScale;
			_rightEye.localScale = _rightEyeScale;
			_leftPupil.localPosition = _leftPupilPosition;
			_rightPupil.localPosition = _rightPupilPosition;
		}

		public EyeAnimation CreateAnimation(Vector2 position)
		{
			CaptureInitialState();
			return new EyeAnimation
			{
				LeftEye = _leftEye,
				RightEye = _rightEye,

				LeftPupil = _leftPupil,
				RightPupil = _rightPupil,

				LeftEyePosition = _leftEyePosition,
				RightEyePosition = _rightEyePosition,

				LeftEyeScale = _leftEyeScale,
				RightEyeScale = _rightEyeScale,

				LeftPupilPosition = _leftPupilPosition,
				RightPupilPosition = _rightPupilPosition,

				PreviousPosition = position,
				BlinkCountdown = Random.Range(MinBlinkCountdown, MaxBlinkCountdown),
				BlinkTime = InactiveBlinkTime
			};
		}
	}
}
