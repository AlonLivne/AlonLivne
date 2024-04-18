using System.Collections.Generic;

public class BaseAnalyticsProvider
{
    public virtual void Initialize() { }

    /// <summary>
    /// Use this method if you need to send event with only one parameter
    /// </summary>
    /// <param name="name">name of the event</param>
    /// <param name="payloadKey">name of the parameter</param>
    /// <param name="payloadValue">value of the paremeter</param>
    public virtual void SendEvent(string name, string payloadKey, object payloadValue) { }

    /// <summary>
    /// Use this method if you need to send event with many parameters
    /// </summary>
    /// <param name="name">name of the event</param>
    /// <param name="payload">dictionary, containing key/value pairs</param>
    public virtual void SendEvent(string name, Dictionary<string, object> payload = null) { }

    public virtual void OptOutAnalytics() { }

    public virtual void OptInAnalytics() { }
}