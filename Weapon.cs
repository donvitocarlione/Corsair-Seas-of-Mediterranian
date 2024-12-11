using UnityEngine;

public enum WeaponType
{
    Cannon,
    Swivel,
    Chain,
    Grape
}

public class Weapon : MonoBehaviour
{
    [Header("Weapon Properties")]
    public WeaponType type = WeaponType.Cannon;
    public float damage = 10f;
    public float reloadTime = 3f;
    public float accuracy = 0.8f;
    public float range = 50f;
    
    [Header("Firing Arc")]
    public float minFiringAngle = -45f;
    public float maxFiringAngle = 45f;

    private float lastFireTime;
    private ProjectileManager projectileManager;

    void Start()
    {
        projectileManager = ProjectileManager.Instance;
    }

    public bool CanFireAtAngle(float angle)
    {
        return angle >= minFiringAngle && angle <= maxFiringAngle && Time.time >= lastFireTime + reloadTime;
    }

    public void Fire(Ship target)
    {
        if (!CanFireAtAngle(GetAngleToTarget(target)))
            return;

        lastFireTime = Time.time;
        Vector3 spread = CalculateSpread();
        
        projectileManager.SpawnProjectile(transform.position, target.transform.position + spread, damage, type);
        PlayFireEffects();
    }

    private Vector3 CalculateSpread()
    {
        float spread = (1f - accuracy) * 5f;
        return new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );
    }

    private float GetAngleToTarget(Ship target)
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        return Vector3.Angle(transform.right, directionToTarget);
    }

    private void PlayFireEffects()
    {
        // TODO: Add visual and sound effects for firing
        Debug.Log($"Weapon fired from {transform.parent.name}");
    }
}