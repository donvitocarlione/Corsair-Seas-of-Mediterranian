using UnityEngine;

public class ShipSelection : MonoBehaviour
{
    private Material originalMaterial;
    private Renderer shipRenderer;

    void Awake()
    {
        // Try to find renderer on this object
        shipRenderer = GetComponent<Renderer>();

        // If not found, look for renderers in children
        if (shipRenderer == null)
        {
            shipRenderer = GetComponentInChildren<Renderer>();
        }

        if (shipRenderer != null)
        {
            originalMaterial = shipRenderer.material;
        }
        else
        {
            Debug.LogWarning($"No Renderer found on {gameObject.name} or its children. Please add a Mesh Renderer component.");
        }
    }

    public void SetSelected(bool selected, Material selectedMaterial)
    {
        if (shipRenderer == null)
        {
            Debug.LogWarning($"No Renderer available on {gameObject.name} or its children");
            return;
        }

        if (selected && selectedMaterial != null)
        {
            shipRenderer.material = selectedMaterial;
        }
        else if (originalMaterial != null)
        {
            shipRenderer.material = originalMaterial;
        }

        Debug.Log($"Set selection state for {gameObject.name}: {selected}");
    }

    private void OnDestroy()
    {
        // Clean up material references
        if (originalMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(originalMaterial);
            }
            else
            {
                DestroyImmediate(originalMaterial);
            }
        }
    }
}
