using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameInitializationMode
{
    Default = 0,
    EditorDefault = 1,
    Dummies = 2
};

public class GameInitializer : MonoBehaviour
{
    private const float FETCH_COMPLETE_TIMEOUT = 5f;

    [SerializeField] private GameInitializationMode _initializationMode = GameInitializationMode.Default;
    [SerializeField] private bool _forceSelectedInitializationMode = false;

    public GameInitializationMode InitializationMode
    {
        get => _initializationMode;
        set => _initializationMode = value;
    }

    public bool ForceSelectedInitializationMode
    {
        get => _forceSelectedInitializationMode;
        set => _forceSelectedInitializationMode = value;
    }

    private void Start()
    {
        ClickBlocker.BlockClicks();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        CreateInstances();
        StartCoroutine(InitCoroutine());
        //ScreenSizeHandler.SyncScreenResolution(); //TODO check if this is responsible for flickering screen
    }

    private IEnumerator InitCoroutine()
    {
        // Internet Checker
        InternetChecker.Instance.Initialize();

        // Firebase Initializer        
        ServiceProvider.FirebaseInitializer.FirebaseInitializationFinished += (b) =>
        {
            if (b)
            {
                Debug.LogError($"Firebase initialization succeeded!");
            }
            else
            {
                Debug.LogError($"Firebase Failed to initialize!");
            }
        };
        ServiceProvider.FirebaseInitializer.Initialize();

        // Storage 
        //Storage.Initialize();
        
        // Analytics
        ServiceProvider.Analytics.Initialize();

        // Wait for Firebase initialization to complete
        while (!ServiceProvider.FirebaseInitializer.FirebaseIsReady)
        {
            yield return null;
        }

        // FirebaseAnalytics.SetUserProperty   |   If we need to set UserProperties important to do it here, before fetching remote config!

        // Remote Config
        // StartCoroutine(UnblockPreloader()); // With current implementation this call is not relevant
        ServiceProvider.RemoteConfig.FetchCompleted += () =>
        {
           
            GameDataHolder.Init();
            
            //Addressables Init
            //ServiceProvider.AddressablesController.Init(GameDataHolder.BundlesInfo, GameDataHolder.BundlesScenariosInfo, TryToLoadScene);
        };

        ServiceProvider.RemoteConfig.Initialize();
        
        //SoundsInit
        
        //TODO check if this is the proper place
        //GameDataHolder

        // Load Next Scene
        yield return new WaitForEndOfFrame();
    }

    private void CreateInstances()
    {
        // Automatically change Default initialization mode to EditorDefault for the Editor Application
        if (!_forceSelectedInitializationMode && Application.isEditor && _initializationMode == GameInitializationMode.Default)
            _initializationMode = GameInitializationMode.EditorDefault;

        switch (_initializationMode)
        {
            case GameInitializationMode.Default:
                //ServiceProvider.SceneLoader = _sceneLoader;
                ServiceProvider.FirebaseInitializer = new FirebaseInitializer();
                ServiceProvider.RemoteConfig = new RemoteConfigProvider();
                ServiceProvider.Analytics = new AnalyticsProvider();
                //ServiceProvider.ClientNotificationsManager = new ClientNotificationsManager();
                //ServiceProvider.AdsSystemController = _adsSystemController;
                //ServiceProvider.SoundController = new SoundController();
                //ServiceProvider.GraphicsController = new GraphicsController();
                
                //ServiceProvider.AddressablesController = _addressablesController;
                break;
            case GameInitializationMode.EditorDefault:
                //ServiceProvider.SceneLoader = _sceneLoader;
                ServiceProvider.FirebaseInitializer = new FirebaseInitializer();
                ServiceProvider.RemoteConfig = new RemoteConfigProvider();
                ServiceProvider.Analytics = new AnalyticsProvider();
                //ServiceProvider.ClientNotificationsManager = new ClientNotificationsManager();
                //ServiceProvider.AdsSystemController = _adsSystemController;
                //ServiceProvider.SoundController = new SoundController();
                //ServiceProvider.GraphicsController = new GraphicsController();
                
                //ServiceProvider.AddressablesController = _addressablesController;
                break;
            case GameInitializationMode.Dummies: 
                //ServiceProvider.SceneLoader = _sceneLoader;
                ServiceProvider.FirebaseInitializer = new BaseFirebaseInitializer();
                ServiceProvider.RemoteConfig = new RemoteConfigProvider();
                ServiceProvider.Analytics = new BaseAnalyticsProvider();
                //ServiceProvider.ClientNotificationsManager = new ClientNotificationsManager();
                //ServiceProvider.AdsSystemController = _adsSystemController;
                //ServiceProvider.SoundController = new SoundController();
                //ServiceProvider.GraphicsController = new GraphicsController();
               
                //ServiceProvider.AddressablesController = _addressablesController;
                break;
        }

        if (!InternetChecker.HasInstance)
            InternetChecker.GetOrCreateInstance(true);

        //_preloader ??= Preloader.GetOrCreateInstance();
    }

    private void InitSounds()
    {
        //var updateSoundSettingsAction = new Action<bool>(b => ServiceProvider.SoundController.UpdateSettings(
            //GameDataHolder.IsMusicOn,
            //GameDataHolder.IsSFXOn));
        
        //updateSoundSettingsAction?.Invoke(true);

        //GameDataHolder.GetPlayerSettings.IsSFXOn.ValueChanged += updateSoundSettingsAction;
        //GameDataHolder.GetPlayerSettings.IsMusicOn.ValueChanged += updateSoundSettingsAction;
    }

    private void InitGraphics()
    {
        //ServiceProvider.GraphicsController.UpdateFrameRate(GameDataHolder.FPSCap);
        //GameDataHolder.GetPlayerSettings.FPSCap.ValueChanged += ServiceProvider.GraphicsController.UpdateFrameRate;
    }
}