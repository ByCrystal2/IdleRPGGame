using UnityEngine;

public class ProductData : ScriptableObject
{
    public string displayName;           
    public string description;            
    public Sprite icon;
    public string productId;              // Benzersiz ID, örn: "feed_herbivore_01"    
    public CurrencyType currency;         // Altin - mucevher

    public int price;                     // 200

    public ProductCategory category;      // Gelistirme vs.
    public PurchaseType purchaseType;
    [Tooltip("Sadece Limited turu icin gecerli: satin alma adedi limiti")]
    public int maxPurchaseCount = 1;

    [SerializeField] int numberofPurchases = 1; // Urunu satin alma sayisi

    public int GetNumberOfPurchases() => numberofPurchases;
    protected void OnPurchase()
    {
        Debug.Log($"Product '{displayName}' purchased!");
        numberofPurchases++;
    }
    
}