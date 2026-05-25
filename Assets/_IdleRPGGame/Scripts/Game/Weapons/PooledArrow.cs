using UnityEngine;
using UnityEngine.Pool;

public class PooledArrow : MonoBehaviour
{
    [Header("Ok Hareket Ayarları")]
    [SerializeField] private float speed = 25f;       // okun gidis hizi
    [SerializeField] private float maxLifetime = 3f;  // havuza dunme suresi (hic bir seye carmadan gidebilecegi max mesafe)

    AttackEffectType m_EffectType = AttackEffectType.None;
    private Transform target;
    private IObjectPool<PooledArrow> objectPool;
    private float lifetimeTimer;
    private bool m_HasHit = false;
    float m_Damage;
    Vector3 defaultScale;

    private void OnEnable()
    {
        m_HasHit = false;
        defaultScale = transform.localScale;
    }
    public void SetPool(IObjectPool<PooledArrow> pool)
    {
        objectPool = pool;
    }

    // Hedefi tanimla
    public void Launch(Transform enemyTarget, float damage, AttackEffectType effectType = AttackEffectType.None)
    {
        target = enemyTarget;
        m_Damage = damage;
        m_EffectType = effectType;
        lifetimeTimer = maxLifetime;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Okun Y eksenini (yukarı eksenini) hedefe çevirir
            transform.up = direction;
        }
        else
        {
            // Hedef yoksa, artık yukarı ekseni neresiyse o yöne doğru gitmeye devam eder
            transform.position += transform.up * speed * Time.deltaTime;
        }

        // Ok bir şeye çarpmazsa havuz geri döner
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0)
        {
            ReleaseToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_HasHit) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth) && !enemyHealth.death.isDead)
            {
                m_HasHit = true;
                enemyHealth.TakeDamage(m_Damage, m_EffectType);
                enemyHealth.death.animator.SetTrigger("Hit");

                if (m_EffectType == AttackEffectType.Burn)
                {
                    ParticleSystem targetExplosion = EffectHandler.Instance.GetExplosionEffect(ExplosionEffectType.Burn);

                    if(targetExplosion != null)
                    {
                        ParticleSystem explosionObj = Instantiate(targetExplosion, transform.position, Quaternion.identity);

                        if (explosionObj != null)
                        {                        
                            Destroy(explosionObj, 2f); // Patlama bitince sahneden silinsin
                        }
                    }

                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, (int)ExplosionEffectType.Burn);
                    foreach (var hitCollider in hitColliders)
                    {
                        EnemyHealth areaEnemy = hitCollider.GetComponent<EnemyHealth>();

                        if (areaEnemy != null)
                        {
                            areaEnemy.TakeDamage(m_Damage * 1.5f,m_EffectType);
                        }
                    }
                }          

                
            }

            // hasarin ardindan oku havuza geri gonder
            ReleaseToPool();
        }
    }

    // Havuza geri donme
    private void ReleaseToPool()
    {
        target = null;

        if (objectPool != null)
        {
            transform.localScale = defaultScale;
            objectPool.Release(this);
        }
        else
        {
            // havuz sistemi yoksa objeyi yok et
            Destroy(gameObject);
        }
    }
}