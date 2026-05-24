using UnityEngine;

public class PlayerHealth : Health
{
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        EventBus<Health>.Publish(EventType.UpgradeCharacter, this); // Can degisikliklerini dinleyen eventlere guncel can bilgisini gonderiyoruz
    }
}
