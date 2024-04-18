using System;
using UnityEngine;

/// <summary>
/// This class is for updating non-mono classes, to preserve order in the scene.
/// It doesn't have fixedUpdate because likely an object which requires fixed update should be mono.
/// </summary>
public class UpdateService : MonoBehaviour
{
    public Action VeryoUpdate;
    public static UpdateService Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("UpdateService: Trying to assign Instance more than once!");
            return;
        }
        
        Instance = this;
    }

    private void Update()
    {
        VeryoUpdate?.Invoke();
    }
}
