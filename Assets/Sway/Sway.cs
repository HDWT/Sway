using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public partial class Sway : MonoBehaviour
{
	// Update order
	private static Type[] TypeValues = (Type[])Enum.GetValues(typeof(Type));
	public enum Type
	{
		MoveTo		= 0,
		LookAt		= 1,
		ScaleTo		= 2,
	}

	private static readonly Dictionary<Transform, Instance> s_instancesByTarget = new Dictionary<Transform, Instance>(16);
	private static readonly Dictionary<Transform, Instance> s_newInstances = new Dictionary<Transform, Instance>(16);

	private static Sway s_singleton = null;

	private static void EnsureSingletonExist()
	{
		if (s_singleton == null || s_singleton.gameObject == null)
		{
			s_singleton = new GameObject("Sway").AddComponent<Sway>();
			s_singleton.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private static Instance GetInstance(Transform target)
	{
		EnsureSingletonExist();

		if ((target == null) || (target.gameObject == null))
		{
			Debug.LogError("Sway.GetInstance failed - target is null");
			return null;
		}

		Instance instance = null;
			
		if (!s_instancesByTarget.TryGetValue(target, out instance) && !s_newInstances.TryGetValue(target, out instance))
		{
			instance = new Instance(target);
			s_newInstances.Add(target, instance);
		}

		return instance;
	}

	/// <summary> </summary>
	public static IMoveTo MoveTo(Transform target, Vector3 position, float time)
	{
		Instance instance = GetInstance(target);

		return (instance != null) ? instance.InitMoveTo(position, time) : MoveToTween.DefaultSetup;
	}

	/// <summary> </summary>
	public static LookAtSetup LookAt(Transform target, Vector3 position, float time)
	{
		Instance instance = GetInstance(target);

		return (instance != null) ? instance.InitLookAt(position, time) : LookAtTween.DefaultSetup;
	}

	/// <summary> </summary>
	public static ScaleToSetup ScaleTo(Transform target, Vector3 scale, float time)
	{
		Instance instance = GetInstance(target);

		return (instance != null) ? instance.InitScaleTo(scale, time) : ScaleToTween.DefaultSetup;
	}

	/// <summary> </summary>
	public static bool IsPlaying(Transform target, Type type)
	{
		if ((target == null) || (target.gameObject == null))
			return false;

		Instance instance = null;

		if (s_instancesByTarget.TryGetValue(target, out instance) || s_newInstances.TryGetValue(target, out instance))
			return instance.IsPlaying(type);

		return false;
	}

	/// <summary> </summary>
	public static bool IsPlaying(Transform target)
	{
		if ((target == null) || (target.gameObject == null))
			return false;

		Instance instance = null;

		if (s_instancesByTarget.TryGetValue(target, out instance) || s_newInstances.TryGetValue(target, out instance))
			return instance.IsPlaying();

		return false;
	}

	/// <summary> </summary>
	public static void Stop(Transform target, Type type)
	{
		if (target == null)
			return;

		Instance instace = null;

		if (s_instancesByTarget.TryGetValue(target, out instace) || s_newInstances.TryGetValue(target, out instace))
			instace.Stop(type);
	}

	/// <summary> </summary>
	public static void StopAll(Transform target)
	{
		if ((target == null) || (target.gameObject == null))
			return;

		Instance instace = null;

		if (s_instancesByTarget.TryGetValue(target, out instace) || s_newInstances.TryGetValue(target, out instace))
			instace.StopAll();
	}

	private List<Transform> localVar_destroyedTargets = new List<Transform>(10);

	private void Update()
	{
		localVar_destroyedTargets.Clear();
		
		// Add new tweens
		if (s_newInstances.Count != 0)
		{
			foreach (var pair in s_newInstances)
				s_instancesByTarget.Add(pair.Key, pair.Value);

			s_newInstances.Clear();
		}

		// Update all tweens
		foreach (var pair in s_instancesByTarget)
		{
			Instance instance = pair.Value;

			if (instance.Destroyed)
				localVar_destroyedTargets.Add(pair.Key);
			else
				instance.Update();
		}

		// Remove destroyed objects
		for (int i = 0; i < localVar_destroyedTargets.Count; ++i)
			s_instancesByTarget.Remove(localVar_destroyedTargets[i]);
	}
}