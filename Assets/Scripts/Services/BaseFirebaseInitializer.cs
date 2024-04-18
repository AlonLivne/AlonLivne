using System;
using UnityEngine;

public class BaseFirebaseInitializer
{
    public enum FirebaseInitializationStatus
    {
        NotInitialized = 0,
        Initialized = 1,
        Error = 2
    }

    protected FirebaseInitializationStatus _firebaseStatus = FirebaseInitializationStatus.NotInitialized;

    public event Action<bool> FirebaseInitializationFinished;
    public bool FirebaseIsReady => _firebaseStatus == FirebaseInitializationStatus.Initialized;


    public virtual void Initialize()
    {
        _firebaseStatus = FirebaseInitializationStatus.Initialized;
        Debug.Log("Dummy Firebase Initialization Finished");
        FirebaseInitializationFinished?.Invoke(true);
    }

    protected void InvokeFirebaseInitializationFinished(bool success) => FirebaseInitializationFinished?.Invoke(success);
}