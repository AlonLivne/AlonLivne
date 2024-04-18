using System;
using UnityEngine;

public class AnimationEventNotifier : MonoBehaviour
{
    public Action<string> AnimationFinished;

    public void Subscribe(Action<string> action)
    {
        AnimationFinished += action;
    }
    
    public void Unsubscribe(Action<string> action)
    {
        AnimationFinished += action;
    }
    
    public void OnAnimationFinished(string animation)
    {
        AnimationFinished?.Invoke(animation);
    }
}
