using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/ArenaConfig")]
	public sealed class ArenaConfig : GameplayConfig
	{
		[SerializeField] private Vector2 _halfSize = new Vector2(8f, 4.5f);
		public Vector2 HalfSize => _halfSize;
		[SerializeField, Min(0f)] private float _cameraPadding = 0f;
		public float CameraPadding => _cameraPadding;
		[SerializeField, Min(0.001f)] private float _minCameraSize = 4.5f;
		public float MinCameraSize => _minCameraSize;

		public override void Validate()
		{
			Number(HalfSize.x, "HalfSize.x", true);
			Number(HalfSize.y, "HalfSize.y", true);
			Number(CameraPadding, nameof(CameraPadding));
			Number(MinCameraSize, nameof(MinCameraSize), true);
		}
	}
}
