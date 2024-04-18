using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

public class RemoteConfigProvider : BaseRemoteConfigProvider
{
    private float _fetchRequestedTime;
    private string _lastFetchFailureReason = null;

    /// <summary>
    /// This function cause allocations, so cache the result 
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>string or null, if the key was not found</returns>
    public override string GetParameter(string key)
    {
        try
        {
            var param = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
            if (String.IsNullOrEmpty(param))
            {
                if (_settings.Parameters.ContainsKey(key))
                {
                    param = _settings.Parameters[key].ToString();
                }
                else
                {
                    Debug.LogError($"RemoteConfigProvider: You are trying to get not assigned parameter. Key = {key}");
                    param = null;
                }
            }

            return param;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return null;
    }

    public override void Initialize()
    {
        _settings = new RemoteConfigSettings();
        // We replace Firebase defaults with our custom implementation to be able to add parameters from separate modules or after remove config initialization  
        /*FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(_settings.Parameters).ContinueWithOnMainThread(task => { });*/
        _fetchRequestedTime = Time.unscaledTime;
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(CompleteFetch);
    }

    private void CompleteFetch(Task task)
    {
        ConfigInfo info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                break;
            case LastFetchStatus.Failure:
                _lastFetchFailureReason = info.LastFetchFailureReason.ToString();
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error:
                        Debug.LogFormat("RemoteConfigProvider: FireBaseRemoteConfig: Fetch failed <color=red>FetchFailureReason.Error</color>");
                        break;
                    case FetchFailureReason.Throttled:
                        Debug.LogFormat("RemoteConfigProvider: FireBaseRemoteConfig: Fetch failed <color=red>FetchFailureReason.Throttled</color>");
                        break;
                }
                break;
            case LastFetchStatus.Pending:
                Debug.LogFormat("RemoteConfigProvider: FireBaseRemoteConfig: <color=red>LastFetchStatus.Pending</color>");
                break;
        }

        FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(CompleteActivate);
    }

    private void CompleteActivate(Task<bool> task)
    {
        ConfigInfo info = FirebaseRemoteConfig.DefaultInstance.Info;
        //AnalyticsEventsDispatcher.RemoteConfigFetchComplete(info.LastFetchStatus.ToString(), Time.unscaledTime - _fetchRequestedTime, _lastFetchFailureReason);

        InvokeFetchCompleted();

        // We use it as experiment activation event
        //if (info.LastFetchStatus == LastFetchStatus.Success)
            //ServiceProvider.Analytics.SendEvent(AnalyticsEventsDispatcher.REMOTE_CONFIG_SUCCESSFUL_FETCH);
    }
}