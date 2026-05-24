using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradePanelController : MonoBehaviour
{
    [SerializeField] UpgradeProductHandler upgradeProductPrefab; 
    [SerializeField] Transform upgradeProductsContent;
    PlayerStats playerStats;
    private void Awake()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
    }
    private void Start()
    {
        List<ProductData> products = EconomyManager.Instance.GetProducts(ProductCategory.Upgrade, CurrencyType.Gold);

        List<UpgradeProductData> charUpgradeProducts = products.OfType<UpgradeProductData>().Where(x => x.GetUpgradeType() == UpgradeType.CharacterUpgrade).ToList();
        upgradeProductsContent.gameObject.SetActive(false);
        foreach (var product in charUpgradeProducts)
        {
            UpgradeProductHandler currentProductUI = Instantiate(upgradeProductPrefab, upgradeProductsContent);
            currentProductUI.SetTargetProduct(product);

        }
        upgradeProductsContent.gameObject.SetActive(true);
    }
    public void AddGoldForTesting() // Test amacli fonksiyon, 1000 altin ekler
    {
        EconomyManager.Instance.AddGold(1000);
        

    }
    public void AddExpdForTesting(int _exp) // Test amacli fonksiyon, istediginiz kadar exp ekleyebilirsiniz
    {
        playerStats.GainXP(_exp);
    }
}
