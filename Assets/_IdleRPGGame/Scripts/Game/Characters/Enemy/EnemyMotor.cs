using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    EnemyStats stats;

    public float changeDirectionTime = 3f;

    [Header("Scare Settings")]
    public Transform playerTransform;
    public float scareDistance = 5f; // Oyuncu bu mesafeye yaklastiginda kacar.

    [Header("Patrol (Devriye) Zamanları")]
    public float walkDuration = 3f;       // yuruyecegi sure
    public float minWaitTime = 1f;       // hedefe varinca bekleyecegi min sure
    public float maxWaitTime = 4f;

    private Rigidbody rb;
    [SerializeField] EnemyArea enemyArea;

    private Vector3 movementDirection;
    private float directionTimer;
    private float stateTimer;
    private bool isFleeing = false;
    private bool isWaiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<EnemyStats>();

        if (enemyArea == null)
        {
            Debug.LogError("EnemyArea scripti bulunamadi!");
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
        StartWalking();
    }

    void Update()
    {
        if (enemyArea == null) return;
        if (stats.health.death.isDead) return;


        // 1. Her zaman öncelikli olarak oyuncu yakınlık kontrolü yap
        CheckPlayerDistance();

        // 2. Eğer oyuncudan KAÇMIYORSA normal devriye zamanlayıcısını çalıştır
        if (!isFleeing)
        {
            stateTimer -= Time.deltaTime;

            if (stateTimer <= 0)
            {
                if (isWaiting)
                {
                    // Bekleme süresi bittiyse yürümeye başla
                    StartWalking();
                }
                else
                {
                    // Yürüme süresi bittiyse dur ve beklemeye geç
                    StartWaiting();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (enemyArea == null) return;
        if (stats.health.death.isDead) return;
        Move();
        ClampToArea();
    }

    void StartWalking()
    {
        isWaiting = false;

        // Rastgele yön seç (X ve Z ekseninde)
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);
        movementDirection = new Vector3(randomX, 0, randomZ).normalized;

        // Ne kadar süre yürüyeceğini ayarla
        stateTimer = walkDuration;
    }

    void StartWaiting()
    {
        isWaiting = true;
        movementDirection = Vector3.zero; // Hareketi kes

        // Belirlediğin aralıkta rastgele bir bekleme süresi seç
        stateTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    void CheckPlayerDistance()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= scareDistance)
        {
            // Oyuncu yakınsa: Bekleme durumunu boz ve kaçış moduna geç
            isFleeing = true;
            isWaiting = false;

            // Oyuncudan ters yöne doğru kaçış vektörü hesapla
            movementDirection = (transform.position - playerTransform.position).normalized;
            movementDirection.y = 0;
        }
        else
        {
            // Oyuncu menzilden çıktıysa ve hala kaçma modundaysak normal devriyeye geri dön
            if (isFleeing)
            {
                isFleeing = false;
                StartWalking(); // Kaldığı yerden rastgele yürümeye başlasın
            }
        }
    }

    void Move()
    {
        // Eğer bekliyorsa hızı 0 yap, kaçıyorsa runSpeed, normal yürüyorsa moveSpeed
        float currentSpeed = 0f;
        if (!isWaiting)
        {
            currentSpeed = isFleeing ? stats.moveSpeed + 1.5f : stats.moveSpeed;
        }

        Vector3 targetVelocity = movementDirection * currentSpeed;

        // Rigidbody hızını uygula (Y yerçekimi korunur)
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // Eğer hareket ediyorsa ve yönü sıfır değilse o yöne baksın
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    // Daire siniri kontrolu
    void ClampToArea()
    {
        Vector3 center = enemyArea.transform.position;
        float radius = enemyArea.areaRadius;

        Vector3 offset = transform.position - center;
        offset.y = 0;

        float distance = offset.magnitude;

        if (distance > radius)
        {
            Vector3 clampedPosition = center + (offset.normalized * radius);
            clampedPosition.y = transform.position.y;
            transform.position = clampedPosition;

            if (Vector3.Dot(movementDirection, offset) > 0)
            {
                if (isFleeing)
                {
                    movementDirection = Vector3.ProjectOnPlane(movementDirection, offset.normalized).normalized;
                }
                else
                {
                    StartWaiting();
                }
            }
        }
    }
}