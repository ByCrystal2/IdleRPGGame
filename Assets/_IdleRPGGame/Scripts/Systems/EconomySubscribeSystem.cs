using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

public class EconomySubscribeSystem : MonoBehaviour
{
    [SerializedDictionary("Subscribe Infos", "Subscribe Currency")]
    [SerializeField] SerializedDictionary<SubsSup, CurrencyType> economySubscribes = new SerializedDictionary<SubsSup, CurrencyType>();

    public void SubscribeEconomy(SubsSup subs, CurrencyType currencyType)
    {
        economySubscribes.Add(subs, currencyType);
        UpdateSubscribedObjectValues();
    }
    public void UpdateSubscribedObjectValues()
    {
        foreach (var subscribes in economySubscribes)
        {
            var subsValues = subscribes.Key;

            switch (subsValues.SubsType)
            {
                case EconomySubscribeType.Price:
                    subsValues.TargetObject.GetComponent<TextMeshProUGUI>().text = EconomyManager.Instance.GetBalance(subscribes.Value).ToString();
                    break;
                default:
                    break;
            }
        }
    }
    [System.Serializable]
    public class SubsSup
    {
        public SubsSup(GameObject targetObject, EconomySubscribeType subsType)
        {
            TargetObject = targetObject;
            SubsType = subsType;
        }
        public GameObject TargetObject;
        public EconomySubscribeType SubsType;
    }
    public enum EconomySubscribeType
    {
        Price
    }
}
