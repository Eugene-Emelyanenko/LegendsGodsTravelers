using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coins
{
    public readonly static string coinsKey = "Coins";

    public static void SaveCoins(int value)
    {
        PlayerPrefs.SetInt(coinsKey, value);
        PlayerPrefs.Save();
    }

    public static int GetCoins() => PlayerPrefs.GetInt(coinsKey, 0);
}
