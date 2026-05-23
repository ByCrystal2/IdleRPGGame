using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeProductData", menuName = "Scriptable Objects/Product/UpgradeProductData")]
public class UpgradeProductData : ProductData
{
    [SerializeField] UpgradeType upgradeType;
    [SerializeField] CharacterUpgradeType characterUpgradeType;
    [SerializeField] int increasingUpgradeValue = 1; // upgrate degerinin ne kadar artacagi
    [SerializeField] int currentUpgradeValue = 1; // mevcut gelistirme degeri
    public int GetCurrentUpgradeValue() => currentUpgradeValue;
    public int GetIncreasingUpgradeValue() => increasingUpgradeValue;
    public UpgradeType GetUpgradeType() => upgradeType;
    public CharacterUpgradeType GetCharacterUpgradeType() => characterUpgradeType;
    public void OnPurchase()
    {
        currentUpgradeValue += increasingUpgradeValue;
        price += price / 2; // her satin alma sonrasi fiyat %50 artar // ayarlanabilir.
        base.OnPurchase();
    }
    public bool IsMaxLevelReached()
    {
        return GetNumberOfPurchases() >= maxPurchaseCount;
    }
}
