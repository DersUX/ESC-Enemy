using UnityEngine;

namespace EcsEnemy
{
	[CreateAssetMenu(menuName = "ECS Enemy/Configs/CombatEffectsConfig")]
	public sealed class CombatEffectsConfig : GameplayConfig
	{
		[System.Serializable]
		public sealed class PulseSettings
		{
			[SerializeField] private Material _material;
			[SerializeField, Min(0.001f)] private float _duration = 0.26f;
			[SerializeField] private Color _tint = Color.white;
			[SerializeField] private Color _highlightTint = Color.white;
			[SerializeField] private Vector2 _boundsPadding = new Vector2(1.35f, 0.25f);
			public Material Material => _material;
			public float Duration => _duration;
			public Color Tint => _tint;
			public Color HighlightTint => _highlightTint;
			public Vector2 BoundsPadding => _boundsPadding;
		}

		[SerializeField] private PulseSettings _attack = new();
		[SerializeField] private PulseSettings _hit = new();
		[SerializeField] private PulseSettings _death = new();
		[SerializeField, Min(0f)] private float _hitRadius = 0.4f;
		[SerializeField, Min(0f)] private float _deathRadius = 0.75f;
		[SerializeField] private float _depth = -0.1f;
		[SerializeField] private int _sortingOrder = 5;
		public float HitRadius => _hitRadius;
		public float DeathRadius => _deathRadius;
		public float Depth => _depth;
		public int SortingOrder => _sortingOrder;

		public PulseSettings Get(CombatEffectKind kind) => kind switch
		{
			CombatEffectKind.Attack => _attack,
			CombatEffectKind.Hit => _hit,
			CombatEffectKind.Death => _death,
			_ => throw new System.ArgumentOutOfRangeException(nameof(kind))
		};

		public override void Validate()
		{
			Number(HitRadius, nameof(HitRadius));
			Number(DeathRadius, nameof(DeathRadius));
			Require(!float.IsNaN(Depth) && !float.IsInfinity(Depth), "Depth must be finite.");
			Require(SortingOrder >= short.MinValue && SortingOrder <= short.MaxValue, "SortingOrder is out of range.");
			foreach (CombatEffectKind kind in System.Enum.GetValues(typeof(CombatEffectKind)))
			{
				PulseSettings preset = Get(kind);
				Require(preset != null && preset.Material != null, $"{kind} material is required.");
				Number(preset.Duration, $"{kind}.Duration", true);
				Number(preset.BoundsPadding.x, $"{kind}.BoundsPadding.x", true);
				Number(preset.BoundsPadding.y, $"{kind}.BoundsPadding.y");
				foreach (string property in new[] { "_Duration", "_Tint", "_HighlightTint", "_BoundsPadding", "_StartTime", "_Radius", "_Extent" })
					Require(preset.Material.HasProperty(property), $"{kind} material is missing {property}.");
			}
		}
	}
}
