using UnityEngine;

public class Ship : MonoBehaviour
{
    public static System.Action<Ship> OnAnyShipDestroyed;
    
    public FactionType faction;
    public string shipName;
    public float health = 100f;
    public float armor = 10f;
    private ShipMovement movement;

    void Awake()
    {
        movement = GetComponent<ShipMovement>();
        if (movement == null)
        {
            movement = gameObject.AddComponent<ShipMovement>();
        }
    }

    public void Initialize(FactionType faction, string shipName, float initialHealth = 100f, float initialArmor = 10f)
    {
        this.faction = faction;
        this.shipName = shipName;
        this.health = initialHealth;
        this.armor = initialArmor;
    }

    public void TakeDamage(float damage)
    {
        float damageAfterArmor = damage - armor;
        if (damageAfterArmor > 0)
        {
            health -= damageAfterArmor;
            if (health <= 0)
            {
                OnAnyShipDestroyed?.Invoke(this);
                Destroy(gameObject);
            }
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
        if (movement != null)
        {
            movement.ClearTarget();
        }
    }
}