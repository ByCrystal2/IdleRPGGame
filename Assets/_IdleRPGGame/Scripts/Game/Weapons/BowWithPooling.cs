using UnityEngine;
using UnityEngine.Pool;

public class BowWithPooling : MonoBehaviour
{
    [Header("Havuz Ayarları")]
    public PooledArrow arrowPrefab;
    public int defaultCapacity = 20;
    public int maxPoolSize = 50;

    [Header("Yay Teknik Ayarları")]
    public Transform arrowSpawnPoint;   // Okun çıkış noktası
    public float attackRange = 10f;     // Menzil bilgisini Player buraya bakarak kontrol edebilir
    public float fireRate = 1f;         // Saldırı hızı

    private IObjectPool<PooledArrow> arrowPool;

    void Awake()
    {
        // Unity'nin hazır havuzlama sistemini kuruyoruz
        arrowPool = new LinkedPool<PooledArrow>(
            createFunc: CreateArrow,
            actionOnGet: OnGetArrow,
            actionOnRelease: OnReleaseArrow,
            actionOnDestroy: OnDestroyArrow,
            collectionCheck: true,
            maxSize: maxPoolSize
        );
    }

    private PooledArrow CreateArrow()
    {
        PooledArrow arrowInstance = Instantiate(arrowPrefab);
        arrowInstance.SetPool(arrowPool);
        return arrowInstance;
    }

    private void OnGetArrow(PooledArrow arrow)
    {
        arrow.gameObject.SetActive(true);
        arrow.transform.position = arrowSpawnPoint.position;
        arrow.transform.rotation = arrowSpawnPoint.rotation;
    }

    private void OnReleaseArrow(PooledArrow arrow)
    {
        arrow.gameObject.SetActive(false);
    }

    private void OnDestroyArrow(PooledArrow arrow)
    {
        Destroy(arrow.gameObject);
    }

    // PlayerMotor scripti ile ateslendi!
    public void Fire(Transform targetEnemy, float damage)
    {
        PooledArrow arrow = arrowPool.Get();
        if (arrow != null)
        {
            arrow.Launch(targetEnemy, damage);
        }
    }
}