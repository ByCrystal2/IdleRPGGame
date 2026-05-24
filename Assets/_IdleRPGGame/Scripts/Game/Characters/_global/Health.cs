using UnityEngine;

[RequireComponent (typeof(Death))]
public class Health : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] private float maxHealth = 50f;
    public float currentHealth { get; private set; }
    [HideInInspector]
    public Death death;
    protected virtual void Awake()
    {
        death = GetComponent<Death>();
        currentHealth = maxHealth;
    }
    // Hasar alma fonksiyonu
    public virtual void TakeDamage(float damage)
    {
        if (currentHealth - damage <= 0)
        {
            //death...
            currentHealth = 0;
            death.Die();
            return;
        }
        currentHealth -= damage;
        
        Debug.Log($"{gameObject.name} adli obje {damage} kadar hasar aldi!");
    }
    // Can yenileme fonksiyonu
    public virtual void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Canin maxHealth'i gecmesini engeller
        EventBus<Health>.Publish(EventType.UpgradeCharacter, this);
        Debug.Log($"{transform.parent.name}, {healAmount} can yeniledi. Guncel Can: {currentHealth}");
    }
    public float IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Max can arttiginda mevcut cani da arttiriyoruz
        EventBus<Health>.Publish(EventType.UpgradeCharacter, this);
        Debug.Log($"{transform.parent.name}, max canini {amount} kadar arttirdi. Yeni Max Can: {maxHealth}, Guncel Can: {currentHealth}");
        return maxHealth;
    }
    public float GetMaxHealth() => maxHealth;
}
