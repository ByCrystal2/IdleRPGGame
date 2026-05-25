using UnityEngine;

public class ParticleEffectHandler : MonoBehaviour
{
    [SerializeField] AttackEffectType attackEffectType;

    float _lifeTime = 3f;

    public void SetLifeTime(float lifeTime)
    {
        _lifeTime = lifeTime;
    }
    private void OnEnable()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
            Destroy(gameObject, _lifeTime);
        }
    }
}