using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBlocker : MonoBehaviour
{
    private static ClickBlocker _clickBlocker;

    private void Awake()
    {
        _clickBlocker = this;
    }

    public static void BlockClicks()
    {
        _clickBlocker?.gameObject.SetActive(true);
    }
    
    public static void UnblockClicks()
    {
        _clickBlocker?.gameObject.SetActive(false);
    }
}
