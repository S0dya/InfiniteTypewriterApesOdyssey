using UnityEngine;

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public static T I
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Debug.Log(instance + " already exists");
            Destroy(gameObject);
        }
    }
}
