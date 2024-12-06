using UnityEngine;

public class Ship : MonoBehaviour
{
    public static System.Action<Ship> OnAnyShipDestroyed;
    
    [Header("Ship Properties")]
    public FactionType faction;
    public string shipName;
    public float maxHealth = 100f;
    public float maxArmor = 10f;

    [Header("Current Status")]
    public float currentHealth;
    public float currentArmor;
    
    [Header("Combat Properties")]
    public float attackRange = 10f;
    public float attackDamage = 15f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    private ShipMovement movement;
    private Rigidbody rb;
    private Buoyancy buoyancy;

    void Awake()
    {
        SetupComponents();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
    }

    private void SetupComponents()
    {
        // Ensure required components exist
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 1000f; // Default mass for ships
        }

        movement = GetComponent<ShipMovement>();
        if (movement == null)
        {
            movement = gameObject.AddComponent<ShipMovement>();
        }

        buoyancy = GetComponent<Buoyancy>();
        if (buoyancy == null)
        {
            buoyancy = gameObject.AddComponent<Buoyancy>();
        }
    }

    public void Initialize(FactionType faction, string shipName, float initialHealth = 100f, float initialArmor = 10f)
    {
        this.faction = faction;
        this.shipName = shipName;
        this.maxHealth = initialHealth;
        this.maxArmor = initialArmor;
        currentHealth = maxHealth;
        currentArmor = maxArmor;
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
        float damageAfterArmor = Mathf.Max(0, damage - currentArmor);
        if (damageAfterArmor > 0)
        {
            currentHealth = Mathf.Max(0, currentHealth - damageAfterArmor);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        OnAnyShipDestroyed?.Invoke(this);
        Destroy(gameObject);
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
}