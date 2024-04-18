
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SymbolFactory 
{
    private SpriteDictionaryObject _symbolsSprites;
    private SpriteDictionaryObject _tileBGSprites;
    private SymbolView _symbolViewPrefab;
    private bool _isInitted;
    public Action InitComplete;

    public SymbolFactory(AssetReference symbolViewPrefab, SpriteDictionaryObject symbolsSprites, SpriteDictionaryObject tileBGSprites, Action callback = null)
    {
        InitComplete += callback;
        _symbolsSprites = symbolsSprites;
        _tileBGSprites = tileBGSprites;
        LoadViewPrefab(symbolViewPrefab);
    }

    private void LoadViewPrefab(AssetReference symbolViewPrefab)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(symbolViewPrefab);
        handle.Completed += SymbolPrefabReady;
    }

    private void FinishInit()
    {
        _isInitted = true;
        InitComplete?.Invoke();
        InitComplete = null;
    }
    
    private void SymbolPrefabReady(AsyncOperationHandle<GameObject> obj) 
    {
        var gobject = obj.Result;
        _symbolViewPrefab = gobject.GetComponent<SymbolView>();
        FinishInit();
    }

    public Symbol GetSymbol(int symbolId, bool withBg = true, bool withSymbol = true)
    {

        var data = GetSymbolData(symbolId);
        var view = GetSymbolView(data, withBg, withSymbol);

        return new Symbol(view, data);
    }
    
    public SymbolView GetSymbolView(SymbolData data, bool withBg = true, bool withSymbol = true)
    {
        return InstantiateView(data.Id());
    }

    public SymbolData GetSymbolData(int symbolId)
    {
        var rawData = GameDataHolder.GetSymbolDataById(symbolId);
        return rawData;
    }

    public Symbol GetRandomSymbol()
    {
        return GetSymbol(GetRandomValidSymbolId());
    }
    
    public SymbolView InstantiateView(string SymbolId, bool withSymbol = true, bool withBackground = true)
    {
        if (!_isInitted)
        {
            Debug.LogError("Trying to create symbol view before factory is initted!");
            //TODO add analytic
            return null;
        }
        Sprite bg = withBackground ? GetSymbolBackgroundSprite(SymbolId) : null;
        Sprite symbol = withSymbol ? GetSymbolSprite(SymbolId) : null;

        var view = MonoBehaviour.Instantiate(_symbolViewPrefab);
        view.Init(bg, symbol);
        return view;
    }

    private Sprite GetSymbolBackgroundSprite(string symbolId)
    {
        if (_tileBGSprites != null && _tileBGSprites.TryGetValue(symbolId, out var bgSprite))
        {
            return bgSprite.Sprite;
        }

        return null;
    }

    private Sprite GetSymbolSprite(string symbolId)
    {
        if (_symbolsSprites != null && _symbolsSprites.TryGetValue(symbolId, out var symbolSprite))
        {
            return symbolSprite.Sprite;
        }
        return null;
    }

    private int GetRandomValidSymbolId()
    {
        return GameDataHolder.GetRandomValidSymbolId();
    }
}
