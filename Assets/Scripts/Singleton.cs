using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }
}
