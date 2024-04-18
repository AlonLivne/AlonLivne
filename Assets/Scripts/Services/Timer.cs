using System;
using UnityEngine;

public class Timer : Updatee
{
    private float seconds;
    private Action onTimerComplete;
    private Action<object> onTimerCompleteWithParameter;
    private object timerCompleteParameter;

    //TODO inject the UpdateService from a class initter 
    public void Init()
    {
        if (_updateService == null)
        {
            _updateService = UpdateService.Instance;
        }
    }
    
    public float GetRemainingSeconds()
    {
        return seconds;
    }
    
    public Timer(float timerInSeconds, Action callback)
    {
        //TODO delete this line once injection is properly handled
        StartTimer(timerInSeconds, callback);
    }

    public Timer(float timerInSeconds, Action<object> callback, object parameter)
    {
        //TODO delete this line once injection is properly handled
        StartTimerWithParameter(timerInSeconds, callback, parameter);
    }

    public void StartTimer(float timeInSeconds, Action callback)
    {
        StartTimerInternal(timeInSeconds, callback, null, null);
    }

    public void StartTimerWithParameter(float timeInSeconds, Action<object> callback, object parameter)
    {
        StartTimerInternal(timeInSeconds, null, callback, parameter);
    }

    private void StartTimerInternal(float timeInSeconds, Action callback, Action<object> callbackWithParameter, object parameter)
    {
        seconds = timeInSeconds;
        onTimerComplete = callback;
        onTimerCompleteWithParameter = callbackWithParameter;
        timerCompleteParameter = parameter;
        _updateService.VeryoUpdate += Tick;
    }

    private void HandleTimerComplete()
    {
        onTimerComplete?.Invoke();
        onTimerCompleteWithParameter?.Invoke(timerCompleteParameter);
        OnDestroy();
    }

    public void KillTimer()
    {
        OnDestroy();
    }
    
    protected override void OnDestroy()
    {
        onTimerComplete = null;
        onTimerCompleteWithParameter = null;
        timerCompleteParameter = null;
        base.OnDestroy();
    }

    protected override void Tick()
    {
        if (seconds < 0)
        {
            HandleTimerComplete();
            return;
        }

        seconds -= Time.deltaTime;    
    }
    
}