using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    [SerializeField]
    private bool _dontDestroyOnLoad = false;

    private static T _instance = null;
    private static object lockObject = new object();

    /// <summary>
    /// Try to get Instance.
    /// If not found, will NOT create one and log the error.
    /// </summary>
    public static T Instance
    {
        get
        {
            // Instance requiered for the first time, we look for it
            if (_instance == null)
            {
                lock (lockObject)
                {
                    //Search for existing objects
                    Object[] result = FindObjectsOfType(typeof(T));

                    if (result.Length > 0)
                    {
                        _instance = result[0] as T;
                        if (result.Length > 1)
                            Debug.LogError($"[MonoSingleton] Something went really wrong - there should never be more than 1 {typeof(T)}!");
                    }
                    // Object not found, inform that we are trying to acquire not created instance
                    else
                    {
                        Debug.LogError($"[MonoSingleton] You are trying to get not created instance of {typeof(T)}");
                    }
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Check if instance is not null.
    /// Result is NOT defined if checked from the Awake event function!
    /// </summary>
    public static bool HasInstance => _instance != null;

    /// <summary>
    /// Try to find instance, if not found create one
    /// </summary>
    /// <param name="dontDestroyOnLoad">Make object DontDestroyOnLoad</param>
    /// <returns></returns>
    public static T GetOrCreateInstance(bool dontDestroyOnLoad = false)
    {
        if (_instance == null)
        {
            lock (lockObject)
            {
                Object[] result = FindObjectsOfType(typeof(T));

                if (result.Length > 0)
                {
                    _instance = result[0] as T;

                    if (result.Length > 1)
                        Debug.LogError($"[MonoSingleton]: Something went really wrong - there should never be more than 1 {typeof(T)}!");
                }
                else
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }

                if (dontDestroyOnLoad)
                    _instance.ApplyDontDestroyOnLoad();
            }
        }

        return _instance;
    }


    /// <summary>
    /// This function is called from Awake, put all the initializations you need here, as you would do in Awake.
    /// </summary>
    protected virtual void Init() { }

    /// <summary>
    /// If no other monobehaviour request the instance in an Awake function executing before this one, no need to search the object.
    /// </summary>
    private void Awake()
    {
        AssignItselfAsInstance();
    }

    /// <summary>
    /// Assign itself as _instance if _instance is null, otherwise destroy itself 
    /// </summary>
    void AssignItselfAsInstance()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (_instance != null)
            {
                if (_instance._dontDestroyOnLoad)
                    ApplyDontDestroyOnLoad();
                _instance.Init();
            }
        }
        else if (_instance == this as T)
        {
            if (_instance._dontDestroyOnLoad)
                ApplyDontDestroyOnLoad();
            _instance.Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void ApplyDontDestroyOnLoad()
    {
        transform.parent = null;
        DontDestroyOnLoad(_instance.gameObject);
    }

    // Make sure the instance isn't referenced anymore when the object is destroyed, just in case.
    protected virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            _instance = null;
        }
    }

    // Make sure the instance isn't referenced anymore when the user quit, just in case.
    private void OnApplicationQuit()
    {
        _instance = null;
    }
}