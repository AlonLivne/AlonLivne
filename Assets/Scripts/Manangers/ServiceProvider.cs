using UnityEngine;

public static class ServiceProvider
{
    private static RemoteConfigProvider _remoteConfig;
    private static BaseFirebaseInitializer _firebaseInitializer;

    private static BaseAnalyticsProvider _analytics;
    //private static ClientNotificationsManager _clientNotificationsManager;
    //private static SceneLoader _sceneLoader;
    //private static GameAddressablesController _addressablesController;

    
    public static RemoteConfigProvider RemoteConfig
    {
        get => _remoteConfig;
        set
        {
            if (_remoteConfig == null)
                _remoteConfig = value;
            else
                Debug.LogError($"You are trying to set {nameof(RemoteConfig)} which is already set!");
        }
    }

    public static BaseFirebaseInitializer FirebaseInitializer
    {
        get => _firebaseInitializer;
        set
        {
            if (_firebaseInitializer == null)
                _firebaseInitializer = value;
            else
                Debug.LogError($"You are trying to set {nameof(FirebaseInitializer)} which is already set!");
        }
    }

    public static BaseAnalyticsProvider Analytics
    {
        get => _analytics;
        set
        {
            if (_analytics == null)
                _analytics = value;
            else
                Debug.LogError($"You are trying to set {nameof(Analytics)} which is already set!");
        }
    }
}

/*
public static ClientNotificationsManager ClientNotificationsManager
{
get => _clientNotificationsManager;
set
{
    if (_clientNotificationsManager == null)
        _clientNotificationsManager = value;
    else
        Debug.LogError($"You are trying to set {nameof(ClientNotificationsManager)} which is already set!");
}
}


public static SceneLoader SceneLoader
{
get => _sceneLoader;
set
{
    if (_sceneLoader == null)
        _sceneLoader = value;
    else
        Debug.LogError($"You are trying to set {nameof(SceneLoader)} which is already set!");
}
}*/
