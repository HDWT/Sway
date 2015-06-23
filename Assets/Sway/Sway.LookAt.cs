using UnityEngine;
using System;

public partial class Sway : MonoBehaviour
{
	public interface LookAtSetup
	{
		bool CanSetup { get; }

		LookAtSetup Delay(float value);
		LookAtSetup IgnoreTimeScale(bool value);

		LookAtSetup EaseType(AnimationCurve curve);

		LookAtSetup OnStart(Action action);
		LookAtSetup OnUpdate(Action action);
		LookAtSetup OnComplete(Action<float> action);
	}

	private class LookAtTween : Base<LookAtSetup>, LookAtSetup
	{
		private static LookAtTween s_defaultSetup = new LookAtTween(s_singleton.transform, Vector3.zero, 0);
		public static LookAtSetup DefaultSetup
		{
			get
			{
				s_defaultSetup.Stop();
				return s_defaultSetup;
			}
		}

		private Quaternion	m_startRotation = Quaternion.identity;
		private Vector3		m_endPosition	= Vector3.zero;
		
		public LookAtTween(Transform target, Vector3 position, float time)
			: base(target, time)
		{
			m_startRotation = target.rotation;
			m_endPosition = position;
		}

		protected override void OnUpdate(float currentTime, float easeTypeCurveValue)
		{
			Vector3 forward = m_endPosition - m_target.position;

			if (forward != Vector3.zero) // If not looking at self
			{
				Vector3 lookRotation = Quaternion.LookRotation(forward).eulerAngles;
				Vector3 rotation = m_startRotation.eulerAngles;

				rotation.x = GetRotation(rotation.x, lookRotation.x, easeTypeCurveValue);
				rotation.y = GetRotation(rotation.y, lookRotation.y, easeTypeCurveValue);
				rotation.z = GetRotation(rotation.z, lookRotation.z, easeTypeCurveValue);

				m_target.rotation = Quaternion.Euler(rotation);
			}
		}

		private float GetRotation(float from, float to, float easeTypeCurveValue)
		{
			float rotation = from;

			if (to > from)
			{
				rotation += (to - from <= 180)
					? (to - from) * easeTypeCurveValue
					: (to - from - 360) * easeTypeCurveValue;
			}
			else if (from > to)
			{
				rotation += (from - to <= 180)
					? (to - from) * easeTypeCurveValue
					: (to - from + 360) * easeTypeCurveValue;
			}

			if (rotation < 0)	rotation += 360;
			if (rotation > 360) rotation -= 360;

			return rotation;
		}
	}
}