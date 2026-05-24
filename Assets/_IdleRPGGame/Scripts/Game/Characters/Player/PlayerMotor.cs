using UnityEngine;
using StarterAssets;
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Health health;
    [SerializeField] Death death;
    public Health GetHealthScipt() => health;
    public Death GetDeathScript() => death;
    public PlayerStats GetPlayerStats() => stats;
    ThirdPersonController tpController;

    Transform currentTarget;
    Death targetDeathScript;
    Animator animator;
    public LayerMask enemyLayer;
    [Header("Arrow Settings")]
    [SerializeField] private BowWithPooling activeBow; // Mevcut yay
    private float nextAttackTime;
    private bool isAttacking = false;
    private Collider[] hitColliders = new Collider[10]; // Ayni anda menzile girebilecek max dusman (performans icin onemli!)

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (activeBow == null || animator == null) return;
        if (currentTarget != null)
        {
            RotateTowardsTarget(currentTarget);
        }

        if (Time.time >= nextAttackTime && !isAttacking)
        {
            currentTarget = FindClosestEnemy(activeBow.attackRange);
            
            if (currentTarget != null)
            {
                StartAttackSequence();
            }
        }
    }
    void StartAttackSequence()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        nextAttackTime = Time.time + (1f / activeBow.fireRate);
    }
    // okun firlatilma animasyonunda tetiklenen event
    public void AnimationEvent_ReleaseArrow()
    {
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= activeBow.attackRange)
        {
            activeBow.Fire(currentTarget,stats.attackDamage);
        }
        isAttacking = false;
    }
    Transform FindClosestEnemy(float range)
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, range-0.5f, hitColliders, enemyLayer);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < numColliders; i++)
        {
            Vector3 directionToTarget = hitColliders[i].transform.position - transform.position;
            float dSQ = directionToTarget.sqrMagnitude;

            if (dSQ < minDistance)
            {
                minDistance = dSQ;
                closestEnemy = hitColliders[i].transform;
            }
        }
        return closestEnemy;
    }

    void RotateTowardsTarget(Transform target)
    {
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

}
