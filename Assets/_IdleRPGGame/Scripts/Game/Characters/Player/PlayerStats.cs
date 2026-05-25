using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StarterAssets;
public class PlayerStats : Stats
{
    [Header("For Player")]
    public int currentXP;
    // Indis kolayligi icin 0. elemani 0 biraktim. level 1 -> gereken: 100, level 2 -> gereken: 300...
    public int[] LevelUpStages = new int[] { 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600 };
    public int MaxLevel;
    public int level = 1;
    public Health health;
    ThirdPersonController player;

    protected override void Awake()
    {
        base.Awake();
        MaxLevel = LevelUpStages.Length - 1;
        player = GetComponent<ThirdPersonController>();

        level = PlayerPrefs.GetInt("Player_Level", 1);
        currentXP = PlayerPrefs.GetInt("Player_Exp", 0);
    }

    private void Start()
    {
        UpdatePlayerStats();
    }
    // Animasyon hýzýný güncelleyen yeni fonksiyonumuz
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
        player.SprintSpeed = moveSpeed;
        // arayuz guncellensin diye 0 gonderiyoruz.
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
        player.SprintSpeed = moveSpeed+0.5f;
        Debug.Log($"Player Stats Updated: AttackDamage: {attackDamage}, AttackSpeed: {attackSpeed}, Health: {health.GetMaxHealth()}, MoveSpeed: {moveSpeed}");
    }

    public void GainXP(int xpAmount)
    {
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
        
        while (level < MaxLevel && currentXP >= LevelUpStages[level])
        {            
            currentXP -= LevelUpStages[level];
            level++;

            Debug.Log($"Level Up! New Level: {level}");
        }
        if (level >= MaxLevel)
        {
            currentXP = 0;
            Debug.Log("Maksimum seviyeye ulasildi!");
        }
    }
}