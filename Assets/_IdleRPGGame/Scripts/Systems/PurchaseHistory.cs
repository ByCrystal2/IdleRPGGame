using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseHistory : MonoBehaviour
{
    public static PurchaseHistory Instance { get; private set; }
    [SerializedDictionary("Product ID", "Buy Count")]
    [SerializeField] SerializedDictionary<string, int> buyCounts = new SerializedDictionary<string, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // (İstersen) buradan PlayerPrefs veya JSON’dan load et
    }

    public int GetPurchaseCount(string productId)
        => buyCounts.ContainsKey(productId) ? buyCounts[productId] : 0;

    public void RecordPurchase(string productId)
    {
        if (!buyCounts.ContainsKey(productId))
            buyCounts[productId] = 0;
        buyCounts[productId]++;
        // (İstersen) hemen save et: PlayerPrefs veya bir save sistemiyle
    }
}
