using System;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GameData
{
    // General Stats
    public int DeathCount;
    public int PlayerFame;
    public int PlayerGold;

    // Player Stats
    public float PlayerCurrentHealth;
    public float PlayerMaxHealth;
    public float PlayerCurrentSanity;
    public float PlayerMaxSanity;
    

    public GameData()
    {
        this.DeathCount = 0;
        this.PlayerFame = 0;
        this.PlayerGold = 0;

        this.PlayerCurrentHealth = this.PlayerMaxHealth= 10f;
        this.PlayerCurrentSanity = this.PlayerMaxSanity= 100f;

    }
}
