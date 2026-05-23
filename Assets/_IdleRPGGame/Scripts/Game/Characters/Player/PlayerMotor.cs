using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Health health;
    [SerializeField] Death death;

    public Health GetHealthScipt() => health;
    public Death GetDeathScript() => death;
    public PlayerStats GetPlayerStats() => stats;
}
