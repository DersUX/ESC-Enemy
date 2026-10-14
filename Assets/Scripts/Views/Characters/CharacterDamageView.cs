using ProjectUtils;
using UnityEngine;

namespace EcsEnemy
{
	[DisallowMultipleComponent]
	public class CharacterDamageView : MonoBehaviour
	{
		[SerializeField] private Transform _visualRoot;
		[SerializeField] private SpriteRenderer _body;
		[SerializeField, Min(0.001f)] private float _duration = 0.22f;
		[SerializeField, Range(0f, 0.5f)] private float _squashAmplitude = 0.22f;
		[SerializeField, Range(0f, 1f)] private float _flashStrength = 1f;
		[SerializeField, Min(0.001f)] private float _flashFadeExponent = 3f;

		private Vector3 _initialScale;
		private Color _initialColor;
		private float _startTime;
		private bool _initialized;
		private bool _playing;

		private void Awake()
		{
			Utils.AssertIsNotNull(_visualRoot);
			Utils.AssertIsNotNull(_body);
			_initialScale = _visualRoot.localScale;
			_initialColor = _body.color;
			_initialized = true;
		}

		public void PlayHit()
		{
			_startTime = Time.time;
			_playing = true;
			ApplyHit(0f);
		}

		private void LateUpdate()
		{
			if (!_playing)
				return;

			float progress = (Time.time - _startTime) / Mathf.Max(0.001f, _duration);
			if (progress >= 1f)
			{
				ResetHit();
				return;
			}

			ApplyHit(Mathf.Clamp01(progress));
		}

		private void ApplyHit(float progress)
		{
			float remaining = 1f - progress;
			float squash = Mathf.Sin(progress * Mathf.PI * 2f) * remaining * _squashAmplitude;
			_visualRoot.localScale = Vector3.Scale(_initialScale, new Vector3(1f + squash, 1f - squash, 1f));

			float flash = Mathf.Pow(remaining, Mathf.Max(0.001f, _flashFadeExponent)) * _flashStrength;
			Color color = Color.Lerp(_initialColor, Color.white, flash);
			color.a = _initialColor.a;
			_body.color = color;
		}

		private void ResetHit()
		{
			_playing = false;
			if (!_initialized)
				return;

			if (_visualRoot != null)
				_visualRoot.localScale = _initialScale;
			if (_body != null)
				_body.color = _initialColor;
		}

		private void OnDisable() => ResetHit();

		private void OnDestroy() => ResetHit();
	}
}
