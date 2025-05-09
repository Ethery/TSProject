using UnityEngine;

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
			Instance = null;
			Debug.Log($"Destroyed {GetType().Name}");
		}
	}

	public void ForceInit()
	{
		if (!IsInstanced)
		{
			Instance = this as T;

			if (Application.isPlaying)
			{
				DontDestroyOnLoad(this);
			}

			InitInstance();

			Debug.Log($"{GetType().Name} Initialized", this);
		}

		if (Instance != this)
		{
			Debug.LogError($"{GetType().Name} is already instanced destroying second instance at {this.gameObject}");
			Destroy(this);
		}
	}

	protected virtual void InitInstance() { }

	#endregion
}
