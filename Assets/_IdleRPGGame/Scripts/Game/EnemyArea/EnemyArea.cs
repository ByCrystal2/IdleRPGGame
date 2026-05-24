using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    [Header("Area Settings")]
    public float areaRadius = 5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}
