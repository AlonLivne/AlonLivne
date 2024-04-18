using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileData
{
    private (int, int) _coordinates;
    public TileData((int, int) coordinates)
    {
        _coordinates = coordinates;
    }
}
