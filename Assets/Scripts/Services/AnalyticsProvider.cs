using System.Collections.Generic;
using System.Text;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsProvider: BaseAnalyticsProvider
{
    private struct PendingEventData
    {
        public readonly string Name;
        public readonly Dictionary<string, object> Payload;

        public PendingEventData(string name, Dictionary<string, object> payload)
        {
            Name = name;
            Payload = payload == null ? null : new Dictionary<string, object>(payload);
        }
    }

    private Stack<PendingEventData> _commonPendingEvents = new Stack<PendingEventData>();
    private Stack<PendingEventData> _firebasePendingEvents = new Stack<PendingEventData>();

    // We keep this dictionary to avoid allocations, caused by creation of parameters dictionary every SendEvent call
    private Dictionary<string, object> _payloadDictionary = new Dictionary<string, object>(2);
    private StringBuilder _stringBuilder = new StringBuilder(100);

    public override void Initialize()
    {
        //InternetChecker.InternetAvailabilityChanged += OnInternetAvailabilityChanged;
        ServiceProvider.FirebaseInitializer.FirebaseInitializationFinished += OnFirebaseInitializationFinished;
    }

    private void OnFirebaseInitializationFinished(bool success)
    {
        if (success)
        {
            SendPendingFirebaseEvents();
        }
    }

    private void OnInternetAvailabilityChanged((bool internetAvailable, NetworkReachability reachability) status)
    {
        if (status.internetAvailable)
        {
            SendPendingCommonEvents();
            if (ServiceProvider.FirebaseInitializer.FirebaseIsReady)
                SendPendingFirebaseEvents();
        }
    }


    /// <summary>
    /// Use this method if you need to send event with only one parameter
    /// </summary>
    /// <param name="name">name of the event</param>
    /// <param name="payloadKey">name of the parameter</param>
    /// <param name="payloadValue">value of the paremeter</param>
    public override void SendEvent(string name, string payloadKey, object payloadValue)
    {
        name = name.Replace(" ", string.Empty);

        if (!string.IsNullOrEmpty(payloadKey) && payloadValue != null)
        {
            _payloadDictionary.Clear();
            _payloadDictionary.Add(payloadKey, payloadValue);
            SendEvent(name, _payloadDictionary);
        }
        else
            SendEvent(name, null);
    }

    /// <summary>
    /// Use this method if you need to send event with many parameters
    /// </summary>
    /// <param name="name">name of the event</param>
    /// <param name="payload">dictionary, containing key/value pairs</param>
    public override void SendEvent(string name, Dictionary<string, object> payload = null)
    {
        // Debug log the event with all the parameters for development builds only
        if (Debug.isDebugBuild)
        {
            if (payload == null)
            {
                Debug.Log($"AnalyticsProvider: SendEvent \"{name}\"");
            }
            else
            {
                _stringBuilder.Clear();
                _stringBuilder.AppendLine($"AnalyticsProvider: SendEvent \"{name}\"");
                foreach (var param in payload)
                    _stringBuilder.AppendLine($"[{param.Key} : {param.Value.ToString()}]");
                Debug.Log(_stringBuilder.ToString());
            }
        }

        /*
        if (!InternetChecker.InternetStatus.isInternetAvaliable)
        {
            _commonPendingEvents.Push(new PendingEventData(name, payload));
            return;
        }*/
        SendEventInternal(name, payload);
    }

    private void SendEventInternal(string name, Dictionary<string, object> payload)
    {
        name = name.Replace(" ", string.Empty);

        // Firebase
        if (ServiceProvider.FirebaseInitializer.FirebaseIsReady)
            SendEventToFirebase(name, payload);
        else
            _firebasePendingEvents.Push(new PendingEventData(name, payload));

        // UnityAnalytics
        if (payload == null)
            Analytics.CustomEvent(name);
        else
            Analytics.CustomEvent(name, payload);
    }

    private void SendEventToFirebase(string name, Dictionary<string, object> payload = null)
    {
        if (payload == null)
        {
            FirebaseAnalytics.LogEvent(name);
        }
        else
        {
            Parameter[] firebaseParams = new Parameter[payload.Count];
            int currIndex = 0;
            foreach (KeyValuePair<string, object> pair in payload)
            {
                firebaseParams[currIndex] = new Parameter(pair.Key, pair.Value.ToString());
                currIndex++;
            }

            FirebaseAnalytics.LogEvent(name, firebaseParams);
        }
    }

    private void SendPendingCommonEvents()
    {
        while (_commonPendingEvents.Count > 0)
        {
            var eventData = _commonPendingEvents.Pop();
            SendEventInternal(eventData.Name, eventData.Payload);
        }
    }

    private void SendPendingFirebaseEvents()
    {
        while (_firebasePendingEvents.Count > 0)
        {
            var eventData = _firebasePendingEvents.Pop();
            SendEventToFirebase(eventData.Name, eventData.Payload);
        }
    }

    private static void SetUserProperty(string key, string value)
    {
        FirebaseAnalytics.SetUserProperty(key, value);
    }


    // Uncomment this attribute to make Unity Analytics disabled from start
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void DisableUnityAnalyticsOnStartup()
    {
        Analytics.enabled = false;
        Analytics.deviceStatsEnabled = false;
        Analytics.initializeOnStartup = false;
        Analytics.limitUserTracking = true;
        PerformanceReporting.enabled = false;
    }

    public override void OptOutAnalytics()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(false);
        Crashlytics.IsCrashlyticsCollectionEnabled = false;

        // Unity 
        Analytics.enabled = false;
        Analytics.deviceStatsEnabled = false;
        Analytics.initializeOnStartup = false;
        Analytics.limitUserTracking = true;
        PerformanceReporting.enabled = false;
    }

    public override void OptInAnalytics()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        Crashlytics.IsCrashlyticsCollectionEnabled = true;

        // Unity 
        Analytics.enabled = true;
        Analytics.deviceStatsEnabled = true;
        Analytics.initializeOnStartup = true;
        Analytics.limitUserTracking = false;
        PerformanceReporting.enabled = true;
        Analytics.ResumeInitialization();
    }
}