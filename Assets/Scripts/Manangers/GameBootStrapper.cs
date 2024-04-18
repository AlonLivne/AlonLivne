using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameBootStrapper : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;
    private SlideMasterTurnManager _turnManager;
    [SerializeField] private ScoreManager _scoreManager;

    public InsertionPosition InsertionPositionPrefab;
    public InsertionLiftUpPosition InsertionLiftUpPrefab;

    [SerializeField] private GameObject _tileTransformPrefab;
    #region Turn View
    public AssetReference TurnView; //TODO use the reference once the prefab is fixed, I stopped using it because the turn view is not properly initted 
    public TurnView SceneTurnView;
    #endregion
    
    #region Symbol Factory Dependencies
    [SerializeField] private SpriteDictionaryObject _symbolsSprites;
    [SerializeField] private SpriteDictionaryObject _tileBGSprites;

    public AssetReference SymbolViewPrefab;
    private SymbolView _symbolViewPrefab;
    private SymbolFactory _symbolFactory;
    
    #endregion
    
    #region ScoreView
    [SerializeField] private ScoreView _scoreView;
    #endregion
    
    private InsertionPosition _defaultInsertionPosition;
    
    private void Awake()
    {
        InitSelf();
        if (GameDataHolder.InitializationIsCompleted)
        {
            InitDependencies();
        }
        else
        {
            Debug.Log("GameBootStrapper is waiting for GameDataHolder to finish Initializing before starting");
            GameDataHolder.RegisterToInitializationCompleteEvent(InitDependencies);
        }
        EventManager.InsertionPointClicked += InsertionClickedHack;
    }

    private void InsertionClickedHack(InsertionPosition point)
    {
        _defaultInsertionPosition = point;
        EventManager.InsertionPointClicked -= InsertionClickedHack;
    }

    private void InitSelf()
    {
        Debug.Log("GameBootStrapper is initializing self");
    }

    private void InitDependencies()
    {
        Debug.Log("GameBootStrapper is initializing dependencies");
        InitSymbolFactory();

    }
    
    private void InitSymbolFactory()
    {
        _symbolFactory = new SymbolFactory(SymbolViewPrefab, _symbolsSprites, _tileBGSprites, OnSymbolFactoryInitComplete);
    }

    private void OnSymbolFactoryInitComplete()
    {
        Debug.Log("SymbolFactory Initted");
        
        InitBoardManager();
        Debug.Log("BoardManager Initted");
        InitTurnView();

        InitScoreManager();
        Debug.Log("ScoreManager Initted");
        
        Debug.Log("GameBootStrapper finished initializing dependencies");
    }

    private async void InitTurnView()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(TurnView);
        handle.Completed += TurnViewReady;
    }

    private async void TurnViewReady(AsyncOperationHandle<GameObject> obj)
    {
        var view = obj.Result.GetComponent<TurnView>();
        InitTurnManager(SceneTurnView); //view); //TODO use this once turn view is properly initted
    }

    private void InitTurnManager(TurnView view)
    {
        var turnManagerData = GameDataHolder.GetTurnsData();
        _turnManager = new SlideMasterTurnManager(view, turnManagerData, _symbolFactory, _defaultInsertionPosition);
        Debug.Log("TurnManager Initted");
        StartGameEntrySequence();
    }

    private void InitBoardManager()
    {
        var configs = GameDataHolder.GetBoardConfigs();
        _boardManager.Init(configs, _symbolFactory, InsertionPositionPrefab, InsertionLiftUpPrefab);
        StartGameEntrySequence();
    }

    private void InitScoreManager()
    {
        //TODO get the actual config of the players from teh back end
        var enemyCards = GameDataHolder.GetEnemyCardData();
        var playerCards = GameDataHolder.GetPlayerCardData();
        var playerShieldData = GameDataHolder.GetPlayerShieldData();
        var enemyShieldData = GameDataHolder.GetEnemyShieldData();
        var playerConfig = new PlayerScoreConfig(playerCards, playerShieldData);
        var enemyConfig = new PlayerScoreConfig(enemyCards, enemyShieldData);
        var jointConfig = new Dictionary<int, PlayerScoreConfig>();
        jointConfig[GameDataHolder.Player1Index] = playerConfig;
        jointConfig[GameDataHolder.Player2Index] = enemyConfig;
        
        var config = new ScoreConfig(jointConfig, GameDataHolder.GetSymbolsScoringData());
        _scoreManager = new ScoreManager(config, _scoreView);
    }

    private void StartGameEntrySequence()
    {
         if (_turnManager == null || _boardManager == null)
        {
            return;
        }
        Debug.Log("Initting Game Manager");
        var gameManager = new GameManager(_turnManager, _boardManager, _scoreManager);
        gameManager.StartGameEntrySequence();
        EventManager.InsertionPointClicked -= InsertionClickedHack;
        Destroy(this);
    }
}
