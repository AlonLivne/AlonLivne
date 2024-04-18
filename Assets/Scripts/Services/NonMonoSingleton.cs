using System;
using UnityEngine;

public abstract class NonMonoSingleton<T> where T : NonMonoSingleton<T>
{
    private static T _instance;

    public static T Instance => _instance;
    public static bool HasInstance => _instance != null;

    protected void AssignInstance(T instance)
    {
        if (_instance != null)
        {
            Debug.LogError($"[NonMonoSingleton]: {typeof(T)} already have an instance, but you are trying to assign another one!");
            return;
        }
        _instance = instance;
    }

    public virtual void Dispose()
    {
        _instance = null;
    }
}