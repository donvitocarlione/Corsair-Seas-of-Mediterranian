using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShipSelection : MonoBehaviour
{
    private Material[] originalMaterials;
    private Renderer shipRenderer;
    private bool isSelected = false;

    void Awake()
    {
        // Get the renderer component
        shipRenderer = GetComponent<Renderer>();
        if (shipRenderer == null)
        {
            // If not found on this object, try to find it in children
            shipRenderer = GetComponentInChildren<Renderer>();
        }

        if (shipRenderer != null)
        {
            // Store all original materials
            originalMaterials = shipRenderer.materials;
            Debug.Log($"ShipSelection initialized on {gameObject.name} with {originalMaterials.Length} materials");
        }
        else
        {
            Debug.LogError($"No Renderer component found on {gameObject.name} or its children");
        }
    }

    public void SetSelected(bool selected, Material selectedMaterial)
    {
        if (shipRenderer == null) 
        {
            Debug.LogError($"No Renderer component on {gameObject.name}");
            return;
        }

        if (selected && selectedMaterial == null)
        {
            Debug.LogError($"Selected material is null on {gameObject.name}");
            return;
        }

        isSelected = selected;
        
        if (selected)
        {
            // Create new array of materials with the same length as original
            Material[] newMaterials = new Material[originalMaterials.Length];
            // Fill all slots with the selected material
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = selectedMaterial;
            }
            shipRenderer.materials = newMaterials;
        }
        else
        {
            // Restore original materials
            shipRenderer.materials = originalMaterials;
        }
        
        Debug.Log($"Ship {gameObject.name} selection state changed to: {selected}");
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    void OnDestroy()
    {
        // Ensure original materials are restored when object is destroyed
        if (shipRenderer != null && originalMaterials != null)
        {
            shipRenderer.materials = originalMaterials;
        }
    }
}
