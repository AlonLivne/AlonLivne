using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour
{
    public Action<string> AnimationFinished;

    public void Initialize()
    {


    }
    
    public void QueueSymbol(Symbol symbolToQueue)
    {
        /*
        if (_playersTempSymbol[player] == null)
        {    _playersSymbols.Add(symbolToQueue);
            return;
        }
        _playersTempSymbol[player] = symbolToQueue;
        //TODO place symbol in the right place
        */
    }

    public void AdvanceTurn()
    {
        
    }

    public void OnAnimationFinished(string animation)
    {
        AnimationFinished?.Invoke(animation);
    }
}
