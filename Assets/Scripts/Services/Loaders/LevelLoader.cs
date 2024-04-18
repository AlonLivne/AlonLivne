using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader
{

    //var config = get firebase config
    IEnumerable Load()
    {
        LoadPlayers();
        LoadTroops();
        LoadBoard();
        yield return null;
    }

    private void LoadBoard()
    {
    }

    private void LoadTroops()
    {
        throw new System.NotImplementedException();
    }

    private void LoadPlayers()
    {
        throw new System.NotImplementedException();
    }
}
