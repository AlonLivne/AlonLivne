using System;
using UnityEngine;

public class TurnsAnimatorReporter : MonoBehaviour, IAnimator //TODO turn this into an interface, realise it with Callouts and Turns animator reporters
{
    [SerializeField] private Animator _animator;
    public Action<string> AnimationFinished;

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

    public void OnAnimationFinished(string animName) //TODO find a cleaner way to do this
    {
        AnimationFinished?.Invoke(animName);
    }
    
    public void OnAnimationFinished<TurnsAnimations>(TurnsAnimations anim)
    {
        AnimationFinished?.Invoke(anim.ToString());
    }
}

[Serializable]
public enum TurnsAnimations{
    TurnsReset,
    OpponentTurnEnd,
    OpponentTurnStart,
    YourTurnEnd,
    YourTurnStart
}
