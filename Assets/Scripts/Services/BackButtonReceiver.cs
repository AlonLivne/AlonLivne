using UnityEngine;

public class BackButtonReceiver : MonoBehaviour
{
    /// <summary>
    /// Until we properly define where this sits (in the "don't destroy" game systems, obviously)
    /// we need to make sure we only have one instance. And even afterwards.
    /// Nothing should address this instance directly, so the instance can be private.
    /// In best scenario, this should be a static class which registers to the UpdateService after the service had been created.
    /// </summary>

    private static BackButtonReceiver _instance;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);            
            return;
        }

        _instance = this;
    }

    //TODO - delete this class if we use a more clever input system...
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //BackButtonExecutor.OnBackButtonClick();
        }
    }
}
