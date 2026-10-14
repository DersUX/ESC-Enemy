using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EcsEnemy
{
	public class CombatEffects : IDisposable
	{
		private static readonly int StartTimeId = Shader.PropertyToID("_StartTime");
		private static readonly int DurationId = Shader.PropertyToID("_Duration");
		private static readonly int RadiusId = Shader.PropertyToID("_Radius");
		private static readonly int ExtentId = Shader.PropertyToID("_Extent");
		private static readonly int BoundsPaddingId = Shader.PropertyToID("_BoundsPadding");

		private readonly GameObject _root;
		private readonly Mesh _quad;
		private readonly Material[] _presets;
		private readonly List<Pulse> _pulses = new();

		public CombatEffects()
		{
			_presets = new[]
			{
				LoadMaterial("Attack"),
				LoadMaterial("Hit"),
				LoadMaterial("Death")
			};
			_root = new GameObject("Combat Effects (pooled)");
			_quad = new Mesh
			{
				name = "Combat Effect Quad",
				vertices = new[]
				{
					new Vector3(-1f, -1f), new Vector3(-1f, 1f),
					new Vector3(1f, 1f), new Vector3(1f, -1f)
				},
				triangles = new[] { 0, 1, 2, 0, 2, 3 }
			};
			_quad.RecalculateBounds();
			_quad.UploadMeshData(true);
		}

		public void Play(Vector2 center, float radius, CombatEffectKind kind)
		{
			if (radius <= 0f)
				return;

			Pulse pulse = _pulses.Find(candidate => !candidate.View.activeSelf);

			if (pulse == null)
			{
				pulse = new Pulse(_root.transform, _quad, _presets[(int)kind]);
				_pulses.Add(pulse);
			}

			pulse.Material.CopyPropertiesFromMaterial(_presets[(int)kind]);
			Vector4 padding = pulse.Material.GetVector(BoundsPaddingId);
			float extent = radius * padding.x + padding.y;
			pulse.View.transform.SetPositionAndRotation(new Vector3(center.x, center.y, -0.1f), Quaternion.identity);
			pulse.View.transform.localScale = new Vector3(extent, extent, 1f);
			pulse.Material.SetFloat(RadiusId, radius);
			pulse.Material.SetFloat(ExtentId, extent);
			pulse.Material.SetFloat(StartTimeId, Time.time);
			pulse.EndTime = Time.time + pulse.Material.GetFloat(DurationId);
			pulse.View.SetActive(true);
		}

		public void Update()
		{
			foreach (Pulse pulse in _pulses)
			{
				if (pulse.View.activeSelf && Time.time >= pulse.EndTime)
					pulse.View.SetActive(false);
			}
		}

		public void Dispose()
		{
			if (_root != null)
			{
				_root.SetActive(false);
				UnityEngine.Object.Destroy(_root);
			}

			foreach (Pulse pulse in _pulses)
				UnityEngine.Object.Destroy(pulse.Material);

			_pulses.Clear();
			UnityEngine.Object.Destroy(_quad);
		}

		private static Material LoadMaterial(string name)
		{
			Material material = Resources.Load<Material>("CombatEffects/" + name);
			return material != null ? material : throw new InvalidOperationException("Missing combat effect material: " + name);
		}

		private class Pulse
		{
			public readonly GameObject View;
			public readonly Material Material;
			public float EndTime;

			public Pulse(Transform parent, Mesh quad, Material preset)
			{
				View = new GameObject("Combat Pulse");
				View.transform.SetParent(parent, false);
				Material = new Material(preset) { name = "Combat Pulse (pooled)" };
				View.AddComponent<MeshFilter>().sharedMesh = quad;
				MeshRenderer renderer = View.AddComponent<MeshRenderer>();
				renderer.sharedMaterial = Material;
				renderer.sortingOrder = 5;
				renderer.shadowCastingMode = ShadowCastingMode.Off;
				renderer.receiveShadows = false;
				View.SetActive(false);
			}
		}
	}
}
