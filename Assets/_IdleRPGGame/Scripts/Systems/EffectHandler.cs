using AYellowpaper.SerializedCollections;
using SingletonSystem;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : PersistentSinleton<EffectHandler>
{
    [SerializedDictionary("AttackEffectType", "ParticleEffects")]
    [SerializeField] SerializedDictionary<AttackEffectType, List<ParticleSystem>> effects = new SerializedDictionary<AttackEffectType, List<ParticleSystem>>();

    [SerializedDictionary("ExplosionEffectType", "ExplosionEffects")]
    [SerializeField] SerializedDictionary<ExplosionEffectType, List<ParticleSystem>> explosionEffects = new SerializedDictionary<ExplosionEffectType, List<ParticleSystem>>();

    [SerializedDictionary("ChargeEffectType", "ChargeEffects")]
    [SerializeField] SerializedDictionary<ChargeEffectType, List<ParticleSystem>> chargeEffects = new SerializedDictionary<ChargeEffectType, List<ParticleSystem>>();


    public ParticleSystem GetEffect(AttackEffectType effectType)
    {
        if (effects.TryGetValue(effectType, out List<ParticleSystem> _effects) && _effects.Count > 0)
        {
            return _effects[Random.Range(0, _effects.Count)];
        }
        return null; // Efekt yoksa null!
    }

    public ParticleSystem GetExplosionEffect(ExplosionEffectType effectType)
    {
        if (explosionEffects.TryGetValue(effectType, out List<ParticleSystem> _effects) && _effects.Count > 0)
        {
            return _effects[Random.Range(0, _effects.Count)];
        }
        return null; // Efekt yoksa null!
    }

    public ParticleSystem GetChargeEffect(ChargeEffectType effectType)
    {
        if (chargeEffects.TryGetValue(effectType, out List<ParticleSystem> _effects) && _effects.Count > 0)
        {
            return _effects[Random.Range(0, _effects.Count)];
        }
        return null; // Efekt yoksa null!
    }
}
public enum ExplosionEffectType // Patlama efektleri capini belirler, orn: burn patlamasi daha buyuk bir alana sahip olabilir, freeze patlamasi ise daha kucuk bir alana sahip olabilir.
{
    None = 0,
    Burn = 3,
    Freeze = 2
}
public enum ChargeEffectType // Saldiri oncesi sarj efektleri, capini belirler, orn: burn sarji daha buyuk bir alana sahip olabilir, freeze sarji ise daha kucuk bir alana sahip olabilir.
{
    None,
    Burn,
    Gian,
    Freeze
}
