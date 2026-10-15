using UnityEngine;

namespace EcsEnemy
{
	public abstract class GameplayConfig : ScriptableObject
	{
		public abstract void Validate();

		protected virtual void OnValidate()
		{
			try { Validate(); }
			catch (System.InvalidOperationException error) { Debug.LogError(error.Message, this); }
		}

		protected void Require(bool condition, string message)
		{
			if (!condition)
				throw new System.InvalidOperationException($"{name} ({GetType().Name}): {message}");
		}

		protected void Number(float value, string field, bool positive = false)
		{
			Require(!float.IsNaN(value) && !float.IsInfinity(value) &&
				(positive ? value > 0f : value >= 0f), $"{field} must be finite and {(positive ? "positive" : "non-negative")}.");
		}
	}
}
