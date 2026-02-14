using UnityEngine;

public class AutoGun : MonoBehaviour
{
    [Header("References")]
    public Transform muzzle;

    [Header("Magazine (ScriptableObject)")]
    public MagazineData magazine;

    [Header("Runtime")]
    public float bulletLife = 3f;

    private float nextFireTime;
    private bool isFiring;

    // XR trigger events
    public void StartFiring() => isFiring = true;
    public void StopFiring() => isFiring = false;

    private void Update()
    {
        if (!isFiring) return;
        if (magazine == null || magazine.bulletPrefab == null || muzzle == null) return;

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / magazine.fireRate);
            FireOne();
        }
    }

    private void FireOne()
    {
        GameObject bullet = Instantiate(magazine.bulletPrefab, muzzle.position, muzzle.rotation);

        // set damage on bullet (Projectile)
        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null)
            proj.damage = magazine.damage;

        // push bullet forward
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = muzzle.forward * magazine.bulletSpeed;

        Destroy(bullet, bulletLife);
    }
}