using EcsEnemy;
using UnityEngine;

namespace Views
{
	public interface IEnemyPoolView
	{
		CharacterEyesView CreateInstance();

		void Prepare(CharacterEyesView view, Vector3 position);

		void Activate(CharacterEyesView view);

		void Deactivate(CharacterEyesView view);

		void DestroyInstance(CharacterEyesView view);
	}
}
