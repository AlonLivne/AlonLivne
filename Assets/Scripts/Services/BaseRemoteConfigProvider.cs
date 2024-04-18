using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseRemoteConfigProvider
{
    [SerializeField]
    protected RemoteConfigSettings _settings;
    public event Action FetchCompleted;
    protected bool _fetchIsCompleted = false;

    /// <summary>
    /// Check if fetching operation completed
    /// If fetch operation failed for some reason - this flag still will be true and values will be taken from cache
    /// </summary>
    public bool FetchIsCompleted
    {
        get { return _fetchIsCompleted; }
    }


    /// <summary>
    /// This function create allocations, so cache the result 
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>string or null, if the key was not found</returns>
    public virtual string GetParameter(string key)
    {
        if (_settings.Parameters.ContainsKey(key))
        {
            return (string) _settings.Parameters[key];
        }
        return null;
    }

    public virtual void Initialize()
    {
        _settings = new RemoteConfigSettings();
        //Imitating FetchComplete for Dummy Provider
        InvokeFetchCompleted();
    }

    protected void InvokeFetchCompleted()
    {
        _fetchIsCompleted = true;
        FetchCompleted?.Invoke();
    }

    public void AddDefaultParameter(string key, object value)
    {
        _settings.AddParameter(key, value);
    }

    public void AddDefaultParameters(Dictionary<string, object> paramsDictionary)
    {
        _settings.AddParameters(paramsDictionary);
    }
}