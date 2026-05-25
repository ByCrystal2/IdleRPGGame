using SingletonSystem;
using UnityEngine;

public class GameManager : PersistentSinleton<GameManager>
{
    PlayerStats playerStats;
    protected override void Awake()
    {
        base.Awake();
        playerStats = FindAnyObjectByType<PlayerStats>();
    }
    public void SaveGame()
    {
        if (playerStats != null)
        {
            PlayerPrefs.SetInt("Player_Level", playerStats.level);
            PlayerPrefs.SetInt("Player_Exp", playerStats.currentXP);
            PlayerPrefs.SetInt("Gold", EconomyManager.Instance.GetBalance(CurrencyType.Gold));
            PlayerPrefs.SetInt("Gem", EconomyManager.Instance.GetBalance(CurrencyType.Gem));
            PlayerPrefs.Save();
            Debug.Log("Game Saved");
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus) 
        {
            SaveGame();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause) 
        { 
            SaveGame();
        }
    }
}
