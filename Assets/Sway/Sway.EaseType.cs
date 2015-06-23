using UnityEngine;
using System;

public partial class Sway : MonoBehaviour
{
	public static class EaseType
	{
		public static AnimationCurve Linear
		{
			get { return new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1)); }
		}
	}

	private static class PathType
	{
		private static AnimationCurve m_linear = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));

		public static AnimationCurve Linear
		{
			get { return m_linear; }
		}
	}
}