using UnityEngine;
using UnityEngine.Pool;

public class PooledArrow : MonoBehaviour
{
    [Header("Ok Hareket Ayarları")]
    [SerializeField] private float speed = 25f;       // okun gidis hizi
    [SerializeField] private float maxLifetime = 3f;  // havuza dunme suresi (hic bir seye carmadan gidebilecegi max mesafe)

    private Transform target;
    private IObjectPool<PooledArrow> objectPool;
    private float lifetimeTimer;
    private bool m_HasHit = false;
    float m_Damage;

    private void OnEnable()
    {
        m_HasHit = false;
    }
    public void SetPool(IObjectPool<PooledArrow> pool)
    {
        objectPool = pool;
    }

    // Hedefi tanimla
    public void Launch(Transform enemyTarget, float damage)
    {
        target = enemyTarget;
        m_Damage = damage;
        lifetimeTimer = maxLifetime;
    }

    void Update()
    {
        
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.forward = direction;
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        //ok bisiye carpmazsa geri doner
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
                enemyHealth.TakeDamage(m_Damage);
                enemyHealth.death.animator.SetTrigger("Hit");
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
            objectPool.Release(this);
        }
        else
        {
            // havuz sistemi yoksa objeyi yok et
            Destroy(gameObject);
        }
    }
}