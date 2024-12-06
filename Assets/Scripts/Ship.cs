using UnityEngine;
using System;

public class Ship : MonoBehaviour
{
    public static Action<Ship> OnAnyShipDestroyed;
    
    [Header("Ship Properties")]
    public FactionType faction;
    public string shipName;
    public float maxHealth = 100f;
    public float maxArmor = 10f;
    
    [Header("Current Status")]
    public float currentHealth;
    public float currentArmor;
    private bool isInitialized = false;
    
    [Header("Combat Properties")]
    public float attackRange = 10f;
    public float attackDamage = 15f;
    public float attackCooldown = 2f;
    public float armorDegradationRate = 0.1f; // How quickly armor degrades when hit
    private float lastAttackTime;

    [Header("Effects")]
    public GameObject hitEffect; // Optional hit effect prefab
    public AudioClip hitSound; // Optional hit sound
    
    private ShipMovement movement;
    private Rigidbody rb;
    private Buoyancy buoyancy;
    private AudioSource audioSource;

    void Awake()
    {
        SetupComponents();
    }

    void Start()
    {
        if (!isInitialized)
        {
            Initialize(faction, shipName, maxHealth, maxArmor);
        }
    }

    private void SetupComponents()
    {
        try
        {
            // Setup Rigidbody
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                ConfigureRigidbody();
            }

            // Setup Movement
            movement = GetComponent<ShipMovement>();
            if (movement == null)
            {
                movement = gameObject.AddComponent<ShipMovement>();
            }

            // Setup Buoyancy
            buoyancy = GetComponent<Buoyancy>();
            if (buoyancy == null)
            {
                buoyancy = gameObject.AddComponent<Buoyancy>();
            }

            // Setup Audio
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && hitSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f; // 3D sound
                audioSource.maxDistance = 50f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting up ship components: {e.Message}");
            enabled = false;
        }
    }

    private void ConfigureRigidbody()
    {
        rb.useGravity = true;
        rb.mass = 1000f;
        rb.drag = 1f;
        rb.angularDrag = 2f;
    }

    public void Initialize(FactionType faction, string shipName, float initialHealth = 100f, float initialArmor = 10f)
    {
        this.faction = faction;
        this.shipName = shipName;
        this.maxHealth = initialHealth;
        this.maxArmor = initialArmor;
        currentHealth = maxHealth;
        currentArmor = maxArmor;
        isInitialized = true;
    }

    public bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }

    public void Attack(Ship target)
    {
        if (!CanAttack() || target == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= attackRange)
        {
            target.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentArmor > 0)
        {
            // Armor reduces damage and degrades
            float armorDamage = Mathf.Min(damage, currentArmor);
            currentArmor = Mathf.Max(0, currentArmor - (armorDamage * armorDegradationRate));
            damage = Mathf.Max(0, damage - armorDamage);
        }

        if (damage > 0)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            PlayHitEffects();

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void PlayHitEffects()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private void Die()
    {
        try
        {
            if (OnAnyShipDestroyed != null)
            {
                OnAnyShipDestroyed.Invoke(this);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in ship destruction event: {e.Message}");
        }
        finally
        {
            Destroy(gameObject);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (movement != null)
        {
            movement.SetTargetPosition(targetPosition);
        }
        else
        {
            Debug.LogError($"No ShipMovement component found on {gameObject.name}");
        }
    }

    public void StopMoving()
    {
        movement?.ClearTarget();
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth * 100f;
    }

    public float GetArmorPercentage()
    {
        return currentArmor / maxArmor * 100f;
    }
}