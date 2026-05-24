using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] int stashedGold = 10;
    Rigidbody rb;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }
    public override void TakeDamage(float damage)
    {        
        base.TakeDamage(damage);
        if (currentHealth - damage <= 0 && death.isDead)
        {
            rb.isKinematic = true; // Olunce fizik etkisi kalmasin diye kinematic yapiyoruz.
            EconomyManager.Instance.AddGold(stashedGold); // playera odul olarak stashedGold kadar altin veriyoruz.
            return;
        }
    }
}
