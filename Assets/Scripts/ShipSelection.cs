using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))] // Ensure there's always a MeshRenderer component
public class ShipSelection : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer[] targetRenderers; // Array to hold multiple renderers
    
    private Material[] originalMaterials;
    private bool initialized = false;

    void OnValidate()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            FindRenderers();
        }
    }

    void Awake()
    {
        if (!initialized)
        {
            Initialize();
        }
    }

    void Initialize()
    {
        FindRenderers();
        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            // Store original materials
            originalMaterials = new Material[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] != null && targetRenderers[i].material != null)
                {
                    originalMaterials[i] = targetRenderers[i].material;
                }
            }
            initialized = true;
        }
        else
        {
            Debug.LogError($"[ShipSelection] No MeshRenderers found on {gameObject.name} or its children.");
        }
    }

    private void FindRenderers()
    {
        // Get all mesh renderers in children (including this object)
        targetRenderers = GetComponentsInChildren<MeshRenderer>();

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            // If no renderers found, at least get the required one on this object
            var selfRenderer = GetComponent<MeshRenderer>();
            if (selfRenderer != null)
            {
                targetRenderers = new MeshRenderer[] { selfRenderer };
            }
        }

#if UNITY_EDITOR
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            // Visual debugging in scene view
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 2f, Color.red, 5f);
            UnityEditor.EditorGUIUtility.PingObject(gameObject);
        }
#endif
    }

    public void SetSelected(bool selected, Material selectedMaterial)
    {
        if (!initialized)
        {
            Initialize();
        }

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Debug.LogError($"[ShipSelection] Cannot set selection state for {gameObject.name} - no valid renderers found!");
            return;
        }

        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                if (selected && selectedMaterial != null)
                {
                    renderer.material = selectedMaterial;
                }
                else if (!selected && originalMaterials != null)
                {
                    // Find the corresponding original material
                    int index = System.Array.IndexOf(targetRenderers, renderer);
                    if (index >= 0 && index < originalMaterials.Length && originalMaterials[index] != null)
                    {
                        renderer.material = originalMaterials[index];
                    }
                }
            }
        }

        if (selected)
        {
            Debug.Log($"[ShipSelection] Selected {gameObject.name}");
        }
        else
        {
            Debug.Log($"[ShipSelection] Deselected {gameObject.name}");
        }
    }

    private void OnDestroy()
    {
        // Clean up material references
        if (originalMaterials != null)
        {
            foreach (var material in originalMaterials)
            {
                if (material != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(material);
                    }
                    else
                    {
                        DestroyImmediate(material);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, "Missing MeshRenderer!");
        }
    }
#endif
}
