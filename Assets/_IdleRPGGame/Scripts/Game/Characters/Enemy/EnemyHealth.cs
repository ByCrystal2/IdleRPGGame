using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health
{
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI sliderText;
    [SerializeField] int stashedGold = 10;
    [SerializeField] int stashedGain = 20;
    Rigidbody rb;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        sliderText.text = $"{currentHealth}/{GetMaxHealth()}";
        healthSlider.value = (float)currentHealth / GetMaxHealth();
    }
    
    public override void TakeDamage(float damage, AttackEffectType effectType = AttackEffectType.None)
    {        
        base.TakeDamage(damage, effectType);
        healthSlider.value = (float)currentHealth / GetMaxHealth();
        sliderText.text = $"{currentHealth}/{GetMaxHealth()}";
        if (currentHealth - damage <= 0 && death.isDead)
        {
            rb.isKinematic = true; // Olunce fizik etkisi kalmasin diye kinematic yapiyoruz.
            EconomyManager.Instance.AddGold(stashedGold); // playera odul olarak stashedGold kadar altin veriyoruz.
            PlayerManager.Instance.AddGainForPlayer(stashedGain); // playera odul olarak stashedgain kadar gain veriyoruz.
            return;
        }
    }
}
