using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public partial class Sway : MonoBehaviour
{
	private class TweenInfo
	{
		public Component	Component	{ get; private set; }
		public string		MemberName	{ get; private set; }
		public ITween		Tween		{ get; set; }

		public TweenInfo(Component component, string memberName, ITween tween)
		{
			this.Component	= component;
			this.MemberName = memberName;
			this.Tween		= tween;
		}
	}

	private class Instance
	{
		private Transform	m_target		= null;
		private GameObject	m_gameObject	= null;
		private bool		m_destroyed		= false;

		private ITween[] m_tweens = new ITween[TypeValues.Length];

		public Instance(Transform target)
		{
			m_target = target;

			m_destroyed = (m_target == null);

			if (!m_destroyed)
				m_gameObject = m_target.gameObject;
		}

		public bool Destroyed
		{
			get { return m_destroyed; }
		}

		//
		public IMoveTo InitMoveTo(Vector3 position, float time)
		{
			if (m_destroyed)
				return MoveToTween.DefaultSetup;

			Stop(Type.MoveTo);

			MoveToTween tween = new MoveToTween(m_target, position, time);
			m_tweens[(int)Type.MoveTo] = tween;

			return tween;
		}
		
		//
		public LookAtSetup InitLookAt(Vector3 position, float time)
		{
			if (m_destroyed)
				return LookAtTween.DefaultSetup;

			Stop(Type.LookAt);

			LookAtTween tween = new LookAtTween(m_target, position, time);
			m_tweens[(int)Type.LookAt] = tween;

			return tween;
		}

		//
		public ScaleToSetup InitScaleTo(Vector3 scale, float time)
		{
			if (m_destroyed)
				return ScaleToTween.DefaultSetup;

			Stop(Type.ScaleTo);

			ScaleToTween tween =  new ScaleToTween(m_target, scale, time);
			m_tweens[(int)Type.ScaleTo] = tween;

			return tween;
		}

		public bool IsPlaying(Type type)
		{
			return m_tweens[(int)type] != null;
		}

		public bool IsPlaying()
		{
			foreach (Type type in TypeValues)
			{
				if (IsPlaying(type))
					return true;
			}

			return false;
		}

		public void Stop(Type type)
		{
			int index = (int)type;
			var tween = m_tweens[index];

			if ((index < m_tweens.Length) && (tween != null))
			{
				m_tweens[index] = null;
				tween.Stop();
			}
		}

		public void StopAll()
		{
			for (int i = 0; i < m_tweens.Length; ++i)
			{
				var tween = m_tweens[i];

				if (tween != null)
				{
					m_tweens[i] = null;
					tween.Stop();
				}
			}
		}

		public void Update()
		{
			if (m_destroyed)
				return;

			if (m_target == null)
			{
				m_destroyed = true;
				return;
			}

			if (!m_gameObject.activeInHierarchy)
				return;

			for (int i = 0; i < m_tweens.Length; ++i)
			{
				ITween tween = m_tweens[i];

				if (tween == null)
					continue;

				if (!tween.IsRun)
					tween.Start();

				tween.Update();

				if (tween.IsDone)
				{
					m_tweens[i] = null;
					tween.Stop();
				}
			}
		}
	}
}