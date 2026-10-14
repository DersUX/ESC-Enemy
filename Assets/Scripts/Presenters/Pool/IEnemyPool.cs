using EcsEnemy;
using UnityEngine;

namespace Presenters
{
	public interface IEnemyPool
	{
		CharacterEyesView Rent();

		void Prepare(CharacterEyesView view, Vector3 position);

		void Activate(CharacterEyesView view);

		void Return(CharacterEyesView view);
	}
}
