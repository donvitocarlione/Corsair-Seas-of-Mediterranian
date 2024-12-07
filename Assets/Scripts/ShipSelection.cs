using UnityEngine;

public class ShipSelection : MonoBehaviour
{
    private Material originalMaterial;
    private Renderer shipRenderer;

    void Awake()
    {
        shipRenderer = GetComponent<Renderer>();
        if (shipRenderer != null)
        {
            originalMaterial = shipRenderer.material;
        }
        else
        {
            Debug.LogWarning($"No Renderer found on {gameObject.name}");
        }
    }

    public void SetSelected(bool selected, Material selectedMaterial)
    {
        if (shipRenderer == null)
        {
            Debug.LogWarning($"No Renderer on {gameObject.name}");
            return;
        }

        if (selected && selectedMaterial != null)
        {
            shipRenderer.material = selectedMaterial;
        }
        else
        {
            shipRenderer.material = originalMaterial;
        }

        Debug.Log($"Set selection state for {gameObject.name}: {selected}");
    }
}
