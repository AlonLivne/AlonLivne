using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes a list of items and sets a dictionary using the list. GetKey is used by inheritors to determine the key
/// <typeparam name="T1">Key</typeparam>
/// <typeparam name="T2">Object</typeparam>
/// </summary>
public abstract class ListToDictionaryObject<T1, T2> : ScriptableObject
{
    [SerializeField]
    protected List<T2> _objects = new List<T2>();

    public Dictionary<T1, T2> Objects = new Dictionary<T1, T2>();

    public void OnEnable()
    {
        Deserialize();
    }

    private void OnValidate()
    {
        Deserialize();
    }

    private void Deserialize()
    {
        Objects.Clear();
        for (int i = 0; i < _objects.Count; i++)
        {
            var element = _objects[i];
            if (element != null)
            {
                try
                {
                    Objects.Add(GetKey(element), element);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e, this);
                    continue;
                }
            }
            else
            {
                continue;
            }
        }
    }

    public T2 this[T1 key]
    {
        get
        {
            return Objects[key];
        }
    }

    public virtual bool TryGetValue(T1 key, out T2 value)
    {
        return Objects.TryGetValue(key, out value);
    }

    protected abstract T1 GetKey(T2 value);
}