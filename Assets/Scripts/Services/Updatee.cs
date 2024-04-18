using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Updatee
{
    protected static UpdateService _updateService;
    
    protected virtual void Tick()
    {

    }

    protected virtual void OnDestroy()
    {
        _updateService.VeryoUpdate -= Tick;
    }
}
