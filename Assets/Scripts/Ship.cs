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
    private float lastAttackTime;

    private ShipMovement movement;
    private Rigidbody rb;
    private Buoyancy buoyancy;
    private DamageSystem damageSystem;

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

            // Setup DamageSystem
            damageSystem = GetComponent<DamageSystem>();
            if (damageSystem == null)
            {
                damageSystem = gameObject.AddComponent<DamageSystem>();
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

    public void Attack(Ship target, string targetZone = "Hull")
    {
        if (!CanAttack() || target == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= attackRange)
        {
            // Use the damage system to apply damage to a specific zone
            target.TakeDamageInZone(attackDamage, targetZone);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        // This method is called by the DamageSystem after calculating zone-specific damage
        if (damage > 0)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void TakeDamageInZone(float damage, string zoneName)
    {
        if (damageSystem != null)
        {
            damageSystem.TakeDamageInZone(damage, zoneName);
        }
        else
        {
            // Fallback to basic damage if no damage system
            TakeDamage(damage);
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

    public void ApplyDamageOverTime(float damagePerSecond, float duration)
    {
        if (damageSystem != null)
        {
            damageSystem.ApplyDamageOverTime(damagePerSecond, duration);
        }
    }
}