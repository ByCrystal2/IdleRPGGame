using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField, Range(0, 100)] float health;
    Death death;
    private void Awake()
    {
        death = GetComponent<Death>();
    }
    public void SetDamage(float damage)
    {
        if (health - damage <= 0)
        {
            health = 0;
            //death...
            return;
        }
        health -= damage;
    }
}
