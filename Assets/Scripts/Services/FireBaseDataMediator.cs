using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class FirebaseDataMediator<TFireBase, TMediated>
{
    public abstract TMediated GetMediatedData(TFireBase rawData);
}

#region Bundles

[Serializable]
public class FirebaseBundleStatusInfo
{
    public string ID;
    public List<string> LoadLabels;
    public List<string> UnloadLabels;
}

[Serializable]
public class FirebaseBundlesLoadingInfo
{
    public List<FirebaseBundleStatusInfo> Bundles;
}

[Serializable]
public class FirebaseBundlesInfo
{
    public FirebaseBundlesLoadingInfo ChaptersInfo;
    public FirebaseBundlesLoadingInfo EventsInfo;
}

[Serializable]
public class FirebaseBundlesScenariosInfo
{
    [Serializable]
    public class ScenarioItem
    {
        public int PlayerLevel;
        public string LoadID;
    }

    public List<ScenarioItem> Items = new List<ScenarioItem>();
}

#endregion


public class FeatureConfig
{
    private string _name;
    private bool _active;
    private int _start;
    private int _end;

    public string Name
    {
        get => _name;
    }    
    
    public bool IsActive
    {
        get => _active;
    }

    public int StartLevel
    {
        get => _start;
    }

    public int EndLevel
    {
        get => _end;
    }
    
    public FeatureConfig(string name, bool active = false, int start = 0, int end = 0, bool isMissionRelated = false)
    {
        this._name = name;
        this._active = active;
        this._start = start;
        this._end = end;
    }
}

[Serializable]
public class FirebaseFeatureConfig
{
    public string name;
    public bool active;
    public int start;
    public int end;
}