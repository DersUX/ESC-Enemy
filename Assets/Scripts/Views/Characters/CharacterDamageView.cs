using ProjectUtils;
using UnityEngine;

namespace EcsEnemy
{
	[DisallowMultipleComponent]
	public class CharacterDamageView : MonoBehaviour
	{
		[SerializeField] private Transform _visualRoot;
		[SerializeField] private SpriteRenderer _body;
		private DamageFeedbackConfig _config;

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

		public void Configure(DamageFeedbackConfig config)
		{
			Utils.AssertIsNotNull(config);
			_config = config;
		}

		public void PlayHit()
		{
			Utils.AssertIsNotNull(_config);
			_startTime = Time.time;
			_playing = true;
			ApplyHit(0f);
		}

		private void LateUpdate()
		{
			if (!_playing)
				return;

			float progress = (Time.time - _startTime) / _config.Duration;
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
			float squash = Mathf.Sin(progress * Mathf.PI * 2f) * remaining * _config.SquashAmplitude;
			_visualRoot.localScale = Vector3.Scale(_initialScale, new Vector3(1f + squash, 1f - squash, 1f));

			float flash = Mathf.Pow(remaining, _config.FlashFadeExponent) * _config.FlashStrength;
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
