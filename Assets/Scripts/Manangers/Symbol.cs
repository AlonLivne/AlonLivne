using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Symbol : IDisposable
{
    private SymbolView _view;
    private SymbolData _data;
    public int Id => _data.SymbolId;
    
    public void ReceiveNewParent(MOATile newParent, Action outOfBoardCallback = null)
    {
        ReceiveNewParent(newParent.Transform, outOfBoardCallback : outOfBoardCallback);
    }

    public void Hide()
    {
        _view.gameObject.SetActive(false);
    }

    public void Show()
    {
        _view.gameObject.SetActive(true);
    }
    
    public void ReceiveNewParent(Transform parentTransform, bool withTween = false, Action outOfBoardCallback = null)
    {
        
        _view.ReceiveNewParent(parentTransform, withTween, ()=>
        {
            outOfBoardCallback?.Invoke();
            EventManager.SymbolMovementOver(this);
        });
    }
    
    public (float, float) GetSize()
    {
        return _view.GetSize();
    }

    public Symbol(SymbolView view, SymbolData data)
    {
        _view = view;
        _data = data;
    }

    public void Dispose()
    {
        _view.OnBreakAnimFinished -= Dispose;
        Debug.Log($"Destroyng symbol {_data.SymbolId}");
        MonoBehaviour.Destroy(_view.gameObject);
        _data = null;
    }

    public void Break(BreakType breakType, Action outOfBoardCallBack)
    {
        _view.Break(breakType);
        //_view.OnBreakAnimFinished += Dispose;
        
        _view.OnSymbolOutOfBoard += outOfBoardCallBack;
        _view.OnSymbolOutOfBoard += Dispose;;
        //Dispose();
    }
}
