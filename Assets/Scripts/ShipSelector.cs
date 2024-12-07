using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    [Header("Selection Settings")]
    public Material selectedMaterial;  // Material when ship is selected
    private bool isSelected = false;    // Track selection state

    private Ship shipComponent;

    void Awake()
    {
        // Get or add required components
        shipComponent = GetComponent<Ship>();
        if (shipComponent == null)
        {
            shipComponent = gameObject.AddComponent<Ship>();
            Debug.Log($"Added Ship component to {gameObject.name}");
        }

        Collider shipCollider = GetComponent<Collider>();
        if (shipCollider == null)
        {
            shipCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log($"Added BoxCollider to {gameObject.name}");
        }

        // Initial setup
        isSelected = false;
    }

    public void Select()
    {
        if (shipComponent != null)
        {
            isSelected = true;
            shipComponent.Select();
            Debug.Log($"Selected ship: {gameObject.name}");
        }
    }

    public void Deselect()
    {
        if (shipComponent != null)
        {
            isSelected = false;
            shipComponent.Deselect();
            Debug.Log($"Deselected ship: {gameObject.name}");
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public bool IsSelectable()
    {
        return shipComponent != null && shipComponent.faction == FactionType.Player;
    }
}
