using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public static class GameDataHolder
{
    private static List<CardData> cardsBank;
    private static List<FireBaseSymbolData> symbolsBank;
    private static bool _initializationIsCompleted;
    private static Action OnInitializationComplete;
    private static Dictionary<int, SymbolScoringData> _symbolsScoring;
    public static Dictionary<int, string> IdToGem { get;}
    public static readonly int Player1Index = 1;
    public static readonly int Player2Index = 2;
    
    public static bool InitializationIsCompleted
    {
        get { return _initializationIsCompleted; }
    }

    public static void Init()
    {
        var remoteConfigs = ServiceProvider.RemoteConfig;
        InitSymbolBank(remoteConfigs);
        InitCardsBank(remoteConfigs);
        InitSymbolScoring();
        CompleteInitalization();
    }

    private static void InitSymbolScoring()
    {
        _symbolsScoring = new Dictionary<int, SymbolScoringData>();
        foreach (var symbol in symbolsBank)
        {
            var symbolData = GetSymbolScoringData(symbol.Id);
            if (symbolData != null)
            {
                _symbolsScoring[symbol.Id] = symbolData;
            }
        }
    }

    public static Dictionary<int,SymbolScoringData> GetSymbolsScoringData()
    {
        return _symbolsScoring;
    }
    
    public static void RegisterToInitializationCompleteEvent(Action action)
    {
        OnInitializationComplete += action;
    }
    
    private static void CompleteInitalization()
    {
        Debug.Log("GameDataHolder has finished initializing");
        _initializationIsCompleted = true;
        OnInitializationComplete?.Invoke();
    }

    private static void InitSymbolBank(RemoteConfigProvider remoteConfigs)
    {
        var param = remoteConfigs.GetParameter("POCSymbolsBank");
        symbolsBank = JsonUtility.FromJson<ListWrapper<FireBaseSymbolData>>(param).List;
    }

    private static void InitCardsBank(RemoteConfigProvider remoteConfigs)
    {
        string param = remoteConfigs.GetParameter("POCCardsBank");
        cardsBank = JsonUtility.FromJson<ListWrapper<CardData>>(param).List;
    }
    
    public static List<CardData> GetCardsDataByIds(List<int> ids)
    {
        var ans = new List<CardData>();
        foreach (var id in ids)
        {
            ans.Add(GetCardData(id));
        }
        
        return ans;
    }
    
    #region POC Enemy Data
    public static List<CardData> GetEnemyCardData()
    {
        var param = ServiceProvider.RemoteConfig.GetParameter("POCEnemyCards");
        var playerCards = JsonUtility.FromJson<ListWrapper<int>>(param);
        return GetCardsDataByIds(playerCards.List);
    }
    
    public static ShieldData GetEnemyShieldData()
    {
        return GetShieldData("POCEnemyShield");
    }
    #endregion
    
    #region POC Player Data
    
    public static List<CardData> GetPlayerCardData()
    {
        var param = ServiceProvider.RemoteConfig.GetParameter("POCPlayerCards");
        var playerCards = JsonUtility.FromJson<ListWrapper<int>>(param);
        return GetCardsDataByIds(playerCards.List);
    }

    public static ShieldData GetPlayerShieldData()
    {
        return GetShieldData("POCPlayerShield");
    }
    
    #endregion
    
    public static ShieldData GetShieldData(string key)
    {
        var param = ServiceProvider.RemoteConfig.GetParameter(key);
        var shieldInfo = JsonUtility.FromJson<ShieldData>(param);
        return shieldInfo;
    }
    
    public static CardData GetCardData(int id)
    {
        if (cardsBank == null)
        {
            throw new Exception("tried to access cardsBank before it was initialized!");
        }
        
        foreach (var cardData in cardsBank)
        {
            if (cardData.Id == id)
            {
                return cardData;
            }
        }
        Debug.LogError($"tried to get a card without an id from bank (id {id}), need to decide on default");
        throw new Exception("tried to get a card without an id from bank, need to decide on default");
    }

    public static TurnData GetTurnsData()
    {
        var remoteConfigs = ServiceProvider.RemoteConfig;
        string param = remoteConfigs.GetParameter("POCTurnsConfig");
        var ans = JsonUtility.FromJson<TurnData>(param);
        return ans;
    }

    public static BoardConfig GetBoardConfigs()
    {
        var remoteConfigs = ServiceProvider.RemoteConfig;
        var param = remoteConfigs.GetParameter("POCBasicBoardConfigs");
        var boardRawData = JsonUtility.FromJson<FirebaseBoardConfig>(param);
        var mediator = new BoardConfigMediator();
        return mediator.GetMediatedData(boardRawData);
    }

    public static SymbolData GetSymbolDataById(int id)
    {
        foreach (var symbolData in symbolsBank)
        {
            if (symbolData.Id == id)
            {
                return new SymbolData(symbolData);
            }
        }
        Debug.LogError($"tried to get a symbol without an id from bank (id {id}), need to decide on default");
        throw new Exception("tried to get a card symbol an id from bank, need to decide on default");
    }
    
    public static int GetRandomValidSymbolId()
    {
        var config = GetBoardConfigs();
        var randomIndex = Random.Range(0, config.SymbolsIds.Count);
        return config.SymbolsIds[randomIndex];
    }    
    
    private static SymbolScoringData GetSymbolScoringData(int id)
    {
        foreach (var symbolData in symbolsBank)
        {
            if (symbolData.Id == id)
            {
                return new SymbolScoringData(symbolData);
            }
        }
        Debug.LogError($"tried to get a card without an id from bank (id {id}), need to decide on default");
        throw new Exception("tried to get a card without an id from bank, need to decide on default");
    }
    
}

[Serializable]
public struct GemModifier
{
    [FormerlySerializedAs("Gem")] public int Id;
    public float Combo3;
    public float Combo4;
    public float Combo5;
}

[Serializable]
public struct CardData
{
    public int Id;
    public string Name;
    public int HP;
    public int Lvl;
    public List<GemModifier> PowerUps;
}

[Serializable]
public struct ListWrapper<T>
{
    public List<T> List;
}


[Serializable]
public struct FireBaseSymbolData
{ 
    public int Id; 
    public string Name; 
    public List<int> values;
}

[Serializable]
public struct TurnData
{ 
    public float TurnLength;
    public int ActionsPerTurn;
}

[Serializable]
public struct ShieldData
{ 
    public int ShieldLvl;
    public int ShieldHp;
}