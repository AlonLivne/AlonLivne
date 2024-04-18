using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class RemoteConfigSettings
{
    private Dictionary<string, object> _parametersDictionary;

    public RemoteConfigSettings()
    {
        InitParametersDictionary();
    }

    //We use Dictionary<string, object> for consistency reasons, FirebaseRemoteConfig.SetDefaults accept it as a parameter
    public Dictionary<string, object> Parameters
    {
        get
        {
            if (_parametersDictionary == null)
                InitParametersDictionary();

            return _parametersDictionary;
        }
    }

    private void InitParametersDictionary()
    {
        // This dictionary contains default values (which will be used in case remote are not reachable for some reason)
        _parametersDictionary = new Dictionary<string, object>()
        {
            //{ GlobalConstants.REMOTE_CONFIG_INTERSTITIAL_PATTERN, "2;0" },
            //{ GlobalConstants.REMOTE_CONFIG_MIN_INTER_DELAY, "30" }
        };

        /*
        var xmlText = Resources.Load<TextAsset>("remote_config_defaults");
        XDocument xmlDoc = XDocument.Parse(xmlText.text);
        var entries = xmlDoc.Root.Elements();
        _parametersDictionary = entries.ToDictionary(n => n.Element("key").Value, n => n.Element("value").Value as object);
        */
        
    }

    public void AddParameter(string key, object value)
    {
        if (_parametersDictionary == null)
            InitParametersDictionary();

        if (!_parametersDictionary.ContainsKey(key))
            _parametersDictionary.Add(key, value);
        else
            Debug.LogError($"RemoteConfigSettings: You are trying to add parameter with \"{key}\" key, but it already exists!");
    }

    public void AddParameters(Dictionary<string, object> paramsDictionary)
    {
        if (_parametersDictionary == null)
            InitParametersDictionary();

        foreach (KeyValuePair<string, object> param in paramsDictionary)
        {
            if (!_parametersDictionary.ContainsKey(param.Key))
                _parametersDictionary.Add(param.Key, param.Value);
            else
                Debug.LogError($"RemoteConfigSettings: You are trying to add parameter with \"{param.Key}\" key, but it already exists!");
        }
    }
}