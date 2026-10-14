using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public struct TransformView : IComponent
	{
		public Transform Value;
	}

	public struct EnemyPoolReference : IComponent
	{
		public CharacterEyesView View;
	}
}
