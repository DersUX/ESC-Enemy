using Scellecs.Morpeh;

namespace EcsEnemy
{
	public struct Health : IComponent
	{
		public int Value;
	}

	public struct DeadTag : IComponent
	{
	}

	public struct DespawnRequest : IComponent
	{
		public Entity Target;
	}
}
