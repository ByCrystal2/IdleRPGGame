using AYellowpaper.SerializedCollections;
using SingletonSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EconomySubscribeSystem))]
public class EconomyManager : PersistentSinleton<EconomyManager>
{
    [Header("Product")]
    [SerializeField] ProductCatalog ProductCatalog;
    [Header("Currency Definitions")]
    [SerializeField] List<CurrencyData> currencyDefinitions;

    // Oyuncu bakiyelerini tutan sozluk
    [Header("Para birimi degerleri runetime disinda editorden degistirilmemelidir.")]
    [SerializedDictionary("Name", "Value")]
    [SerializeField] SerializedDictionary<string, int> balances = new SerializedDictionary<string, int>();
    public DetailsOfEconomy detailsOfEconomy { get; private set; }
    public EconomySubscribeSystem economySubscribeSystem { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        LoadStartingBalances();
        detailsOfEconomy = new DetailsOfEconomy(currencyDefinitions);
        economySubscribeSystem = GetComponent<EconomySubscribeSystem>();
    }
    private void Start()
    {
        economySubscribeSystem.UpdateSubscribedObjectValues();
    }
    public (bool result, string message) TryPurchase(ProductData data)
    {
        int bought = PurchaseHistory.Instance.GetPurchaseCount(data.productId);

        // Limit kontrolü
        switch (data.purchaseType)
        {
            case PurchaseType.NonConsumable:
                if (bought >= 1)
                {
                    return (false, "This product has already been purchased.");
                }
                break;

            case PurchaseType.Limited:
                if (bought >= data.maxPurchaseCount)
                {
                    return (false, $"You can purchase this product up to {data.maxPurchaseCount} times.");
                }
                break;

            case PurchaseType.Consumable:
            default:
                // sinirsiz, ekstra kontrol yok
                break;
        }

        // Bakiye kontrolu
        string currencyName = "";
        foreach (var splitingString in data.currency.ToString().Split('_'))
        {
            if (data.currency.ToString().Split('_').Last() == splitingString)
                currencyName += splitingString;
            else
                currencyName += splitingString + " ";

        }
        bool success = ChangeBalance(currencyName, -data.price);
        if (!success)
        {
            return (false, "Insufficient balance!");
        }
        // Satin alma kaydi ve etki

        PurchaseHistory.Instance.RecordPurchase(data.productId);
        economySubscribeSystem.UpdateSubscribedObjectValues();
        return (true, $"Purchase completed! New balance: {GetBalance(currencyName)}");
    }

    /// <summary>
    /// Bakiyeyi getirir.
    /// </summary>
    public int GetBalance(string currencyName)
    {
        return balances.ContainsKey(currencyName) ? balances[currencyName] : 0;
    }
    public int GetBalance(CurrencyType currencyType)
    {
        string currencyName = "";
        foreach (var splitingString in currencyType.ToString().Split('_'))
        {
            if (currencyType.ToString().Split('_').Last() == splitingString)
                currencyName += splitingString;
            else
                currencyName += splitingString + " ";
        }
        return GetBalance(currencyName);
    }
    /// <summary>
    /// Belirli miktar ekler/cikarir. (NOT: her bakiye degisim isleminde, islemin gerceklesip gerceklesmedigi kontrol edilmelidir [ture/false])
    /// </summary>
    bool ChangeBalance(string currencyName, int delta)
    {
        if (!balances.ContainsKey(currencyName)) return false;

        Debug.Log($"{currencyName} adli bakiye turu degistirilmek istendi. Gonderilen tutar:{delta}, yeni deger:{balances[currencyName] + delta}");
        int newAmount = balances[currencyName] + delta;
        if (newAmount < 0) return false;  // Yetersiz bakiye
        balances[currencyName] = newAmount;
        //UI and other processes
        return true;
    }

    /// <summary>
    /// Baslangic bakiyelerini yukler.
    /// </summary>
    void LoadStartingBalances()
    {
        foreach (var def in currencyDefinitions)
            balances[def.currencyName] = def.startingAmount;
    }
    public List<ProductData> GetProducts(ProductCategory category, CurrencyType currencyType)
    {
        return ProductCatalog.allProducts.FindAll(x => x.category == category && x.currency == currencyType);
    }

    public class DetailsOfEconomy
    {
        List<CurrencyData> currencyDefinitions;
        public DetailsOfEconomy(List<CurrencyData> currencyDefinitions)
        {
            this.currencyDefinitions = currencyDefinitions;
        }
        public Sprite GetCurrencySprite(CurrencyType currencyType)
        {
            return currencyDefinitions.Where(x => x.currency == currencyType).SingleOrDefault().icon;
        }
    }
}
public enum CurrencyType { Gold, Gem} // _ => " " ('_' isareti donusturulme durumlarinda bosluk olarak kullanilacaktir.
public enum ProductCategory { Upgrade, Market1, Market2, SpecialMarket1}
public enum PurchaseType
{
    Consumable,      // Sinirsiz satin alinabilir 
    NonConsumable,   // Bir kereye mahsus satin alinir 
    Limited          // Belirli sayida satin alinabilir 
}