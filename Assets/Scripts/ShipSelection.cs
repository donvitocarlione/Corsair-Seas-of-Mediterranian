using UnityEngine;

[RequireComponent(typeof(Collider))] // Ensure there's a collider for selection
public class ShipSelection : MonoBehaviour
{
    [SerializeField]
    private Renderer targetRenderer; // Optional: Directly assign a specific renderer
    
    private Material originalMaterial;
    private Renderer shipRenderer;
    private bool hasValidRenderer = false;

    void OnValidate()
    {
        // This helps catch missing renderer issues in the editor
        if (targetRenderer == null)
        {
            FindRenderer();
        }
    }

    void Awake()
    {
        FindRenderer();
        if (hasValidRenderer)
        {
            originalMaterial = shipRenderer.material;
        }
    }

    private void FindRenderer()
    {
        // Use manually assigned renderer if available
        if (targetRenderer != null)
        {
            shipRenderer = targetRenderer;
            hasValidRenderer = true;
            return;
        }

        // Try to find renderer on this object
        shipRenderer = GetComponent<Renderer>();

        // If not found, look for renderers in children
        if (shipRenderer == null)
        {
            shipRenderer = GetComponentInChildren<Renderer>();
        }

        hasValidRenderer = shipRenderer != null;

        if (!hasValidRenderer)
        {
            Debug.LogError($"[ShipSelection] No Renderer found on {gameObject.name} or its children. Please either:\n" +
                          "1. Add a Mesh Renderer component to this object or a child\n" +
                          "2. Assign a specific renderer in the inspector");

#if UNITY_EDITOR
            // Visual debugging in scene view
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 2f, Color.red, 5f);
            UnityEditor.EditorGUIUtility.PingObject(gameObject);
#endif
        }
    }

    public void SetSelected(bool selected, Material selectedMaterial)
    {
        if (!hasValidRenderer)
        {
            Debug.LogError($"[ShipSelection] Cannot set selection state for {gameObject.name} - no valid renderer found!");
            return;
        }

        if (selected && selectedMaterial != null)
        {
            shipRenderer.material = selectedMaterial;
            Debug.Log($"[ShipSelection] Selected {gameObject.name}");
        }
        else if (originalMaterial != null)
        {
            shipRenderer.material = originalMaterial;
            Debug.Log($"[ShipSelection] Deselected {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[ShipSelection] Original material is missing for {gameObject.name}");
        }
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

#if UNITY_EDITOR
    // Visual debugging in scene view
    private void OnDrawGizmos()
    {
        if (!hasValidRenderer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, "Missing Renderer!");
        }
    }
#endif
}
