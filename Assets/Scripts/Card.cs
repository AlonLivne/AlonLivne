using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Card : MonoBehaviour
{
    private CardData _data;

    public GemModifier? GetPowerUp(int symbolId)
    {
        foreach (var powerUp in _data.PowerUps)
        {
            if (powerUp.Id == symbolId)
            {
                return powerUp;
            }
        }

        return null;
    }

    public int GetHP()
    {
        return _data.HP;
    }
}
