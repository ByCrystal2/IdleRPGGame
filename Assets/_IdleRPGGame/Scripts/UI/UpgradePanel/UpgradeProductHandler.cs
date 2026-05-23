using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeProductHandler : MonoBehaviour
{
    [SerializeField] UpgradeProductData targetProduct;
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_DisplayNameText;
    [SerializeField] TextMeshProUGUI m_UpgradeValueText;
    [SerializeField] TextMeshProUGUI m_DescriptionText;
    [SerializeField] TextMeshProUGUI m_buttonValueText;
    [SerializeField] Slider m_UpgradeLevelSlider;
    [SerializeField] TextMeshProUGUI m_UpgradeLevelSliderText;
    [SerializeField] Button m_UpgradeButton;
    PlayerMotor player;
    public void SetTargetProduct(UpgradeProductData targetProduct) => this.targetProduct = targetProduct;
    private void Awake()
    {
        m_UpgradeButton.onClick.AddListener(() => OnProductUpgradeButtonClick(targetProduct.GetUpgradeType(), targetProduct.GetCharacterUpgradeType()));
        player = FindFirstObjectByType<PlayerMotor>();
    }
    private void OnEnable()
    {
        UpdateUI();
    }
    void OnProductUpgradeButtonClick(UpgradeType upgradeType, CharacterUpgradeType characterUpgradeType)
    {
        Debug.Log("Product Upgrade button clicked!");

        // Upgrade islemi burda olacak.
        var results = EconomyManager.Instance.TryPurchase(targetProduct);
        if (!results.result)
        {
            Debug.Log($"Purchase failed: {results.message}");            
            return;
        }
        targetProduct.OnPurchase();
        UpdateUI();
        if(characterUpgradeType != CharacterUpgradeType.None)
        player.GetPlayerStats().UpdatePlayerStats(characterUpgradeType);
    }
    void UpdateUI()
    {
        
        m_Icon.sprite = targetProduct.icon;
        m_DisplayNameText.text = targetProduct.displayName;
        
        m_DescriptionText.text = targetProduct.description;

        if (targetProduct.IsMaxLevelReached())
        {
            Debug.Log("Maximum upgrade level reached.");
            m_UpgradeButton.interactable = false;
            m_buttonValueText.text = "Max Level";
            m_UpgradeValueText.text = "";
        }
        else
        {
            m_buttonValueText.text = $"{targetProduct.price}\nUpgrade";
            m_UpgradeValueText.text = $"{targetProduct.GetCurrentUpgradeValue()} > {targetProduct.GetCurrentUpgradeValue() + targetProduct.GetIncreasingUpgradeValue()}";
        }


        m_UpgradeLevelSlider.value = (float)targetProduct.GetNumberOfPurchases() / targetProduct.maxPurchaseCount;
        m_UpgradeLevelSliderText.text = $"{targetProduct.GetNumberOfPurchases()}/{targetProduct.maxPurchaseCount}";
    }
}
