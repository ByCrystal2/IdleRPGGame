using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
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
    public void VibrationSubscribedObjects(CurrencyType currencyType,Color textColor, float duration = 0.2f)
    {
        List<SubsSup> subsToVibrate = economySubscribes.Where(s => s.Value == currencyType).Select(s => s.Key).ToList();
        foreach (var subscribe in subsToVibrate)
        {           
            switch (subscribe.SubsType)
            {
                case EconomySubscribeType.Price:
                    TextMeshProUGUI text = subscribe.TargetObject.GetComponent<TextMeshProUGUI>();

                    if (text != null)
                    {
                        
                        Color originalColor = text.color;
                        text.transform.DOKill();
                        text.DOKill();


                        text.DOColor(textColor, duration / 2)
                            .OnComplete(() => text.DOColor(originalColor, duration / 2));
                        text.transform.DOShakePosition(duration, strength: 10f, vibrato: 20, randomness: 90, fadeOut: true);
                    }
                    break;

                default:
                    break;
            }
        }
        Handheld.Vibrate();
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
