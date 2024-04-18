using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

public class BoardManager : MonoBehaviour
{
    public AssetReference SymbolViewPrefab;

    [SerializeField] private GameObject _tileTransformPrefab;

    private SymbolView _symbolViewPrefab;
    
    private BoardConfig _config;
    private Dictionary<(int, int), MOATile> _tiles;
    private bool _tilesCreated;
    private SymbolFactory _symbolFactory;
    private InsertionPosition _insertionPositionPrefab;
    private InsertionLiftUpPosition _insertionLiftUpPositionPrefab;
    private List<Streak> _streaksToPlay;
    private Action OnStreaksHandelingOver;
    private Action OnSymbolBreakOver;
    private Action OnSymbolMovementOver;
    public Action BoardLoadingDone;
    private List<Symbol> _movingSymbols;
    public BoardManager(BoardConfig config, SymbolFactory symbolFactory, InsertionPosition insertionPositionPrefab, InsertionLiftUpPosition insertionLiftUpPositionPrefab)
    {
        _config = config;
        _symbolFactory = symbolFactory;
        _insertionPositionPrefab = insertionPositionPrefab;
        _insertionLiftUpPositionPrefab = insertionLiftUpPositionPrefab;
        _movingSymbols = new List<Symbol>();
        EventManager.PlayerPlayed += ExecutePlayerChoice;
        EventManager.OnStreakHandlingOver += OnStreakDone;
    }

    public void Init(BoardConfig configs, SymbolFactory symbolFactory, InsertionPosition insertionPositionPrefab, InsertionLiftUpPosition insertionLiftUpPositionPrefab)
    {
        _config = configs;
        _symbolFactory = symbolFactory;
        _insertionPositionPrefab = insertionPositionPrefab;
        _insertionLiftUpPositionPrefab = insertionLiftUpPositionPrefab;
        _movingSymbols = new List<Symbol>();
        EventManager.PlayerPlayed += ExecutePlayerChoice;
        EventManager.OnStreakHandlingOver += OnStreakDone;
    }

    private void ExecutePlayerChoice(TurnObject turnobject)
    {
        Debug.Log("starting to execute player's choice");
        var symbol = turnobject.TurnSymbol;
        var coords = turnobject.Coords;
        var direction = GetDirection(coords);
        var insertionCoords  = (coords.Item1 + direction.Item1, coords.Item2 + direction.Item2);

        HandleTileMovement(symbol, insertionCoords, direction);
    }

    private (int, int) GetDirection((int, int) coords)
    {
        var direction = (0, -1);
        if (coords.Item1 == -1)
        {
            direction = (1, 0);
        } 
        else if (coords.Item1 == _config._sizeX)
        {
            direction = (-1,0);
        }

        return direction;
    }

    private void HandleTileMovement(Symbol symbol, (int, int) insertionCoords, (int, int) direction)
    {
        Debug.Log($"handling symbol movement {insertionCoords} direction {direction}");
        OnStreaksHandelingOver += StreaksExecutionOver;

        ClearRoomForPush(insertionCoords, direction,
            () => { AfterTileCleared(symbol, insertionCoords, direction);});
    }
    
    private void AfterTileCleared(Symbol symbol, (int, int) insertionCoords, (int, int) direction)
    {
        var didAdvance = AdvanceLine(insertionCoords, direction,
            ()=>{InsertSymbol(symbol, insertionCoords, ()=>
            {
                AdvanceGravityAllBoard(true);
            });});
        if (!didAdvance)
        {
            InsertSymbol(symbol, insertionCoords, ()=>{InsertSymbol(symbol, insertionCoords, ()=>
            {
                EventManager.InsertionOver(symbol);
                AdvanceGravityAllBoard(true);
            });});
        }
    }

    #region streaks handeling

        private void StreaksExecutionOver()
    {
        OnStreaksHandelingOver -= StreaksExecutionOver;
        EventManager.OnTurnOver();
    }
    
    private void CheckAndExecuteStreaksAllBoard()
    {
        Debug.Log("CheckAndExecuteStreaksAllBoard");
        EventManager.OnSymbolMovementOver += SymbolAdvanced;

        var streaks = CheckAllBoardForStreaks();
        
        if (streaks.Count > 0)
        {
            ExecuteStreaks(streaks);
            return;
        }
        
        EventManager.OnTurnOver();
        
        EventManager.OnSymbolMovementOver -= SymbolAdvanced;
    }
    
    private void ExecuteStreaks(List<Streak> streaks)
    {
        Debug.Log("ExecuteStreaks(List<Streak> streaks)");

        _streaksToPlay = streaks;
        ExecuteStreaks();
    }
    
    private void ExecuteStreaks()
    {
        Debug.Log("ExecuteStreaks()");

        var streaksToExecute = GetNextStreaksToExecute();
        Debug.Log($"Streaks count {streaksToExecute.Count}");

        foreach (var streak in streaksToExecute)
        {
            _streaksToPlay.Remove(streak);
            ExecuteStreak(streak);
        }
    }

    private List<Streak> GetNextStreaksToExecute()
    {
        var ans = new List<Streak>();
        var id = 0;
        foreach (var streak in _streaksToPlay)
        {
            if (ans.Count == 0)
            {
                id = streak.SymbolID;
            }

            if (streak.IsSameType(id))
            {
                ans.Add(streak);
            }
        }

        return ans;
    }

    private void OnStreakDone(Streak streak)
    {
        //Debug.Log($"OnStreakDone(Streak {streak.SymbolID})");

        _streaksToPlay.Remove(streak);
        if (_streaksToPlay.Count == 0)
        {
            AdvanceGravityAllBoard(false);
            return;
        }
        
        ExecuteStreaks();
    }
    
    private void ExecuteStreaks(Queue<Streak> streaks)
    {
        foreach (var streak in streaks)
        {
            ExecuteStreak(streak);
        }
    }
    
    private void ExecuteStreak(Streak streak)
    {
        Debug.Log($"ExecuteStreak(Streak streak)");

        foreach (var tile in streak.TilesInStreak)
        {
            BreakSymbol(tile, BreakType.Streak);
        }
    }

    private List<Streak> CheckAllBoardForStreaks()
    {
        var allStreaks = new List<Streak>();
        var direction = (0, 1);
        for (int x = 0; x < _config._sizeX; x++)
        {
            var coords = (x, 0);
            var lineStreaks = CheckForLineStreaks(coords, direction);
            allStreaks = JoinStreaks(allStreaks, lineStreaks);
        }

        return allStreaks;
    }

    private List<Streak> CheckForLineStreaks((int, int) coordsToAdvance, (int, int) direction)
    {
        var streaks = new List<Streak>();
        while (AreCoordsInBoard(coordsToAdvance))
        {
            var newStreaks = CheckForStreaks(coordsToAdvance);
            streaks = JoinStreaks(streaks, newStreaks);
            coordsToAdvance.Item1 += direction.Item1;
            coordsToAdvance.Item2 += direction.Item2;
        }

        return streaks;
    }

    private List<Streak> JoinStreaks(List<Streak> streaks, List<Streak> newStreaks)
    {
        foreach (var streak in streaks)
        {
            if (newStreaks.Contains(streak))
            {
                newStreaks.Remove(streak);
            }
        }
        streaks.AddRange(newStreaks);
        return streaks;
    }
    
       private List<Streak> CheckForStreaks((int x, int y) coordinates)
    {
        var symbolIdNullable = _tiles[coordinates].CurrentSymbolId;
        if (symbolIdNullable == null)
        {
            return new List<Streak>();;
        }
        var symbolId = symbolIdNullable.Value;
        
        return CheckForStreaks(coordinates, symbolId);
    }

    private List<Streak> CheckForStreaks((int x, int y) coordinates, int symbolId)
    {
        //TODO better algorithm here
        var startTile = _tiles[coordinates];
        var ans = new List<Streak>();
        var streakX = new List<MOATile>()
        {
            startTile
        };
        var streakY = new List<MOATile>()
        {
            startTile
        };
        
        var x = coordinates.x + 1 ;
        var y = coordinates.y;
        while (x <_config._sizeX && _tiles[(x , y)].CurrentSymbolId != null && _tiles[(x , y)].CurrentSymbolId == symbolId)
        {
            streakX.Add(_tiles[(x,y)]);
            x = x + 1;
        }
        x = coordinates.x - 1;
        while (x >= 0 && _tiles[(x , y)].CurrentSymbolId != null && _tiles[(x , y)].CurrentSymbolId == symbolId)
        {
            streakX.Add(_tiles[(x,y)]);
            x = x - 1;
        }

        if (streakX.Count < 3)//TODO - config this
        {
            streakX.Clear();
        }
        else
        {
            var streak = new Streak(streakX);
            ans.Add(streak);
        }
        x = coordinates.x;
        y = coordinates.y + 1;
        while (y <_config._sizeY && AreSymbolsEqual((x,y),symbolId))
        {
            streakY.Add(_tiles[(x,y)]);
            y = y + 1;
        }
        y = coordinates.y - 1;
        while (y >= 0 && _tiles[(x , y)].CurrentSymbolId != null && _tiles[(x , y)].CurrentSymbolId == symbolId)
        {
            streakY.Add(_tiles[(x,y)]);
            y = y - 1;
        }
        
        if (streakY.Count < 3)//TODO - config this
        {
            streakY.Clear();
        } else
        {
            var streak = new Streak(streakY);
            ans.Add(streak);
        }

        //ans = UniteStreaks(streakX, streakY); //We decided that there are no special shapes, just lines/rows
        return ans;
    }

    private List<Streak> UniteStreaks(List<MOATile> streak1, List<MOATile> streak2)
    {
        var tiles = streak1;
        var ans = new List<Streak>();
        
        if (streak2.Count > 0)
        {
            foreach (var tile in streak2)
            {
                if (streak1.Contains(tile))
                {
                    continue;
                }
            
                tiles.Add(tile);
            }
        }

        if (tiles.Count > 0)
        {
            var streak = new Streak(tiles);
            ans.Add(streak);
        }
        return ans;
    }

    #endregion

    #region symbol handling

    private void BreakSymbol((int, int) coords, BreakType breakType, Action breakingDoneCallback = null)
    {
        Debug.Log($"breaking symbol in {coords}");
        var tileToDestroy = _tiles[coords];
        BreakSymbol(tileToDestroy, breakType, breakingDoneCallback);
    }
    
    private void BreakSymbol(MOATile tileToDestroy, BreakType breakType, Action breakingDoneCallback = null)
    {
        Debug.Log($"breaking symbol in {tileToDestroy.Transform.name} for reason {breakType}");
        tileToDestroy.BreakSymbol(breakType, breakingDoneCallback);
    }
    
    private void InsertSymbol(Symbol symbol, (int, int) coordsToEnter, Action outOfBoardCallback = null)
    {
        var tile = _tiles[coordsToEnter];
        Debug.Log($"inserting symbol symbol in {tile.Transform.name}");
        AdvanceSymbol(symbol, tile, outOfBoardCallback);
        //StartCoroutine(tile.AdoptSymbolCoroutine(symbol));
    }

    #endregion

    #region movement handling

    private void AdvanceGravityAllBoard(bool forceCheckStreaks = false)
    {
        AdvanceGravityAllBoard(CheckAndExecuteStreaksAllBoard, forceCheckStreaks);
    }

    private void AdvanceGravityAllBoard(Action onGravityExecutionOver = null, bool forceCheckStreaks = false)
    {
        Debug.Log($"AdvanceGravityAllBoard(Action onGravityExecutionOver = {onGravityExecutionOver})");

        var direction = (0, 1);
        var advanced = false;
        Action callback = null;
        for (int x = 0; x < _config._sizeX; x++)
        {
            var coord = (x, 0);
            if (x == _config._sizeX - 1)
            {
                callback = onGravityExecutionOver;
            }
            advanced = advanced || AdvanceGravityLine(coord, direction, callback);
        }

        if (!advanced)
        {
            if (forceCheckStreaks)
            {
                CheckAndExecuteStreaksAllBoard();
                return;
            }

            if (callback == null)
            {
                StreaksExecutionOver();
            }
        }
    }

    private bool AdvanceGravityLine((int, int) startCoord, (int, int) direction, Action onAdvanceOverOver = null)
    {
        var advanced = false;
        Debug.Log($"Advancing gravity line in coord {startCoord} direction {direction}");
        var possibleFreeCoords = FindNextFreeCoords(startCoord, direction, true);
        if (possibleFreeCoords != null)
        {
            var freeCoords = possibleFreeCoords.Value;
            var nullableCoordsToAdvanceFrom = FindNextCoordsByState(freeCoords, direction, false,true);
            Action callback = null;
            while (AreCoordsInBoard(nullableCoordsToAdvanceFrom))
            {
                var coordsToAdvanceFrom = nullableCoordsToAdvanceFrom.Value;
                var tileToAdvanceFrom = _tiles[coordsToAdvanceFrom];
                var tileToAdvanceTo = _tiles[freeCoords];
                
                freeCoords = coordsToAdvanceFrom;
                nullableCoordsToAdvanceFrom = FindNextCoordsByState(freeCoords, direction, false,true);
                    
                if (!AreCoordsInBoard(nullableCoordsToAdvanceFrom))
                {
                    callback = onAdvanceOverOver;
                }
                AdvanceSymbol(tileToAdvanceFrom, tileToAdvanceTo, callback);
                advanced = true;
            }
        }
        return advanced;
    }
    
    private bool AdvanceLine((int, int) startCoord, (int, int) direction, Action onAdvanceOverOver = null)
    {
        var advanced = false;
        Debug.Log($"Advancing line in coord {startCoord} direction {direction}");
        var possibleFreeCoords = FindNextFreeCoords(startCoord, direction, true);
         if (possibleFreeCoords != null)
        {
            var freeCoords = possibleFreeCoords.Value;
            var coordsToAdvanceFrom = (freeCoords.Item1 - direction.Item1,
                freeCoords.Item2 - direction.Item2);
            Action callback = null;
            while (AreCoordsInBoard(coordsToAdvanceFrom))
            {
                var tileToAdvanceFrom = _tiles[coordsToAdvanceFrom];
                var tileToAdvanceTo = _tiles[freeCoords];

                freeCoords = coordsToAdvanceFrom;
                coordsToAdvanceFrom = (coordsToAdvanceFrom.Item1 - direction.Item1,
                    coordsToAdvanceFrom.Item2 - direction.Item2);
                if (!AreCoordsInBoard(coordsToAdvanceFrom))
                {
                    callback = onAdvanceOverOver;
                }
                AdvanceSymbol(tileToAdvanceFrom, tileToAdvanceTo, callback);
                advanced = true;
            }
        }
        return advanced;
    }

    private void AdvancePush((int, int) startCoord, (int, int) direction)
    {
        Debug.Log($"Advancing push in coord {startCoord} direction {direction}");
        var coordsToAdvance = (startCoord.Item1 + direction.Item1, startCoord.Item2 + direction.Item2);
        if (_tiles[startCoord].CurrentSymbol == null || !AreCoordsInBoard(coordsToAdvance))
        {
            return;
        }
        
        //ContinueAdvancePush(startCoord, direction, coordsToAdvance);
        var tileToAdvanceFrom = _tiles[startCoord];
        var tileToAdvanceTo = _tiles[coordsToAdvance];
        var symbol = tileToAdvanceTo.CurrentSymbol;
        if (symbol != null)
        {
            AdvancePush(coordsToAdvance, direction);
        }
        
        AdvanceSymbol(tileToAdvanceFrom, tileToAdvanceTo);
    }

    
    private void ContinueAdvancePush((int, int)  startCoord, (int, int) direction, (int, int) coordsToAdvance)
    {
        var tileToAdvanceFrom = _tiles[startCoord];
        var tileToAdvanceTo = _tiles[coordsToAdvance];
        var symbol = tileToAdvanceTo.CurrentSymbol;
        if (symbol != null)
        {
            ContinueAdvancePush(startCoord, direction, coordsToAdvance);
        }
        
        AdvanceSymbol(tileToAdvanceFrom, tileToAdvanceTo);
    }
    #endregion
    
    private bool AreCoordsInBoard((int, int)? nullableCoordsToCheck)
    {
        if (nullableCoordsToCheck == null)
        {
            return false;
        }

        var coordsToCheck = nullableCoordsToCheck.Value;
        return (coordsToCheck.Item1 > -1 && coordsToCheck.Item1 < _config._sizeX
                                        && coordsToCheck.Item2 < _config._sizeY && coordsToCheck.Item2 > -1);
    }

    private void ClearRoomForPush((int, int) startCoord, (int, int) direction, Action outOfBoardCallback = null)
    {
        Debug.Log($"Clearin room for push in coord {startCoord} direction {direction}");
        var breakCoords = startCoord;
        var coordsToAdvance = startCoord;// (startCoord.Item1 + direction.Item1, startCoord.Item2 + direction.Item2);
        while (AreCoordsInBoard(coordsToAdvance))
        {
            if (_tiles[coordsToAdvance].CurrentSymbol == null)
            {
                Debug.Log($"Free slot for push in dir {direction} in coord {startCoord} ");
                outOfBoardCallback?.Invoke();
                return;
            }

            breakCoords = coordsToAdvance;
            coordsToAdvance = (coordsToAdvance.Item1 + direction.Item1, coordsToAdvance.Item2 + direction.Item2);
        }
        
        BreakSymbol(breakCoords, GetBreakDirection(direction), outOfBoardCallback);
    }
    
    private BreakType GetBreakDirection((int, int) direction)
    {
        if (direction.Item1 == -1)
        {
            return BreakType.Left;
        }

        if (direction.Item1 == 1)
        {
            return BreakType.Right;
        }

        return BreakType.Down;
    }

    private void AdvanceSymbol(MOATile tileToAdvanceFrom, MOATile tileToAdvanceTo, Action onAdvancementOver = null)
    {
        var symbolToAdvance = tileToAdvanceFrom.CurrentSymbol; ;
        tileToAdvanceFrom.ReleaseSymbol();
        AdvanceSymbol(symbolToAdvance,tileToAdvanceTo, onAdvancementOver);
    }    
    
    private void AdvanceSymbol(Symbol symbolToAdvance, MOATile tileToAdvanceTo, Action onAdvancementOver = null)
    {
        _movingSymbols.Add(symbolToAdvance);
        tileToAdvanceTo.AdopSymbol(symbolToAdvance, onAdvancementOver);
    }

    private void SymbolAdvanced(Symbol symbol)
    {
        _movingSymbols.Remove(symbol);
        if (_movingSymbols.Count == 0)
        {
            CheckAndExecuteStreaksAllBoard();
        }
    }
    
    private (int,int)? FindNextFreeCoords((int x, int) coords, (int, int) direction, bool inclusive = false)
    {
        return FindNextCoordsByState(coords, direction, inclusive, false);
    }

    private (int, int)? FindNextCoordsByState((int x, int) coords, (int, int) direction, bool inclusive, bool isOccupied)
    {
        if (!inclusive)
        {
            coords = (coords.Item1 + direction.Item1, coords.Item2 + direction.Item2);
        }
        
        if (AreCoordsInBoard(coords))
        {
            var tile = _tiles[coords];
            var symbol = tile.CurrentSymbol;
            if ((symbol != null) == isOccupied)
            {
                return coords;
            }
            return FindNextCoordsByState(coords, direction, false, isOccupied);
        }
        return null;
    }

    private MOATile FindNextFreeTile((int x, int) coords, (int, int) direction, bool inclusive = false)
    {
        return FindNextTileByState(coords, direction, inclusive, false);
    }

    private MOATile FindNextTileByState((int x, int) coords, (int, int) direction, bool inclusive, bool isOccupied)
    {
        if (!inclusive)
        {
            coords = (coords.Item1 + direction.Item1, coords.Item2 + direction.Item2);
        }
        
        if (AreCoordsInBoard(coords))
        {
            var tile = _tiles[coords];
            var symbol = tile.CurrentSymbol;
            if ((symbol != null) == isOccupied)
            {
                return tile;
            }
            return FindNextOccupiedTile(coords, direction, false);
        }
        return null;
    }

    private MOATile FindNextOccupiedTile((int x, int) coords, (int, int) direction, bool inclusive = false)
    {
        return FindNextTileByState(coords, direction, inclusive, true);
    }

    private void AdvanceSymbols((int, int) coords, (int, int) direction)
    {
        Debug.Log($"Advancing coords {coords} in direction {direction}");
        while (AreCoordsInBoard(coords)) //TODO insert condition
        {
            Debug.Log($"Checking symbol for coords {coords} in direction {direction}");

            var tileToAdvanceTo = _tiles[coords];
            
            Symbol symbolToAdvance = null;
            var coordsToAdvance = (coords.Item1 - direction.Item1, coords.Item2 - direction.Item2);
            while (AreCoordsInBoard(coordsToAdvance) && symbolToAdvance == null)
            {
                Debug.Log($"Checking advancement symbol for coords {coordsToAdvance} in direction");

                var tileToAdvanceFrom = _tiles[coordsToAdvance];
                symbolToAdvance = tileToAdvanceFrom.CurrentSymbol;
                if (symbolToAdvance != null)
                {
                    Debug.Log($"moving symbol from {tileToAdvanceFrom.Transform.name} to {tileToAdvanceTo.Transform.name}");
                    tileToAdvanceTo.AdopSymbol(symbolToAdvance);
                    tileToAdvanceFrom.ReleaseSymbol();
                    var coordsBelow = (coordsToAdvance.Item1, coordsToAdvance.Item2 - 1);
                    if (AreCoordsInBoard(coordsBelow))
                    {
                        
                    }
                    break;
                }
                coordsToAdvance = (coordsToAdvance.Item1 - direction.Item1, coordsToAdvance.Item2 - direction.Item2);
            }
            
            coords = (coords.Item1 - direction.Item1, coords.Item2 - direction.Item2);
        }
    }

    private Symbol GetSymbol((int, int) coords)
    {
        MOATile tile;
        if (_tiles.TryGetValue(coords, out tile))
        {
            return tile.CurrentSymbol;
        }

        return null;
    }

    #region board creation
    public async Task LoadBoard()
    {
        Debug.Log("Starting board loading");
        
        HandleBoard();
        
        //yield return new WaitUntil(()=>_tilesCreated);
        await PopulateTiles();
        Debug.Log("Finished board loading");
        BoardLoadingDone?.Invoke();
    }

    private async Task PopulateTiles()
    {
        for (int j = 0; j < _config._sizeY; j++)
        {
            for (int i = 0; i < _config._sizeX; i++)
            {
                int symbolId = GetLegalId(i,j);
                Debug.Log($"Adding sym {symbolId} to tile {i},{j}");
                var symbol = GenerateSymbol(symbolId);
                var tile = _tiles[(i, j)];
                StartCoroutine(tile.AdoptSymbolCoroutine(symbol));
                await Task.Delay(40);
            }
        }
    }
    
    private int GetLegalId(int i, int j)
    {
        var streaks = new List<Streak>();
        int id;
        do
        {
            id = GameDataHolder.GetRandomValidSymbolId();
            streaks = CheckForStreaks((i, j), id);
        } 
        while (streaks.Count != 0);

        return id;
    }

    private Symbol GenerateSymbol(int symbolId)
    {
        return _symbolFactory.GetSymbol(symbolId);
    }

    private void HandleBoard()
    {
        var offsets = PrepareRandomSymbolAndReturnSizes(_symbolViewPrefab);
        CreateTiles(offsets);
        CreateInsertionPoints(offsets);
    }
    
    private void CreateTiles((float, float) offsets)
    {
        var screenOffset = GetScreenOffset();
        _tiles = new Dictionary<(int, int), MOATile>();
        for (int i = 0; i < _config._sizeX; i++)
        {
            for (int j = 0; j < _config._sizeY; j++)
            {
                var obj = Instantiate(_tileTransformPrefab);
                obj.name = $"Tile{i}{j}";
                obj.transform.position = new Vector3((i -_config._sizeX/2) * offsets.Item1, (j - _config._sizeY) *offsets.Item2, 0);
                var data = new TileData((i, j));
                _tiles[(i, j)] = new MOATile(data, obj.transform);
            }
        }
        
        _tilesCreated = true;
    }

    private void CreateInsertionPoints((float, float) offsets)
    {
        InsertionPosition insertionPoint;
        InsertionPosition hackDefaultInsertionPoint; //TODO write normal architecture someday
        InsertionLiftUpPosition liftUpPositionDetector;
        for (int i = 0; i < _config._sizeX; i++)
        { 
            insertionPoint = CreateInsertionPosition((i, _config._sizeY));
            insertionPoint.transform.position = new Vector3((i -_config._sizeX/2) * offsets.Item1, (_config._sizeY - _config._sizeY) *offsets.Item2, 0);

            liftUpPositionDetector = CreateLiftUpPositionDetector((i, _config._sizeY - 1), insertionPoint);
            liftUpPositionDetector.transform.position = new Vector3((i -_config._sizeX/2) * offsets.Item1, (-1) *offsets.Item2, 0);

            if (i == 2)
            {
                hackDefaultInsertionPoint = insertionPoint;
                EventManager.OnInsertionClicked(hackDefaultInsertionPoint);
            }
        }

        for (int j = 0; j < _config._sizeY; j++)
        {
            insertionPoint = CreateInsertionPosition((-1, j), 90);
            insertionPoint.transform.position = new Vector3((-1 -_config._sizeX/2) * offsets.Item1, (j - _config._sizeY) *offsets.Item2, 0);
            liftUpPositionDetector = CreateLiftUpPositionDetector((0, j), insertionPoint);
            liftUpPositionDetector.transform.position = new Vector3((0 -_config._sizeX/2) * offsets.Item1, (j - _config._sizeY) *offsets.Item2, 0);

            insertionPoint = CreateInsertionPosition((_config._sizeX, j), 270);
            insertionPoint.transform.position = new Vector3((_config._sizeX -_config._sizeX/2) * offsets.Item1, (j - _config._sizeY) *offsets.Item2, 0);
            liftUpPositionDetector = CreateLiftUpPositionDetector((_config._sizeX - 1, j), insertionPoint);
            liftUpPositionDetector.transform.position = new Vector3((_config._sizeX - 1 -_config._sizeX/2) * offsets.Item1, (j - _config._sizeY) *offsets.Item2, 0);
        }
    }

    private InsertionLiftUpPosition CreateLiftUpPositionDetector((int, int) coordinates,
        InsertionPosition insertionPosition)
    {
        var LiftUpPoint = MonoBehaviour.Instantiate<InsertionLiftUpPosition>(_insertionLiftUpPositionPrefab);
        LiftUpPoint.Coordinates = (coordinates.Item1, coordinates.Item2);
        LiftUpPoint.name = $"LiftUpPPoint{coordinates}";
        LiftUpPoint.Init(insertionPosition);
        if (coordinates == (2, 4))
        {
            InsertionLiftUpPosition.ManuallySelectPosition(LiftUpPoint);
        }
        return LiftUpPoint;    
    }

    private InsertionPosition CreateInsertionPosition((int, int) coordinates, float zRotation = 0)
    {
        var insertionPoint = MonoBehaviour.Instantiate<InsertionPosition>(_insertionPositionPrefab);
        insertionPoint.Coordinates = (coordinates.Item1, coordinates.Item2);
        insertionPoint.transform.Rotate(0, 0, zRotation);
        insertionPoint.name = $"InsertionPoint{coordinates}";
        return insertionPoint;
    }
    
    private (float,float) GetScreenOffset()
    {
        var width = Screen.width;
        var height = Screen.height;
        return (width, height);
    }

    private (float, float) PrepareRandomSymbolAndReturnSizes(SymbolView prefab)
    {
        var symbol = _symbolFactory.GetSymbol(1);
        var size = symbol.GetSize();
        symbol.Dispose();
        return size;
    }
    #endregion

    private bool AreSymbolsEqual((int x, int y) coords, int symbolId)
    {
        return _tiles[(coords.x, coords.y)].CurrentSymbolId != null &&
               _tiles[(coords.x, coords.y)].CurrentSymbolId == symbolId;
    }
}


public struct BoardConfig
{
    public int _sizeX;
    public int _sizeY;
    public readonly List<int> SymbolsIds;
    public int SizeX => _sizeX;
    public int SizeY => _sizeY;

    public BoardConfig(int sizeX, int sizeY, List<int> symbols)
    {
        _sizeX = sizeX;
        _sizeY = sizeY;
        SymbolsIds = symbols;
    }
}

public struct FirebaseBoardConfig
{
    public List<int> List;
    public int SizeX;
    public int SizeY;
}

public class BoardConfigMediator : FirebaseDataMediator<FirebaseBoardConfig, BoardConfig>
{
    public override BoardConfig GetMediatedData(FirebaseBoardConfig rawData)
    {
        return new BoardConfig(rawData.SizeX, rawData.SizeY, rawData.List);
    }
}

public struct Streak : IEquatable<Streak>
{
    public List<MOATile> TilesInStreak;
    public int SymbolID => TilesInStreak.First().CurrentSymbolId.Value;
    //public string Gem = TilesInStreak.First().CurrentSymbolId;
    public Streak(List<MOATile> tilesInStreak)
    {
        TilesInStreak = tilesInStreak;
        EventManager.OnSymbolBreakingOver += SymbolBroken;
    } 

    public bool Equals(Streak other)
    {
        foreach (var tile in TilesInStreak)
        {
            if (!other.TilesInStreak.Contains(tile))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        return obj is Streak other && Equals(other);
    }

    public bool IsSameType(Streak other)
    {
        return IsSameType(other.SymbolID);
    }
    
    public bool IsSameType(int id)
    {
        return (SymbolID == id);
    }

    public void SymbolBroken(MOATile moaTile)
    {
        TilesInStreak.Remove(moaTile);
        if (TilesInStreak.Count == 0)
        {
            EventManager.OnSymbolBreakingOver -= SymbolBroken;
            EventManager.StreakHandlingOver(this);
        }
    }

    public int Count => TilesInStreak.Count;

}