using EcsEnemy;
using ProjectUtils;
using UnityEngine;

namespace Views
{
	public class EnemyPoolView : MonoBehaviour, IEnemyPoolView
	{
		[SerializeField] private CharacterEyesView _enemyPrefab;

		[Space] [SerializeField] private Transform _activeContainer;
		[SerializeField] private Transform _inactiveContainer;

		private void Awake()
		{
			Utils.AssertIsNotNull(_enemyPrefab);
			Utils.AssertIsNotNull(_activeContainer);
			Utils.AssertIsNotNull(_inactiveContainer);

			if (_activeContainer.IsChildOf(_inactiveContainer) || transform.IsChildOf(_inactiveContainer))
				throw new System.InvalidOperationException(
					"Inactive container must not contain the active container or EnemyPoolView.");
		}

		public CharacterEyesView CreateInstance()
		{
			_inactiveContainer.gameObject.SetActive(false);
			CharacterEyesView view = Instantiate(_enemyPrefab, _inactiveContainer);

			view.gameObject.SetActive(false);
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
