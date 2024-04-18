using System;

public interface IAnimator
{
    void PlayAnimation(string animation);
    void RegisterToAnimationFinished(Action<string> action);
    void DeregisterToAnimationFinished(Action<string> action);
    void OnAnimationFinished<T>(T anim);
}
