using System;using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CalloutsAnimatorReporter : MonoBehaviour, ICueAnimator
{
    [SerializeField] private Animator _animator;
    public Action<string> AnimationFinished;
    public Action<string> CueFired;

    //private Dictionary<string, string> TODO - get animations triggers from scriptable object
    public void PlayAnimation(string animation)
    {
        _animator.SetTrigger(animation);
    }

    public void RegisterToAnimationFinished(Action<string> action)
    {
        AnimationFinished += action;
    }
        
    public void DeregisterToAnimationFinished(Action<string> action)
    {
        AnimationFinished -= action;
    }    
    
    public void RegisterToAnimationCue(Action<string> action)
    {
        CueFired += action;
    }
        
    public void DeregisterToAnimationCue(Action<string> action)
    {
        CueFired -= action;
    }

    public void OnAnimationFinished<CalloutsAnimations>(CalloutsAnimations anim)
    {
        AnimationFinished?.Invoke(anim.ToString());
    }        
    
    public void OnAnimationFinished(string animName)
    {
        Debug.Log($"Finishing animation {animName}");
        AnimationFinished?.Invoke(animName);
    }    
    
    public void OnAnimationCue<CalloutsAnimations>(CalloutsAnimations anim)
    {
        CueFired?.Invoke(anim.ToString());
    }    
    
    public void OnAnimationCue(string animName)
    {
        CueFired?.Invoke(animName);
    }
}

public enum CalloutsAnimations{
    GetReady,
    GetReadyExit,
    MatchOver,
    MatchOverExit,
    OpponentsTurn,
    YourTurn
}


