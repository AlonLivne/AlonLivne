using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InsertionPosition : MonoBehaviour
{
    private static InsertionPosition _lastHoveredInsertionPosition;
    public static InsertionPosition LastHoveredInsertionPosition => _lastHoveredInsertionPosition;
    private (int, int) _coordinates;

    private static Dictionary<(int, int), InsertionPosition> _coordsToInsertionPosition =
        new Dictionary<(int, int), InsertionPosition>();
    [SerializeField] private Animator _animator;
    public (int, int) Coordinates
    {
        get => _coordinates;
        set
        {
            _coordinates = value;
            _coordsToInsertionPosition[_coordinates] = this;
        }
    }

    [SerializeField] private Transform _contained;
    
    private void OnMouseOver()
    {
        if (_lastHoveredInsertionPosition == this)
        {
            return;
        }

        ChangeLastPosition(this);
    }

    public static void ManuallySelectPosition(InsertionPosition position)
    {
        ChangeLastPosition(position);
    }
    
    public static void ManuallySelectPosition((int, int) coords)
    {
        ManuallySelectPosition(_coordsToInsertionPosition[coords]);
    }
    
    private static void ChangeLastPosition(InsertionPosition newPosition)
    {
        _lastHoveredInsertionPosition?.Deselect();
        _lastHoveredInsertionPosition = newPosition;
        _lastHoveredInsertionPosition.Select();
        EventManager.OnInsertionClicked(_lastHoveredInsertionPosition);
    }

    public Transform GetTransform()
    {
        return _contained;
    }
    
    private void Deselect()
    {
        _animator.SetTrigger("deselect");
    }

    private void Select()
    {
        _animator.SetTrigger("select");
    }
}
