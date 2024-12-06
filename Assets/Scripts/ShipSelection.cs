using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShipSelection : MonoBehaviour
{
    private Material originalMaterial;
    private Renderer shipRenderer;
    private bool isSelected = false;

    void Awake()
    {
        shipRenderer = GetComponent<Renderer>();
        if (shipRenderer != null)
        {
            originalMaterial = shipRenderer.material;
            Debug.Log($"ShipSelection initialized on {gameObject.name}");
        }
        else
        {
            Debug.LogError($"No Renderer component found on {gameObject.name}");
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
        shipRenderer.material = selected ? selectedMaterial : originalMaterial;
        Debug.Log($"Ship {gameObject.name} selection state changed to: {selected}");
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}