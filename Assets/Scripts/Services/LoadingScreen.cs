using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen _loadingScreen;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _loadingScreen = this;
    }

    private void Start()
    {
        GameDataHolder.RegisterToInitializationCompleteEvent(UnblockView);
    }

    public static void BlockView()
    {
        _loadingScreen._animator.SetTrigger("show");
    }
    
    public static void UnblockView()
    {
        _loadingScreen._animator.SetTrigger("hide");
    }
}
