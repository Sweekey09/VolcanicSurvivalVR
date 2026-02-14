using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20;

private void OnTriggerEnter(Collider other)
{
    EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
    if (enemy != null)
    {
        enemy.TakeDamage(damage);
        Destroy(gameObject);
        }
    }
}
