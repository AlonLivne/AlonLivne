using System;

public interface ICueAnimator:IAnimator
{
    public void OnAnimationCue<T>(T cue);
    public void RegisterToAnimationCue(Action<string> action); 
    public void DeregisterToAnimationCue(Action<string> action);
}


