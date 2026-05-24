using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Char ID")]
    public string characterName = "Bilinmeyen Karakter";
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    [Tooltip("Eger karakter saldirmayacaksa (Sadece kacan dusmansa) bunu kapatabilirsiniz.")]
    public bool canAttack = true;
    public float attackDamage = 10f;
    public float attackSpeed = 1f; // Saniyede kac vurus yapabilecegini belirler
    public int attackRange = 2; // Saldiri menzili
    public float fireRate = 1f; // Saniyede kac ok atabilir
    protected virtual void Awake()
    {

    }
}
