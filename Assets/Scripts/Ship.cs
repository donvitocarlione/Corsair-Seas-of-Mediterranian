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
        if (shipMovement == null)
        {
            shipMovement = gameObject.AddComponent<ShipMovement>();
            Debug.Log($"Added ShipMovement to {gameObject.name}");
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log($"Added Rigidbody to {gameObject.name}");
        }

        // Configure Rigidbody
        rb.useGravity = false; // Since we're using buoyancy
        rb.mass = 1000f;       // Ships are heavy
        rb.drag = 1f;          // Water resistance
        rb.angularDrag = 1f;   // Rotational resistance
        rb.constraints = RigidbodyConstraints.FreezePositionY; // Keep ships at water level

        // Get ship selector if exists
        shipSelector = GetComponent<ShipSelector>();
        if (shipSelector == null)
        {
            shipSelector = gameObject.AddComponent<ShipSelector>();
            Debug.Log($"Added ShipSelector to {gameObject.name}");
        }

        // Get mesh renderer
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning($"No MeshRenderer found on {gameObject.name}");
        }

        // Ensure there's a collider for selection
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            // Make the collider slightly larger than the mesh for easier selection
            boxCollider.size = transform.localScale * 1.1f;
            Debug.Log($"Added BoxCollider to {gameObject.name}");
        }
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
        else
        {
            Debug.LogWarning($"No ShipSelector found during initialization of {newName}");
        }

        // Ensure the ship is visible
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        // Make sure the GameObject is active
        gameObject.SetActive(true);

        // Initialize movement component
        if (shipMovement != null)
        {
            shipMovement.enabled = true;
            shipMovement.speed = speed;
            shipMovement.turnSpeed = turnSpeed;
            Debug.Log($"Initialized ShipMovement for {newName}");
        }
    }

    public void MoveTo(Vector3 position)
    {
        if (shipMovement != null)
        {
            Debug.Log($"{shipName} moving to position: {position}");
            shipMovement.SetTargetPosition(position);
        }
        else
        {
            Debug.LogError($"No ShipMovement component found on {shipName}");
        }
    }

    public void StopMoving()
    {
        if (shipMovement != null)
        {
            shipMovement.ClearTarget();
            Debug.Log($"{shipName} stopped moving");
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{shipName} took {damage} damage. Health: {health}");
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{shipName} has been destroyed");
        
        // Notify ShipManager
        ShipManager shipManager = FindObjectOfType<ShipManager>();
        if (shipManager != null)
        {
            shipManager.OnShipDestroyed(this);
        }

        // Deselect if selected
        if (isSelected)
        {
            SetSelected(false);
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
            {
                shipSelector.Select();
                Debug.Log($"{shipName} selected");
            }
            else
            {
                shipSelector.Deselect();
                Debug.Log($"{shipName} deselected");
            }
        }
        else
        {
            Debug.LogWarning($"Cannot set selection state - no ShipSelector on {shipName}");
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    void OnEnable()
    {
        if (meshRenderer != null) meshRenderer.enabled = true;
        Debug.Log($"Ship {shipName} enabled");
    }

    void Start()
    {
        Debug.Log($"Ship {shipName} started, faction: {faction}");
    }

    void OnValidate()
    {
        // Update ShipSelector faction when changed in inspector
        if (shipSelector != null && shipSelector.PlayerFaction != faction)
        {
            shipSelector.PlayerFaction = faction;
            Debug.Log($"Updated ShipSelector faction to match Ship: {faction}");
        }
    }
}
