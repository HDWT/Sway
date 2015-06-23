using UnityEngine;
using System;

public partial class Sway : MonoBehaviour
{
	public interface IMoveTo
	{
		bool CanSetup { get; }

		IMoveTo Delay(float value);
		IMoveTo IgnoreTimeScale(bool value);
		IMoveTo Space(Space space);

		IMoveTo EaseType(AnimationCurve curve);
		IMoveTo Forward(AnimationCurve curve);
		IMoveTo Up(AnimationCurve curve);

		IMoveTo OnStart(Action action);
		IMoveTo OnUpdate(Action action);
		IMoveTo OnComplete(Action<float> action);
	}

	private class MoveToTween : Base<IMoveTo>, IMoveTo
	{
		private static MoveToTween s_defaultSetup = new MoveToTween(s_singleton.transform, Vector3.zero, 0);
		public static IMoveTo DefaultSetup
		{
			get
			{
				s_defaultSetup.Stop();
				return s_defaultSetup;
			}
		}

		private static readonly Quaternion s_rotation = Quaternion.Euler(0, 270, 0);

		private Vector3			m_startPosition		= Vector3.zero;
		private Vector3			m_endPosition		= Vector3.zero;
		private float			m_distance			= 0;
		private Space			m_space				= UnityEngine.Space.World;
		private Vector3			m_forward			= Vector3.forward;
		private Vector3			m_up				= Vector3.up;

		private AnimationCurve	m_moveForwardCurve	= PathType.Linear;
		private AnimationCurve	m_moveUpCurve		= PathType.Linear;

		public MoveToTween(Transform target, Vector3 position, float time)
			: base(target, time)
		{
			m_endPosition = position;
			Space(m_space);
		}

		protected override void OnUpdate(float currentTime, float easeTypeCurveValue)
		{
			float moveForwardCurveValue = m_distance * m_moveForwardCurve.Evaluate(easeTypeCurveValue);
			float moveUpCurveValue = m_distance * m_moveUpCurve.Evaluate(easeTypeCurveValue);

			// Calculate current position
			Vector3 currentPosition = m_startPosition + (m_endPosition - m_startPosition) * easeTypeCurveValue + m_forward * moveForwardCurveValue + m_up * moveUpCurveValue;

			//
			if (m_space == UnityEngine.Space.World)
				m_target.position = currentPosition;
			else
				m_target.localPosition = currentPosition;
		}

		#region --- Setup Function ---

		/// <summary> Default - World </summary>
		public IMoveTo Space(Space space)
		{
			if (!CanSetup)
				return this;

			m_space = space;

			if (m_target != null)
			{
				m_startPosition = (m_space == UnityEngine.Space.World) ? (m_target.position) : (m_target.localPosition);
				m_distance = Vector3.Distance(m_startPosition, m_endPosition);

				// Get Forward and Up Vector
				Quaternion currentRotation = m_target.rotation;

				m_target.rotation = (m_endPosition - m_startPosition != Vector3.zero)
					? Quaternion.LookRotation(m_endPosition - m_startPosition) * s_rotation
					: m_target.rotation;

				m_forward = m_target.forward;
				m_up = m_target.up;

				m_target.rotation = currentRotation;
			}

			return this;
		}

		/// <summary> Default - Linear </summary>
		public IMoveTo Forward(AnimationCurve curve)
		{
			if (CanSetup)
				m_moveForwardCurve = (curve != null) ? (curve) : (PathType.Linear);

			return this;
		}

		/// <summary> Default - Linear </summary>
		public IMoveTo Up(AnimationCurve curve)
		{
			if (CanSetup)
				m_moveUpCurve = (curve != null) ? (curve) : (PathType.Linear);

			return this;
		}

		#endregion
	}
}