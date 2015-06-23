using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

public partial class Sway : MonoBehaviour
{
	private interface ITween
	{
		void Start();
		void Start(float time);
		void Update();
		void Stop();

		bool IsRun	{ get; }
		bool IsDone { get; }
	}

	private abstract class Base<T> : ITween where T : class
	{
		protected readonly Transform m_target = null;

		private static AnimationCurve s_easeTypeLinear = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

		private float			m_time				= 0;
		private float			m_delay				= 0;		// Может быть отрицательный, чтобы начать с 0,5 сек например
		private bool			m_ignoreTimeScale	= false;
		private AnimationCurve	m_easeTypeCurve		= s_easeTypeLinear;

		private bool			m_canSetup			= true;
		private float			m_startTime			= float.NaN;
		private float			m_currentTime		= 0;
		private bool			m_done				= false;
		private float			m_overtime			= 0;

		private Action			m_onStart			= null;
		private Action			m_onUpdate			= null;
		private Action<float>	m_onComplete		= null;

		public bool				CanSetup			{ get { return m_canSetup; } }
		public bool				IsRun				{ get { return !float.IsNaN(m_startTime); } }
		public bool				IsDone				{ get { return m_done; } }

		public Base(Transform target, float time)
		{
			m_target = target;
			m_time = (time < 0) ? (0) : (time);
		}

		public void Start()
		{
			float startTime = (m_delay < 0) ? (Mathf.Abs(m_delay)) : (0);

			Start(startTime);
		}
		
		public void Start(float time)
		{
			if (IsRun)
			{
				Debug.LogError("Tween already started");
				return;
			}

			m_canSetup		= false;
			m_startTime		= GetUnityTime() - Mathf.Clamp(time, 0, m_time);
			m_currentTime	= 0;
			m_done			= false;

			if (m_delay < 0)
				m_delay = 0;

			OnStart();

			if (m_onStart != null)
				m_onStart();
		}

		public void Update()
		{
			if (!IsRun)
				return;

			m_currentTime = GetUnityTime() - m_startTime;

			if (m_currentTime < m_delay)
				return;

			m_currentTime -= m_delay;

			if (m_currentTime > m_time)
				m_currentTime = m_time;

			// Target has been destroyed
			if (m_target == null)
			{
				m_done = true;
				return;
			}

			// Update values
			if (m_currentTime <= m_time)
			{
				float easeTypeCurveValue = GetEasyTypeCurveValue(m_currentTime);
				easeTypeCurveValue = Mathf.Clamp(easeTypeCurveValue, 0, 1);

				OnUpdate(m_currentTime, easeTypeCurveValue);

				if (m_onUpdate != null)
					m_onUpdate();
			}

			// 
			if (m_currentTime >= m_time)
			{
				m_done = true;
				m_overtime = m_currentTime - m_time;
			}
		}

		public void Stop()
		{
			if (!IsRun)
				return;

			m_startTime = float.NaN;

			if (!m_done)
			{
				m_done = true;
				m_overtime = m_currentTime - m_time;
			}

			OnStop();

			if (m_onComplete != null)
				m_onComplete(m_overtime);
		}

		protected abstract void OnUpdate(float currentTime, float easeTypeCurveValue);

		protected virtual void OnStart()
		{ }

		protected virtual void OnStop()
		{ }

		private float GetEasyTypeCurveValue(float currentTime)
		{
			float time = (m_time != 0) ? (currentTime / m_time) : (1);

			return m_easeTypeCurve.Evaluate(time);
		}

		private float GetUnityTime()
		{
			return m_ignoreTimeScale ? UnityEngine.Time.realtimeSinceStartup : UnityEngine.Time.time;
		}

		#region --- Setup Functions ---

		public T Delay(float value)
		{
			if (CanSetup)
				m_delay = value;

			return this as T;
		}

		public T IgnoreTimeScale(bool value)
		{
			if (CanSetup)
				m_ignoreTimeScale = value;

			return this as T;
		}

		public T EaseType(AnimationCurve curve)
		{
			if (CanSetup)
				m_easeTypeCurve = (curve != null) ? (curve) : (s_easeTypeLinear);

			return this as T;
		}

		public T OnStart(Action action)
		{
			if (CanSetup)
				m_onStart = action;

			return this as T;
		}

		public T OnUpdate(Action action)
		{
			if (CanSetup)
				m_onUpdate = action;

			return this as T;
		}

		public T OnComplete(Action<float> action)
		{
			if (CanSetup)
				m_onComplete = action;

			return this as T;
		}

		#endregion
	}
}