using UnityEngine;

public class PlayerHealth : Health
{
    public override void TakeDamage(float damage, AttackEffectType effectType = AttackEffectType.None)
    {
        base.TakeDamage(damage, effectType);
        EventBus<Health>.Publish(EventType.UpgradeCharacter, this); // Can degisikliklerini dinleyen eventlere guncel can bilgisini gonderiyoruz
    }
}
