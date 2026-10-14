using Scellecs.Morpeh;
using UnityEngine;

namespace EcsEnemy
{
	public struct EyeAnimation : IComponent
	{
		public Transform LeftEye;
		public Transform RightEye;
		public Transform LeftPupil;
		public Transform RightPupil;

		public Vector3 LeftEyePosition;
		public Vector3 RightEyePosition;

		public Vector3 LeftEyeScale;
		public Vector3 RightEyeScale;

		public Vector3 LeftPupilPosition;
		public Vector3 RightPupilPosition;

		public Vector2 PreviousPosition;

		public Vector2 LookDirection;

		public float BlinkCountdown;
		public float BlinkTime;
	}
}
