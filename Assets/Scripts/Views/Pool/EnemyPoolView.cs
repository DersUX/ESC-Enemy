using EcsEnemy;
using ProjectUtils;
using UnityEngine;

namespace Views
{
	public class EnemyPoolView : MonoBehaviour, IEnemyPoolView
	{
		private CharacterEyesView _enemyPrefab;
		private DamageFeedbackConfig _damageFeedback;

		[Space] [SerializeField] private Transform _activeContainer;
		[SerializeField] private Transform _inactiveContainer;

		private void Awake()
		{
			Utils.AssertIsNotNull(_activeContainer);
			Utils.AssertIsNotNull(_inactiveContainer);

			if (_activeContainer.IsChildOf(_inactiveContainer) || transform.IsChildOf(_inactiveContainer))
				throw new System.InvalidOperationException(
					"Inactive container must not contain the active container or EnemyPoolView.");
		}

		public void Configure(EnemyConfig config, DamageFeedbackConfig damageFeedback)
		{
			Utils.AssertIsNotNull(config);
			Utils.AssertIsNotNull(damageFeedback);
			_enemyPrefab = config.Prefab;
			_damageFeedback = damageFeedback;
		}

		public CharacterEyesView CreateInstance()
		{
			Utils.AssertIsNotNull(_enemyPrefab);
			_inactiveContainer.gameObject.SetActive(false);
			CharacterEyesView view = Instantiate(_enemyPrefab, _inactiveContainer);

			view.gameObject.SetActive(false);
			view.GetComponent<CharacterDamageView>().Configure(_damageFeedback);
			view.ResetVisualState();

			return view;
		}

		public void Prepare(CharacterEyesView view, Vector3 position)
		{
			view.gameObject.SetActive(false);
			view.transform.SetParent(_activeContainer, false);

			view.transform.localRotation = _enemyPrefab.transform.localRotation;
			view.transform.localScale = _enemyPrefab.transform.localScale;
			view.transform.position = position;

			view.ResetVisualState();
		}

		public void Activate(CharacterEyesView view)
		{
			view.gameObject.SetActive(true);
		}

		public void Deactivate(CharacterEyesView view)
		{
			view.gameObject.SetActive(false);
			view.ResetVisualState();
			view.transform.SetParent(_inactiveContainer, false);
		}

		public void DestroyInstance(CharacterEyesView view)
		{
			if (view == null)
				return;

			view.gameObject.SetActive(false);
			Destroy(view.gameObject);
		}
	}
}
