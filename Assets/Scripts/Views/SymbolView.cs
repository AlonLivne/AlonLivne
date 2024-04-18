using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

public class SymbolView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private SpriteRenderer _symbol;
    [SerializeField] private Animator _symbolAnimator;
    private float _tweenTime = 1f;

    public (float, float) GetSize()
    {
        var xOffset = _background.sprite.rect.width / 225;
        var yOffset = _background.sprite.rect.height / 225;

        return (xOffset, yOffset);
    }

    public bool Init(Sprite background, Sprite symbol, Animator animator = null)
    {
        _background.sprite = background;
        _symbol.sprite = symbol;
        if (animator != null)
        {
            _symbolAnimator = animator;
        }

        return true;
    }

    public void ReceiveNewParent(Transform parentTransform, bool withTween = false, Action outOfBoardCallback = null)
    {
        //StartCoroutine(ReceiveNewParentCoroutine(parentTransform));
        transform.SetParent(parentTransform);
        transform.localPosition = new Vector3(0, 0, 0);
        outOfBoardCallback?.Invoke();
    }

    IEnumerator ReceiveNewParentCoroutine(Transform parentTransform)
    {
        transform.SetParent(parentTransform);
        Debug.Log($"Moving symbol {transform.parent.name}");
        var time = _tweenTime;
        var initialPosition = transform.position;
        var z = initialPosition.z;

        while (time > 0)
        {
            time = time - Time.deltaTime;
            var x = initialPosition.x * (time / _tweenTime);
            var y = initialPosition.y * (time / _tweenTime);

            transform.position = new Vector3(x, y, z);
            yield return null;
        }

        transform.position = new Vector3(0, 0, z);
        Debug.Log($"finished moving symbol {transform.parent.name}");
        yield return null;

    }

    public void Break(BreakType breakType)
    {
        var trigger = "breakDown";
        switch (breakType)
        {
            case BreakType.Down:
                trigger = "breakDown";
                break;
            case BreakType.Left:
                trigger = "breakLeft";
                break;
            case BreakType.Right:
                trigger = "breakRight";
                break;

        }

        _symbolAnimator.SetTrigger(trigger);
    }

    public void SymbolOutOfBoard()
    {
        Debug.Log("Symbol out of board - ready to continue");
        OnSymbolOutOfBoard?.Invoke();
        OnSymbolOutOfBoard = null;
    }

    public void BreakAnimationFinished()
    {
        Debug.Log("Break animation finished");
        SymbolOutOfBoard();
        OnBreakAnimFinished?.Invoke();
        OnBreakAnimFinished = null;
    }
    
    public Action OnBreakAnimFinished;
    public Action OnSymbolOutOfBoard;
}
