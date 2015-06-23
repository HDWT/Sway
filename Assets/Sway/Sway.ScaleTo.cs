using UnityEngine;
using System;

public partial class Sway : MonoBehaviour
{
	public interface ScaleToSetup
	{
		bool CanSetup { get; }

		ScaleToSetup Delay(float value);
		ScaleToSetup IgnoreTimeScale(bool value);

		ScaleToSetup EaseType(AnimationCurve curve);

		ScaleToSetup OnStart(Action action);
		ScaleToSetup OnUpdate(Action action);
		ScaleToSetup OnComplete(Action<float> action);
	}

	private class ScaleToTween : Base<ScaleToSetup>, ScaleToSetup
	{
		private static readonly ScaleToTween s_defaultSetup = new ScaleToTween(s_singleton.transform, Vector3.one, 0);
		public static ScaleToSetup DefaultSetup
		{
			get
			{
				s_defaultSetup.Stop();
				return s_defaultSetup;
			}
		}

		private Vector3 m_startScale = Vector3.one;
		private Vector3	m_endScale	 = Vector3.one;

		public ScaleToTween(Transform target, Vector3 scale, float time)
			: base(target, time)
		{
			m_startScale = target.localScale;
			m_endScale = scale;
		}

		protected override void OnUpdate(float currentTime, float easeTypeCurveValue)
		{
			Vector3 currentScale = m_startScale;

			currentScale.x += (m_endScale.x - m_startScale.x) * easeTypeCurveValue;
			currentScale.y += (m_endScale.y - m_startScale.y) * easeTypeCurveValue;
			currentScale.z += (m_endScale.z - m_startScale.z) * easeTypeCurveValue;

			m_target.localScale = currentScale;
		}
	}
}