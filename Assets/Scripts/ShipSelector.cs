using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    [Header("Ship Settings")]
    public Material selectedMaterial;  // Material when ship is selected
    private Material originalMaterial;  // Store the original material
    private bool isSelected = false;    // Track selection state

    [Header("Faction Settings")]
    [SerializeField] private FactionType playerFaction;
    public FactionType PlayerFaction
    {
        get { return playerFaction; }
        set 
        { 
            playerFaction = value;
            // Update ship's faction if it has a Ship component
            Ship ship = GetComponent<Ship>();
            if (ship != null)
            {
                ship.faction = value;
            }
        }
    }

    private MeshRenderer meshRenderer;

    void Awake()
    {
        // Get the MeshRenderer component
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // Store the original material
            originalMaterial = meshRenderer.material;
        }
        else
        {
            Debug.LogWarning("MeshRenderer not found on ship");
        }
    }

    public void Select()
    {
        if (meshRenderer != null && selectedMaterial != null)
        {
            isSelected = true;
            meshRenderer.material = selectedMaterial;
        }
    }

    public void Deselect()
    {
        if (meshRenderer != null && originalMaterial != null)
        {
            isSelected = false;
            meshRenderer.material = originalMaterial;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
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
