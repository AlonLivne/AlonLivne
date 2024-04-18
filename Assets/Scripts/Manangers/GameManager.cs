using System.Threading.Tasks;
using UnityEngine;

public class GameManager
{
    private ITurnManager _turnManager;
    private ScoreManager _scoreManager;
    private BoardManager _boardManager;

    public GameManager(ITurnManager turnManager, BoardManager boardManager, ScoreManager scoreManager)
    {
        _scoreManager = scoreManager;
        _turnManager = turnManager;
        _boardManager = boardManager;
        //_boardManager.BoardLoadingDone += StartGame;
    }
    
    private void StartGame()
    {
        Debug.Log("GameManager is starting game");
        //_turnManager.StartGame();
    }
    
    public async Task StartGameEntrySequence()
    {
        await _boardManager.LoadBoard();
        await _scoreManager.LoadScoreView();
        //_turnManager.StartGame();
    }
    
}
