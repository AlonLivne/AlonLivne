using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOATile
{
  private Symbol _currentSymbol;
  public Symbol CurrentSymbol{
    get
    {
      return _currentSymbol;
    }
  }
  
  private TileData _data;
  public int? CurrentSymbolId
  {
    get
    {
      if (_currentSymbol != null)
        return _currentSymbol.Id;
      return null;
    }
  }

  public Transform Transform => _transform;
  private Transform _transform;
  
  public MOATile(TileData data, Transform transform)
  {
    _data = data;
    _transform = transform;
  }

  public void ReleaseSymbol()
  {
    _currentSymbol = null;
  }

  public void AdopSymbol(Symbol symbol, Action outOfBoardCallback = null)
  {
    if (_currentSymbol != null)
    {
      Debug.LogError($"Tile {_transform.name} tried to adopt symbol {symbol.Id} but another symbol is there - {_currentSymbol.Id}");
    }

    _currentSymbol = symbol;
    symbol?.ReceiveNewParent(this, outOfBoardCallback : outOfBoardCallback);
  }
  
  public IEnumerator AdoptSymbolCoroutine(Symbol symbol)
  {
    while (_currentSymbol != null && _currentSymbol != symbol)
    { 
      yield return null;
    }
    _currentSymbol = symbol;
    
    symbol?.ReceiveNewParent(this);
  }
  
  public void BreakSymbol(BreakType breakType, Action breakingDoneCallback = null)
  {
    if (_currentSymbol == null)
    {
      return;
    }

    if (breakType == BreakType.Streak && breakingDoneCallback == null)
    {
      breakingDoneCallback = () =>
      {
        EventManager.SymbolBreakingOver(this);
      };
    }
    _currentSymbol.Break(breakType, breakingDoneCallback);

    _currentSymbol = null;
  }
}

public enum BreakType
{
  Streak,
  Left,
  Right,
  Down
}