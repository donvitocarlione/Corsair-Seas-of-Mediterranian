using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DamageSystem : MonoBehaviour
{
    [System.Serializable]
    public class DamageZone
    {
        public string zoneName;
        public float damageMultiplier = 1f;
        public bool isCriticalZone = false;
        public float currentHealth = 100f;
        public float maxHealth = 100f;
        public GameObject visualDamageEffect;  // Visual effect for this zone
        public bool isRepairable = true;
    }

    [Header("Damage Zones")]
    public DamageZone[] damageZones;

    [Header("Critical Hit")]
    public float criticalHitMultiplier = 2f;
    public float criticalHitChance = 0.1f;

    [Header("Effects")]
    public GameObject fireEffect;
    public GameObject waterSprayEffect;
    public AudioClip hitSound;
    public AudioClip criticalHitSound;

    [Header("Events")]
    public UnityEvent onCriticalHit;
    public UnityEvent onZoneDestroyed;

    private Ship shipReference;
    private AudioSource audioSource;
    private Dictionary<string, GameObject> activeEffects = new Dictionary<string, GameObject>();

    void Start()
    {
        InitializeComponents();
        InitializeDamageZones();
    }

    private void InitializeComponents()
    {
        shipReference = GetComponent<Ship>();
        if (shipReference == null)
        {
            Debug.LogError("DamageSystem requires a Ship component!");
            enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void InitializeDamageZones()
    {
        foreach (var zone in damageZones)
        {
            zone.currentHealth = zone.maxHealth;
        }
    }

    public float CalculateDamage(float baseDamage, string hitZone, out bool isCritical)
    {
        float finalDamage = baseDamage;
        isCritical = false;

        DamageZone zone = System.Array.Find(damageZones, z => z.zoneName == hitZone);
        
        if (zone != null)
        {
            finalDamage *= zone.damageMultiplier;
            
            if (zone.isCriticalZone && Random.value < criticalHitChance)
            {
                isCritical = true;
                finalDamage *= criticalHitMultiplier;
                onCriticalHit?.Invoke();
                PlayCriticalHitEffects(zone);
            }
        }

        return finalDamage;
    }

    public void TakeDamageInZone(float damage, string zoneName)
    {
        if (shipReference == null) return;

        bool isCritical;
        float calculatedDamage = CalculateDamage(damage, zoneName, out isCritical);
        
        DamageZone zone = System.Array.Find(damageZones, z => z.zoneName == zoneName);
        if (zone != null)
        {
            zone.currentHealth -= calculatedDamage;
            
            if (zone.currentHealth <= 0 && !activeEffects.ContainsKey(zoneName))
            {
                ZoneDestroyed(zone);
            }
            else
            {
                PlayHitEffects(zone, isCritical);
            }

            shipReference.TakeDamage(calculatedDamage);
        }
    }

    public void ApplyDamageOverTime(float damagePerSecond, float duration)
    {
        StartCoroutine(DamageOverTimeCoroutine(damagePerSecond, duration));
    }

    private void ZoneDestroyed(DamageZone zone)
    {
        onZoneDestroyed?.Invoke();
        
        if (fireEffect != null)
        {
            GameObject effect = Instantiate(fireEffect, transform.position, Quaternion.identity);
            effect.transform.parent = transform;
            activeEffects[zone.zoneName] = effect;
        }

        if (zone.zoneName.ToLower().Contains("hull"))
        {
            ApplyDamageOverTime(10f, 999f); // Continuous hull damage
        }
    }

    private void PlayHitEffects(DamageZone zone, bool isCritical)
    {
        if (zone.visualDamageEffect != null)
        {
            Instantiate(zone.visualDamageEffect, transform.position, Quaternion.identity);
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(isCritical ? criticalHitSound : hitSound);
        }
    }

    private void PlayCriticalHitEffects(DamageZone zone)
    {
        // Additional critical hit effects
        if (zone.visualDamageEffect != null)
        {
            var effect = Instantiate(zone.visualDamageEffect, transform.position, Quaternion.identity);
            var particleSystem = effect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startSizeMultiplier *= 2f; // Bigger effect for critical hits
            }
        }
    }

    public void RepairZone(string zoneName, float repairAmount)
    {
        DamageZone zone = System.Array.Find(damageZones, z => z.zoneName == zoneName);
        if (zone != null && zone.isRepairable)
        {
            zone.currentHealth = Mathf.Min(zone.currentHealth + repairAmount, zone.maxHealth);
            
            if (zone.currentHealth > 0 && activeEffects.ContainsKey(zoneName))
            {
                Destroy(activeEffects[zoneName]);
                activeEffects.Remove(zoneName);

                if (waterSprayEffect != null)
                {
                    Instantiate(waterSprayEffect, transform.position, Quaternion.identity);
                }
            }
        }
    }

    public float GetZoneHealthPercentage(string zoneName)
    {
        DamageZone zone = System.Array.Find(damageZones, z => z.zoneName == zoneName);
        return zone != null ? (zone.currentHealth / zone.maxHealth) * 100f : 0f;
    }

    private System.Collections.IEnumerator DamageOverTimeCoroutine(float damagePerSecond, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration && shipReference != null)
        {
            shipReference.TakeDamage(damagePerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}