using System;

namespace ProjectUtils
{
	public static class Utils
	{
		public static void AssertIsNotNull(object field)
		{
			if (IsNull(field))
				throw new InvalidOperationException($"{nameof(field)} must not be null.");
		}

		private static bool IsNull(object field)
		{
			if (field is UnityEngine.Object unityObject)
				return unityObject == null;

			return field == null;
		}
	}
}
