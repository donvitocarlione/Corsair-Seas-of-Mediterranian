using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [System.Serializable]
    public class DamageZone
    {
        public string zoneName;
        public float damageMultiplier = 1f;
        public bool isCriticalZone = false;
    }

    [Header("Damage Zones")]
    public DamageZone[] damageZones;

    [Header("Critical Hit")]
    public float criticalHitMultiplier = 2f;
    public float criticalHitChance = 0.1f;

    private Ship shipReference;

    void Start()
    {
        shipReference = GetComponent<Ship>();
        if (shipReference == null)
        {
            Debug.LogError("DamageSystem requires a Ship component!");
            enabled = false;
        }
    }

    public float CalculateDamage(float baseDamage, string hitZone)
    {
        float finalDamage = baseDamage;

        // Find the damage zone that was hit
        DamageZone zone = System.Array.Find(damageZones, z => z.zoneName == hitZone);
        
        if (zone != null)
        {
            finalDamage *= zone.damageMultiplier;
            
            // Check for critical hit in critical zones
            if (zone.isCriticalZone && Random.value < criticalHitChance)
            {
                finalDamage *= criticalHitMultiplier;
                Debug.Log($"Critical hit on {zone.zoneName}!");
            }
        }

        return finalDamage;
    }

    public void TakeDamageInZone(float damage, string zoneName)
    {
        if (shipReference != null)
        {
            float calculatedDamage = CalculateDamage(damage, zoneName);
            shipReference.TakeDamage(calculatedDamage);
            
            Debug.Log($"Ship took {calculatedDamage} damage in {zoneName}");
        }
    }

    public void ApplyDamageOverTime(float damagePerSecond, float duration)
    {
        StartCoroutine(DamageOverTimeCoroutine(damagePerSecond, duration));
    }

    private System.Collections.IEnumerator DamageOverTimeCoroutine(float damagePerSecond, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (shipReference != null)
            {
                shipReference.TakeDamage(damagePerSecond * Time.deltaTime);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}