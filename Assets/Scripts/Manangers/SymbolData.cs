using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolData
{
    private int _symbolId;
    private string _id;
    public int SymbolId => _symbolId;

    public SymbolData(FireBaseSymbolData data)
    {
        _symbolId = data.Id;
        _id = data.Name;
    }
    
    public string Id()
    {
        if (string.IsNullOrEmpty(_id))
        {
            AssignId(_symbolId.ToString());
        }

        return _id;
    }

    private void AssignId(string id)
    {
        _id = id;
    }
}

public class SymbolScoringData{

    private int _symbolId;
    public int SymbolId => _symbolId;
    public string Gem;
    public List<int> Values => _values;
    private List<int> _values;
    public SymbolScoringData(FireBaseSymbolData rawData)
    {
        _values = _values;
        Gem = rawData.Name;
        _symbolId = rawData.Id;
    }
}