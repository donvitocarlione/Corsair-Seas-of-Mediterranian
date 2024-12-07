using UnityEngine;

public class Ship : MonoBehaviour
{
    public FactionType faction;
    public string shipName;
    public float health = 100f;
    public float speed = 10f;
    public float turnSpeed = 5f;

    private ShipMovement shipMovement;
    private Rigidbody rb;
    private ShipSelector shipSelector;
    private MeshRenderer meshRenderer;
    private bool isSelected = false;

    void Awake()
    {
        // Get or add required components
        shipMovement = GetComponent<ShipMovement>();
        if (shipMovement == null) shipMovement = gameObject.AddComponent<ShipMovement>();

        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        // Configure Rigidbody
        rb.useGravity = false; // Since we're using buoyancy
        rb.mass = 1000f;       // Ships are heavy
        rb.drag = 1f;          // Water resistance
        rb.angularDrag = 1f;   // Rotational resistance

        // Get ship selector if exists
        shipSelector = GetComponent<ShipSelector>();

        // Get mesh renderer
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(FactionType newFaction, string newName)
    {
        Debug.Log($"Initializing ship {newName} for faction {newFaction}");
        faction = newFaction;
        shipName = newName;

        // If we have a ship selector, update its faction
        if (shipSelector != null)
        {
            shipSelector.PlayerFaction = newFaction;
            Debug.Log($"Updated ShipSelector faction to {newFaction}");
        }

        // Ensure the ship is visible
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        // Make sure the GameObject is active
        gameObject.SetActive(true);

        // Initialize movement component if needed
        if (shipMovement != null)
        {
            shipMovement.enabled = true;
            shipMovement.speed = speed;
            shipMovement.turnSpeed = turnSpeed;
        }
    }

    public void MoveTo(Vector3 position)
    {
        if (shipMovement != null)
        {
            shipMovement.SetTargetPosition(position);
        }
    }

    public void StopMoving()
    {
        if (shipMovement != null)
        {
            shipMovement.ClearTarget();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Notify ShipManager
        if (ShipManager.Instance != null)
        {
            ShipManager.Instance.OnShipDestroyed(this);
        }

        // Destroy the game object
        Destroy(gameObject);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (shipSelector != null)
        {
            if (selected)
                shipSelector.Select();
            else
                shipSelector.Deselect();
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    // Optionally override some Unity messages to ensure proper behavior
    void OnEnable()
    {
        if (meshRenderer != null) meshRenderer.enabled = true;
    }

    void Start()
    {
        // Additional initialization if needed
        Debug.Log($"Ship {shipName} started, faction: {faction}");
    }
}
