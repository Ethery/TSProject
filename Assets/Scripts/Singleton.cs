using UnityEngine;
//!	@class	Singleton
//!
//!	@brief	singleton pattern class implementation for a mono behaviour.

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; private set; }
	public static bool IsInstanced => Instance != null;

	#region Private

	protected virtual void Awake()
	{
		ForceInit();
	}

	protected virtual void OnDestroy()
	{
		if (Instance == this)
		{
			Debug.Log(string.Format("{0}: destroy", GetType().Name), this);
			Instance = null;
		}
	}

	protected virtual void OnApplicationQuit()
	{
	}

	public void ForceInit()
	{
		if (!IsInstanced)
		{
			Instance = this as T;

			if (transform.parent == null && Application.isPlaying)
			{
				DontDestroyOnLoad(this);
			}
			Debug.Log(string.Format("{0}: awake", GetType().Name), this);
		}

		if (Instance != this)
		{
			Debug.LogError(string.Format("{0}: already instanced -> destroying {1}", GetType().Name, name));
			Destroy(this);
		}
	}

	protected virtual void InitInstance() { }

	#endregion
}
