using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus<T>
{

    private static Dictionary<EventType, Action<T>> eventTable = new Dictionary<EventType, Action<T>>();

    //  Son gönderim zamanlarını sakliyoruz
    private static Dictionary<EventType, float> lastEventTimes = new Dictionary<EventType, float>();

    //  Minimum bekleme süresi (spam engelleme)
    private static float minInterval = 0.05f; // 50 ms, gerekirse ayarlanabilir

    public static void Subscribe(EventType eventType, Action<T> listener)
    {
        if (!eventTable.ContainsKey(eventType))
            eventTable[eventType] = delegate { };

        eventTable[eventType] += listener;
    }

    public static void Unsubscribe(EventType eventType, Action<T> listener)
    {
        if (eventTable.ContainsKey(eventType))
            eventTable[eventType] -= listener;
    }

    public static void Publish(EventType eventType, T param)
    {
        // Daha once bu event yakin zamanda tetiklendiyse, yoksay
        if (lastEventTimes.ContainsKey(eventType) &&
            Time.realtimeSinceStartup - lastEventTimes[eventType] < minInterval)
        {
            return; // Spam engellendi
        }

        lastEventTimes[eventType] = Time.realtimeSinceStartup;

        if (eventTable.ContainsKey(eventType))
            eventTable[eventType].Invoke(param);
    }    
}
public enum EventType
{
    UpgradeCharacter,
    ExpChange,
    PlayerLevelUp,
    EnemyDefeated,
    ItemPurchased,
}