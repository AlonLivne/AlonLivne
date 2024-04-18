using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public abstract class MonoObjectCacher<T>:MonoBehaviour
{
    [SerializeField]
    protected GameObject _objectPrefab;

    private Thread _myThread;
    private Queue<T> _objectQueue;

    private void Awake()
    {
        ASyncEnqueue();
    }

    public virtual T GetObject()
    {
        HandleEnqueueing();
        return _objectQueue.Dequeue();
    }

    private void HandleEnqueueing()
    {
        if (_objectQueue.Peek() != null)
        {
            ASyncEnqueue();
        }
        else
        {
            ImmediateEnqueue();
        }
    }
    
    private void ASyncEnqueue()
    {
        Thread thread = new Thread(ImmediateEnqueue);
        thread.Start();
    }

    private void ImmediateEnqueue()
    {
        T objectToEnqueue = CreateObject();
        
        if (objectToEnqueue != null)
        {
            _objectQueue.Enqueue(objectToEnqueue);
            return;
        }

        Debug.LogWarning("Trying to cache an object with an invalid prefab! Missing component!");
    }

    private T CreateObject()
    {
        GameObject cachedObject = Instantiate(_objectPrefab, transform);
        T typedObject = cachedObject.GetComponent<T>();

        return typedObject;
    }
}
