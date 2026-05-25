using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Death))]
public class Health : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] private float maxHealth = 50f;
    public float currentHealth { get; private set; }
    AttackEffectType currentEffectType = AttackEffectType.None;
    ParticleEffectHandler currentEffectHandler = null;
    [HideInInspector]
    public Death death;
    protected virtual void Awake()
    {
        death = GetComponent<Death>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if(currentEffectHandler != null)
        {
            currentEffectHandler.transform.position = transform.position; // Efekt objesini karakterin pozisyonuna sabitler
        }
    }

    public virtual void TakeDamage(float damage, AttackEffectType effectType = AttackEffectType.None)
    {
        // 1. ilk anlik hasari normal sekilde veriyoruz
        ApplyDamageCalculation(damage);

        if (currentEffectType != AttackEffectType.None) return; // Zaten bir efekt uygulaniyorsa yeni bir efekt uygulamiyoruz

        int effectDuration = (int)effectType;
        if (effectType != AttackEffectType.None && effectDuration > 0)
        {
            currentEffectType = effectType;
            if(effectType == AttackEffectType.Burn) // hasar efektleri surekli hasar verir.
            StartCoroutine(ApplyOverTimeDamage(damage, effectDuration));
            else if (effectType == AttackEffectType.Freeze) // freeze efekti surekli hasar vermese de karakteri yavaslatabilir veya hareketini durdurabilir. Burada sadece log attik.
            {
                Debug.Log($"{gameObject.name} freeze efektine maruz kaldi! Hareket hizi azaldi.");
            }
        }
    }

    private void ApplyDamageCalculation(float damage)
    {
        if (currentHealth <= 0) return; 

        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
            death.Die();
            Debug.Log($"{gameObject.name} öldü.");
        }
        else
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} adli obje {damage} kadar hasar aldi! Kalan Can: {currentHealth}");
        }
    }
    
    private IEnumerator ApplyOverTimeDamage(float damagePerSecond, int duration)
    {
        int remainingSeconds = duration;
        ParticleSystem targetEffect = EffectHandler.Instance.GetEffect(currentEffectType);
        ParticleSystem effectObj = Instantiate(targetEffect, transform.position, Quaternion.identity);
        currentEffectHandler = effectObj.GetComponent<ParticleEffectHandler>();
        currentEffectHandler.SetLifeTime(remainingSeconds);
        effectObj.gameObject.SetActive(true);
        while (remainingSeconds > 0 && currentHealth > 0)
        {
            yield return new WaitForSeconds(1f);

            Debug.Log($"Efekt hasari vuruluyor... Kalan sure: {remainingSeconds - 1}sn");
            ApplyDamageCalculation(damagePerSecond);
            death.animator.SetTrigger("Hit");
            remainingSeconds--; 
        }
        if(effectObj != null) 
        effectObj.gameObject.SetActive(false);

        currentEffectHandler = null;
        currentEffectType = AttackEffectType.None; // Efekt suresi bittiginde effect kalkiyor.
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
public enum AttackEffectType // Effect sureleri yanlarinde yazmaktadir.
{
    None,
    Freeze = 2,
    Burn = 5,
    Poison = 8
}