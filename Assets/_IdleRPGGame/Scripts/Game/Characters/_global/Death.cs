using UnityEngine;

public class Death : MonoBehaviour
{
    public void Die()
    {
        // Olme animasyonu tetikleme ve objeyi yok etme kodlari
        Destroy(gameObject);
    }
}
