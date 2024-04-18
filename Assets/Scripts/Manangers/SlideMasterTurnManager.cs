using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class SlideMasterTurnManager : ITurnManager
{
    private TurnView _turnView;
    //private Queue<TurnObject> _turnObjects = new Queue<TurnObject>();
    private TurnObject _currentTurn;
    //private TurnObject _nextTurn;
    private SymbolFactory _symbolFactory;
    private InsertionPosition _defaultInsertionPosition;
    private Queue<Symbol> _symbolsQueue = new Queue<Symbol>();

    public SlideMasterTurnManager(TurnView view, TurnData turnData, SymbolFactory symbolFactory, InsertionPosition defaultInsertionPosition)
    {
        _turnView = view;
        _symbolFactory = symbolFactory;
        _turnView.Initialize();
        EventManager.TurnOver += HandleTurnOver;
        if (defaultInsertionPosition == null)
        {
            EventManager.InsertionPointClicked += CreateInitialTurns;
        }
        else
        {
            _defaultInsertionPosition = defaultInsertionPosition;
            CreateInitialTurns();
        }
    }

    private void HandleTurnOver()
    {
        AdvanceTurn(GetTurnObjet());
    }
    
    private void CreateInitialTurns(InsertionPosition defaultInsertionPosition)
    {
        EventManager.InsertionPointClicked -= CreateInitialTurns;
        _defaultInsertionPosition = defaultInsertionPosition;
        CreateInitialTurns();
    }

    private void CreateInitialTurns()
    { 
        EnqueueSymbol();
        if (_defaultInsertionPosition)
        {
            return;
        }

        if (_currentTurn == null) //TODO fix double _currentTurn init later
        {
            _currentTurn = GetTurnObjet();
        }
    }
    
    private TurnObject GetTurnObjet()
    {
        var turnObject = new TurnObject();
        return turnObject;
    }

    private void EnqueueSymbol()
    {
        var symbol = _symbolFactory.GetRandomSymbol();
        Debug.Log("created symbol for turn view");
        symbol.Hide();
        _turnView.QueueSymbol(symbol);
        _symbolsQueue.Enqueue(symbol);
    }

    private void AdvanceTurn(TurnObject turnObject)
    {
        _currentTurn.Reset();
        StartTurn();
    }
    
    public void StartTurn()
    {
        EnqueueSymbol();
        _turnView.AdvanceTurn();
        var currentSymbol = _symbolsQueue.Dequeue();
        currentSymbol.Show(); //TODO delete this and the hide once the "next symbol" is ready
        _currentTurn.Activate(_defaultInsertionPosition, currentSymbol);
    }

    public void AdvanceTurn()
    {
        throw new System.NotImplementedException();
    }

}
