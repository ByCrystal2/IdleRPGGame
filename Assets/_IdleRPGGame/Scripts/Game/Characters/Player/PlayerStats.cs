using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StarterAssets;
public class PlayerStats : Stats
{
    [Header("For Player")]
    public int currentXP;
    // İndis kolaylığı için 0. elemanı 0 bıraktım. level 1 -> gereken: 100, level 2 -> gereken: 300...
    public int[] LevelUpStages = new int[] { 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600 };
    public int MaxLevel;
    public int level = 1;
    public Health health;
    ThirdPersonController player;
    protected override void Awake()
    {
        base.Awake();
        // Maksimum ulaşılabilecek seviye dizinin son elemanının indeksidir
        MaxLevel = LevelUpStages.Length - 1;
        player = GetComponent<ThirdPersonController>();
        // Oyun açıldığında kayıtlı verileri geri yükle
        level = PlayerPrefs.GetInt("Player_Level", 1);
        currentXP = PlayerPrefs.GetInt("Player_Exp", 0);
    }

    private void Start()
    {
        UpdatePlayerStats();
    }

    public void UpdatePlayerStats()
    {
        List<UpgradeProductData> upgradeDatas = EconomyManager.Instance.GetProducts(ProductCategory.Upgrade, CurrencyType.Gold).OfType<UpgradeProductData>().ToList();
        foreach (var product in upgradeDatas)
        {
            int currentUpgradeValue = product.GetCurrentUpgradeValue();
            _ = (product.GetCharacterUpgradeType()) switch
            {
                CharacterUpgradeType.AttackDamage => attackDamage = currentUpgradeValue,
                CharacterUpgradeType.AttackSpeed => attackSpeed = currentUpgradeValue,
                CharacterUpgradeType.Health => health.IncreaseMaxHealth(currentUpgradeValue),
                CharacterUpgradeType.MoveSpeed => moveSpeed = currentUpgradeValue,
                _ => default
            };
        }
        player.MoveSpeed = moveSpeed;
        // Sahne başında arayüzün güncellenmesi için 0 EXP vererek tetikliyoruz
        GainXP(0);
    }

    public void UpdatePlayerStats(CharacterUpgradeType characterUpgradeType)
    {
        List<UpgradeProductData> upgradeDatas = EconomyManager.Instance.GetProducts(ProductCategory.Upgrade, CurrencyType.Gold).OfType<UpgradeProductData>().ToList();
        UpgradeProductData product = upgradeDatas.FirstOrDefault(p => p.GetCharacterUpgradeType() == characterUpgradeType);

        if (product != null)
        {
            int currentUpgradeValue = product.GetCurrentUpgradeValue();
            _ = (characterUpgradeType) switch
            {
                CharacterUpgradeType.AttackDamage => attackDamage = currentUpgradeValue,
                CharacterUpgradeType.AttackSpeed => attackSpeed = currentUpgradeValue,
                CharacterUpgradeType.Health => health.IncreaseMaxHealth(currentUpgradeValue),
                CharacterUpgradeType.MoveSpeed => moveSpeed = currentUpgradeValue,
                _ => default
            };
        }
        player.MoveSpeed = moveSpeed;
        Debug.Log($"Player Stats Updated: AttackDamage: {attackDamage}, AttackSpeed: {attackSpeed}, Health: {health.GetMaxHealth()}, MoveSpeed: {moveSpeed}");
    }

    public void GainXP(int xpAmount)
    {
        // Eğer zaten son seviyedeyse daha fazla EXP almasın
        if (level >= MaxLevel)
        {
            EventBus<PlayerStats>.Publish(EventType.ExpChange, this);
            return;
        }

        currentXP += xpAmount;

        // Seviye atlama kontrolunu yap (Artan EXP iceride islenecek)
        CheckLevelUp();

        PlayerPrefs.SetInt("Player_Exp", currentXP);
        PlayerPrefs.SetInt("Player_Level", level);

        EventBus<PlayerStats>.Publish(EventType.ExpChange, this);
    }

    public void CheckLevelUp()
    {
        // while döngüsü sayesinde gelen yüksek EXP'lerde peş peşe seviye atlayabilir.
        // level < MaxLevel kontrolü dizinin dışına taşmayı (Index out of range) engeller.
        while (level < MaxLevel && currentXP >= LevelUpStages[level])
        {
            // Mevcut EXP'den o seviye için gereken miktarı düşüyoruz (Kalan EXP sonraki seviyeye devreder)
            currentXP -= LevelUpStages[level];
            level++;

            Debug.Log($"Level Up! New Level: {level}");
            // Buraya seviye atlama görsel efektleri veya sesleri eklenebilir
        }

        // Eğer son seviyeye ulaştıysa EXP'yi sabitle veya sıfırla (Tercihe bağlı)
        if (level >= MaxLevel)
        {
            currentXP = 0;
            Debug.Log("Maksimum seviyeye ulaşıldı!");
        }
    }
}