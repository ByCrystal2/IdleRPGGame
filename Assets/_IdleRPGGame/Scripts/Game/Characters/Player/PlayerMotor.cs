using UnityEngine;
using StarterAssets;
using System.Collections;
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Health health;
    [SerializeField] Death death;
    [SerializeField] PlayerSkillHandler skillHandler;
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

    [Header("Attack Speed")]
    [SerializeField] private float baseAttackAnimationDuration = 1f;
    [SerializeField] private float attackSpeedScale = 0.05f;

    [SerializeField] private float weightBlendSpeed = 10f;
    private int upperBodyLayerIndex;
    private float targetLayerWeight = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        tpController = GetComponent<ThirdPersonController>();
    }
    private void Start()
    {
        if (animator != null)
        {
            upperBodyLayerIndex = animator.GetLayerIndex("UpperBody");
        }
    }
    void Update()
    {
        if (activeBow == null || animator == null) return;
        float currentWeight = animator.GetLayerWeight(upperBodyLayerIndex);
        float nextWeight = Mathf.MoveTowards(currentWeight, targetLayerWeight, Time.deltaTime * weightBlendSpeed);
        animator.SetLayerWeight(upperBodyLayerIndex, nextWeight);
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
            else
            {
                targetLayerWeight = 0f;
            }
        }
    }
    private IEnumerator FireMultipleArrowsRoutine()
    {
        // Toplam 5 ok atýlacak
        for (int i = 0; i < 5; i++)
        {
            if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= activeBow.attackRange)
            {
                activeBow.Fire(currentTarget, stats.attackDamage);
            }

            yield return new WaitForSeconds(skillHandler.multiShotArrowDelay);
        }
        animator.speed = 1f;
    }
    private IEnumerator FireGiantArrowRoutine()
    {
        float moveSpeed = tpController.MoveSpeed;
        float sprintSpeed = tpController.SprintSpeed;
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= activeBow.attackRange)
        {
            tpController.MoveSpeed = 0f; // Hareketi durdur
            tpController.SprintSpeed = 0f;

            ParticleSystem giantChargeArrowEffect = EffectHandler.Instance.GetChargeEffect(ChargeEffectType.Gian);
            ParticleSystem chargeObj = Instantiate(giantChargeArrowEffect, activeBow.arrowSpawnPoint);

            yield return new WaitForSeconds(2f);

            Destroy(chargeObj.gameObject);
            activeBow.Fire(currentTarget, stats.attackDamage * skillHandler.giantArrowDamageMultiplier, AttackEffectType.None ,6);
            skillHandler.GiantArrowActivation(false);
        }
        tpController.MoveSpeed = moveSpeed;
        tpController.SprintSpeed = sprintSpeed;
        animator.speed = 1f;
    }
    private IEnumerator FireFlamingArrowRoutine()
    {
        float moveSpeed = tpController.MoveSpeed;
        float sprintSpeed = tpController.SprintSpeed;
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= activeBow.attackRange)
        {
            tpController.MoveSpeed = 0f; // Hareketi durdur
            tpController.SprintSpeed = 0f;
            ParticleSystem fireChargeArrowEffect = EffectHandler.Instance.GetChargeEffect(ChargeEffectType.Burn);
            ParticleSystem chargeObj = Instantiate(fireChargeArrowEffect, activeBow.arrowSpawnPoint);
            yield return new WaitForSeconds(1f);
            Destroy(chargeObj.gameObject);
            activeBow.Fire(currentTarget, stats.attackDamage * skillHandler.flamingArrowDamageMultiplier,AttackEffectType.Burn, 3);
            skillHandler.FlamingArrowActivation(false);
        }
        tpController.MoveSpeed = moveSpeed;
        tpController.SprintSpeed = sprintSpeed;
        animator.speed = 1f;
    }
    void StartAttackSequence()
    {
        isAttacking = true;
        targetLayerWeight = 1f;
        float animSpeedMultiplier = 1f + (stats.attackSpeed * attackSpeedScale);

        animSpeedMultiplier = Mathf.Clamp(animSpeedMultiplier, 1f, 5f);
        Debug.Log("Calculated Animation Speed Multiplier: " + animSpeedMultiplier);

        animator.SetFloat("AttackSpeedMultiplier", animSpeedMultiplier);
        animator.SetTrigger("Attack");

        float actualAnimationDuration = baseAttackAnimationDuration / animSpeedMultiplier;
        nextAttackTime = Time.time + actualAnimationDuration;
    }
    // okun firlatilma animasyonunda tetiklenen event
    public void AnimationEvent_ReleaseArrow()
    {
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= activeBow.attackRange)
        {
            if (skillHandler.IsMultiShotActive())
            {
                animator.speed = 0f;
                StartCoroutine(FireMultipleArrowsRoutine()); // coklu atis
            }
            else if (skillHandler.IsGiantArrowActive())
            {
                animator.speed = 0f;
                StartCoroutine(FireGiantArrowRoutine()); // dev ok atisi
            }
            else if (skillHandler.IsFlamingArrowActive())
            {
                animator.speed = 0f;
                StartCoroutine(FireFlamingArrowRoutine()); // alevli ok atisi
            }
            else
            {
                activeBow.Fire(currentTarget, stats.attackDamage); // normal atis
            }
        }
        isAttacking = false;
        targetLayerWeight = 0f;
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
    public BowWithPooling GetCurrentBow() => activeBow;
    public Transform GetCurrentTarget() => currentTarget;

}
