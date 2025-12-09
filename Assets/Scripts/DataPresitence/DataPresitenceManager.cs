using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;

public class DataPresitenceManager : MonoBehaviour
{
    private GameData _gameData;
    public static DataPresitenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("More than one DataPresitenceManager spotted in scene");
        }
    }

    private void Start()
    {
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void StartGame()
    {
        this._gameData = new GameData();
    }

    public void LoadGame()
    {
        //load saved data from file and deserialize it into _gameData

        //if no saved data found, initialize new game data
        if (this._gameData == null)
        {
            Debug.Log("No data found, start new game");
            StartGame();
        }

        //Give save data to other systems
    }


    public void SaveGame()
    {
        //get all data from other systems and update _gameData

        //serialize _gameData and save to file
    }
}
