using UnityEngine;

public class GraphicsController
{
    private static int[] _capFPSArray = new[] {30, 60,-1};
    
    public void UpdateFrameRate(int capIndex)
    {
        capIndex = Mathf.Clamp(capIndex, 0, _capFPSArray.Length - 1);
        
        var cap = _capFPSArray[capIndex];
        if (cap <= 0)
        {
            cap = Screen.currentResolution.refreshRate;
        }

        Application.targetFrameRate = cap;
    }
}
