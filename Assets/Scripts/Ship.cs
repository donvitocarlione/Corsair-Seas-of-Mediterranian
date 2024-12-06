using UnityEngine;

public class Ship : MonoBehaviour
{
    public FactionType faction;
    public string shipName;
    public float health;
    public float armor;

    public void Initialize(FactionType faction, string shipName)
    {
        this.faction = faction;
        this.shipName = shipName;
        // Initialize health and armor values here if needed (e.g., from ShipStats ScriptableObject)
    }

    public void TakeDamage(float damage)
    {
        float damageAfterArmor = damage - armor;
        if (damageAfterArmor > 0)
        {
            health -= damageAfterArmor;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        // Implement movement logic here
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 10f);
    }
}
