
using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Check to see if we're about to be destroyed
	private static bool m_ShuttingDown = false;
	private static object m_Lock = new object();
	private static T m_Instance;

	/// <summary>
	/// Access singleton instance through this propriety.
	/// </summary>
	public static T Instance
	{
		get
		{
			if (m_ShuttingDown)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
								 "' already destroyed. Returning null.");
				return null;
			}

			lock (m_Lock)
			{
				if (m_Instance == null)
				{
					// Search for existing instance.
					var instances = FindObjectsByType(typeof(T), FindObjectsSortMode.None);
					
					// Create new instance if one doesn't already exist.
					if (instances.Length == 0)
					{
						// Need to create a new GameObject to attach the singleton to.
						var singletonObject = new GameObject();
						m_Instance = singletonObject.AddComponent<T>();
						singletonObject.name = typeof(T).ToString() + " (Singleton)";
					}
					else
					{
						m_Instance = instances[0] as T;
					}
					DontDestroyOnLoad(m_Instance);
				}

				return m_Instance;
			}
		}
	}

	protected virtual void OnApplicationQuit()
	{
		m_ShuttingDown = true;
	}


	protected virtual void OnDestroy()
	{
		m_ShuttingDown = true;
	}
}