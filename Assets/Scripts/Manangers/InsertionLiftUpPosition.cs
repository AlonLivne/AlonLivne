using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InsertionLiftUpPosition : MonoBehaviour
{
    private static InsertionLiftUpPosition _lastHoveredLiftUpPosition;
    public static InsertionLiftUpPosition LastHoveredLiftUpPosition => _lastHoveredLiftUpPosition;
    private InsertionPosition _insertionPosition;
    private (int, int) _coordinates;
    public (int, int) Coordinates { get => _coordinates; set => _coordinates = value; }

    public void Init(InsertionPosition insertionPosition)
    {
        _insertionPosition = insertionPosition;
    }

    private void OnMouseDrag()
    {
        //throw new NotImplementedException();
    }
    
    public static void ManuallySelectPosition(InsertionLiftUpPosition position)
    {
        ChangeLastPosition(position);
    }
    
    private void OnMouseOver()
    {
        if (_lastHoveredLiftUpPosition == this)
        {
            return;
        }

        ChangeLastPosition(this);
    }
    
    private static void ChangeLastPosition(InsertionLiftUpPosition newPosition)
    {
        _lastHoveredLiftUpPosition = newPosition;
        Debug.Log($"Mouse over lift up {_lastHoveredLiftUpPosition._coordinates}");
    }
    
    private void OnMouseUp()
    {
        Debug.Log($"Mouse up received lift up {_lastHoveredLiftUpPosition._coordinates}");
        EventManager.OnInsertionLiftUpClicked(this);
    }
}
