using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField] public Animator animator;
    public bool isDead { get; private set; }
    public void Die()
    {
        // Olme animasyonu tetikleme ve objeyi yok etme kodlari
        if (animator == null) { Debug.LogWarning($"Karaktere ({transform.parent.name} animator atanmamis!)"); return; }

        isDead = true;
        animator.SetTrigger("Die");
        Destroy(gameObject,1f);
    }
}
