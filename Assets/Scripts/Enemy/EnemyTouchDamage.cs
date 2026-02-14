using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    public int damage = 10;
    public float cooldown = 0.8f;

    private float nextHitTime;

    private void OnTriggerStay(Collider other)
    {
        if (Time.time < nextHitTime) return;

        PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
            nextHitTime = Time.time + cooldown;
        }
    }
}

