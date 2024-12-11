using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private float damage;
    private WeaponType type;
    private float speed;
    private bool isInitialized;

    public void Initialize(Vector3 target, float dmg, WeaponType weaponType, float projectileSpeed)
    {
        targetPosition = target;
        damage = dmg;
        type = weaponType;
        speed = projectileSpeed;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            OnReachTarget();
        }
    }

    void OnReachTarget()
    {
        // TODO: Add impact effects
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Ship hitShip = collision.gameObject.GetComponent<Ship>();
        if (hitShip != null)
        {
            hitShip.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}