using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Death death;

    public Health GetHealthScipt() => health;
    public Death GetDeathScript() => death;
}
