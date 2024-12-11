using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager instance;
    public static ProjectileManager Instance => instance;

    [Header("Projectile Settings")]
    public GameObject cannonballPrefab;
    public float projectileSpeed = 50f;
    public float gravity = 9.81f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnProjectile(Vector3 startPos, Vector3 targetPos, float damage, WeaponType type)
    {
        GameObject projectileObj = Instantiate(cannonballPrefab, startPos, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        
        if (projectile != null)
        {
            projectile.Initialize(targetPos, damage, type, projectileSpeed);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}