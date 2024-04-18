using System;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializer : BaseFirebaseInitializer
{
    public override void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                _firebaseStatus = FirebaseInitializationStatus.Initialized;
            }
            else
            {
                // Firebase Unity SDK is not safe to use here.
                try
                {
                    _firebaseStatus = FirebaseInitializationStatus.Error;
                    throw new Exception($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            InvokeFirebaseInitializationFinished(_firebaseStatus == FirebaseInitializationStatus.Initialized);
        });
    }
}