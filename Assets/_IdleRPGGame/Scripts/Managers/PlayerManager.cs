using SingletonSystem;
using UnityEngine;

public class PlayerManager : PersistentSinleton<PlayerManager>
{
    PlayerMotor player;

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<PlayerMotor>();
    }
    public void AddGainForPlayer(int amount)
    {
        player.GetPlayerStats().GainXP(amount);
    }
    public bool IsThereAnEnemyNearby() 
    {
        bool result = player.GetCurrentTarget() != null && Vector3.Distance(player.transform.position, player.GetCurrentTarget().position) <= player.GetCurrentBow().attackRange;
        Debug.Log("IsThereAnEnemyNearby result =>" + result);
        return result;
    }
}
