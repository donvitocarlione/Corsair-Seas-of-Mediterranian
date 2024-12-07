using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    [Header("Selection Settings")]
    public Material selectedMaterial;  // Material when ship is selected
    private bool isSelected = false;    // Track selection state

    [Header("Faction Settings")]
    [SerializeField] private FactionType playerFaction;
    public FactionType PlayerFaction
    {
        get { return playerFaction; }
        set 
        { 
            playerFaction = value;
            Ship ship = GetComponent<Ship>();
            if (ship != null)
            {
                ship.faction = value;
            }
        }
    }

    private ShipSelection selectionComponent;
    private Collider shipCollider;

    void Awake()
    {
        // Get or add required components
        selectionComponent = GetComponent<ShipSelection>();
        if (selectionComponent == null)
        {
            selectionComponent = gameObject.AddComponent<ShipSelection>();
        }

        shipCollider = GetComponent<Collider>();
        if (shipCollider == null)
        {
            shipCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log($"Added BoxCollider to {gameObject.name}");
        }

        // Ensure ship has a rigidbody for proper physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false; // Ships should float
            rb.drag = 1f; // Add some water resistance
            rb.angularDrag = 1f;
            Debug.Log($"Added Rigidbody to {gameObject.name}");
        }

        // Initial setup
        isSelected = false;
    }

    public void Select()
    {
        if (selectionComponent != null && selectedMaterial != null)
        {
            isSelected = true;
            selectionComponent.SetSelected(true, selectedMaterial);
            Debug.Log($"Selected ship: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"Missing components on {gameObject.name}. SelectionComponent: {selectionComponent != null}, SelectedMaterial: {selectedMaterial != null}");
        }
    }

    public void Deselect()
    {
        if (selectionComponent != null)
        {
            isSelected = false;
            selectionComponent.SetSelected(false, null);
            Debug.Log($"Deselected ship: {gameObject.name}");
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public bool IsSelectable()
    {
        Ship ship = GetComponent<Ship>();
        if (ship == null) return false;

        ShipManager shipManager = FindObjectOfType<ShipManager>();
        if (shipManager == null) return false;

        return ship.faction == shipManager.PlayerFaction;
    }

    void OnValidate()
    {
        // Update the ship's faction when changed in inspector
        Ship ship = GetComponent<Ship>();
        if (ship != null)
        {
            ship.faction = playerFaction;
        }
    }
}
