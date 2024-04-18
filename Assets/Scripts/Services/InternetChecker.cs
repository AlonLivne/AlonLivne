using System;
using System.Collections;
using UnityEngine;

public class InternetChecker : MonoSingleton<InternetChecker>
{
    public static event Action<(bool isInternetAvaliable, NetworkReachability internetReachabilityStatus)> InternetAvailabilityChanged;
    
    [SerializeField]
    private string IP1 = "1.1.1.1"; // Cloudflare
    [SerializeField]
    private string IP2 = "8.8.8.8"; // google-public-dns-a.google.com.
    [SerializeField]
    private string IP3 = "8.8.4.4"; // google-public-dns-b.google.com

    [SerializeField]
    private float CheckDelay = 2f;
    [SerializeField]
    private float MaxResponseTime = 3f;

    private static (bool isInternetAvaliable, NetworkReachability internetReachabilityStatus) _internetStatus =
        (false, NetworkReachability.NotReachable);
    private bool _isRunning = false;

    public static (bool isInternetAvaliable, NetworkReachability internetReachabilityStatus) InternetStatus
    {
        get => _internetStatus;
        private set
        {
            if (value.internetReachabilityStatus == _internetStatus.internetReachabilityStatus &&
                value.isInternetAvaliable == _internetStatus.isInternetAvaliable)
            {
                return;
            }
            
            _internetStatus = value;
            InternetAvailabilityChanged?.Invoke(value);
        }
    }

    public void Initialize()
    {
        if (!_isRunning)
            StartCoroutine(CheckConnection());
    }

    private void Update()
    {
        if (!_isRunning)
            StartCoroutine(CheckConnection());
    }
    
    private IEnumerator CheckConnection()
    {
        _isRunning = true;
        var success = false;

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            var ping1 = new Ping(IP1);
            var ping2 = new Ping(IP2);
            var ping3 = new Ping(IP3);
            var exitTime = Time.unscaledTime + MaxResponseTime;
            yield return null;

            while (Time.unscaledTime < exitTime)
            {
                if (ping1.isDone || ping2.isDone || ping3.isDone)
                {
                    success = true;
                    break;
                }
                
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        InternetStatus = (success, Application.internetReachability);

        yield return new WaitForSecondsRealtime(CheckDelay);

        _isRunning = false;
    }
}