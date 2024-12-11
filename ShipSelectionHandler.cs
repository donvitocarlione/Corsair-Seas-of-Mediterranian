using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShipSelectionHandler : MonoBehaviour
{
    [Header("Selection Visual Settings")]
    [SerializeField]
    private Material selectedMaterial;
    [SerializeField]
    private GameObject selectionIndicator;
    
    [Header("Selection Settings")]
    [SerializeField]
    private LayerMask selectableLayerMask = Physics.DefaultRaycastLayers;

    private MeshRenderer[] targetRenderers;
    private Material[] originalMaterials;
    private Ship shipReference;
    private bool isInitialized = false;

    private void OnEnable()
    {
        if (!isInitialized)
        {
            InitializeHandler();
        }
        
        // Ensure selection indicator starts hidden
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
    }
    
    private void Awake()
    {
        InitializeHandler();
    }

    private void InitializeHandler()
    {
        if (isInitialized) return;

        Debug.Log($"[ShipSelectionHandler] Initializing handler for {gameObject.name}");
        
        // Get ship reference
        shipReference = GetComponent<Ship>();
        if (shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] No Ship component found on {gameObject.name}");
            return;
        }

        // Find all renderers in the ship hierarchy
        FindTargetRenderers();

        // Store original materials
        StoreOriginalMaterials();

        // Ensure this object is on the correct layer
        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
            Debug.Log($"[ShipSelectionHandler] Set layer to Ship for {gameObject.name}");
        }

        // Validate selected material
        if (selectedMaterial == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Selected material is not assigned on {gameObject.name}");
        }

        isInitialized = true;
    }

    private void FindTargetRenderers()
    {
        // Get all mesh renderers in children
        targetRenderers = GetComponentsInChildren<MeshRenderer>();
        
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Debug.LogError($"[ShipSelectionHandler] No mesh renderers found in {gameObject.name}");
            return;
        }

        // Log found renderers for debugging
        Debug.Log($"[ShipSelectionHandler] Found {targetRenderers.Length} renderers in {gameObject.name}:");
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            Debug.Log($"- Renderer {i}: {targetRenderers[i].name}");
        }
    }

    private void StoreOriginalMaterials()
    {
        if (targetRenderers == null) return;

        originalMaterials = new Material[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
            {
                originalMaterials[i] = targetRenderers[i].material;
                Debug.Log($"[ShipSelectionHandler] Stored original material for {targetRenderers[i].name}: {originalMaterials[i].name}");
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public bool Select()
    {
        if (!CanBeSelected())
        {
            Debug.LogWarning($"[ShipSelectionHandler] Cannot select ship {gameObject.name} - conditions not met");
            return false;
        }

        Debug.Log($"[ShipSelectionHandler] Selecting ship {gameObject.name}");
        ApplySelectedMaterial();
        ShowSelectionIndicator(true);
        return true;
    }

    public void Deselect()
    {
        Debug.Log($"[ShipSelectionHandler] Deselecting ship {gameObject.name}");
        RestoreOriginalMaterials();
        ShowSelectionIndicator(false);
    }

    private bool CanBeSelected()
    {
        if (!isInitialized || shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Cannot select - not properly initialized on {gameObject.name}");
            return false;
        }

        if (shipReference.ShipOwner == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Cannot select - ship has no owner on {gameObject.name}");
            return false;
        }

        if (!(shipReference.ShipOwner is Player))
        {
            Debug.LogWarning($"[ShipSelectionHandler] Cannot select - ship's owner is not a Player on {gameObject.name}");
            return false;
        }

        if (shipReference.IsSinking)
        {
            Debug.LogWarning($"[ShipSelectionHandler] Cannot select - ship is sinking {gameObject.name}");
            return false;
        }

        return true;
    }

    private void ApplySelectedMaterial()
    {
        if (selectedMaterial == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Cannot apply selection - selected material is null on {gameObject.name}");
            return;
        }

        if (targetRenderers == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Cannot apply selection - no target renderers found on {gameObject.name}");
            return;
        }

        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                renderer.material = selectedMaterial;
                Debug.Log($"[ShipSelectionHandler] Applied selected material to {renderer.name}");
            }
        }
    }

    private void RestoreOriginalMaterials()
    {
        if (targetRenderers == null || originalMaterials == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Cannot restore materials - references are null on {gameObject.name}");
            return;
        }

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null && originalMaterials[i] != null)
            {
                targetRenderers[i].material = originalMaterials[i];
                Debug.Log($"[ShipSelectionHandler] Restored original material for {targetRenderers[i].name}");
            }
        }
    }

    private void ShowSelectionIndicator(bool show)
    {
        if (selectionIndicator != null)
        {
            Debug.Log($"[ShipSelectionHandler] {(show ? "Showing" : "Hiding")} selection indicator for {gameObject.name}");
            selectionIndicator.SetActive(show);
        }
        else
        {
            Debug.LogWarning($"[ShipSelectionHandler] Selection indicator is null on {gameObject.name}");
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            // Clean up materials to prevent memory leaks
            if (originalMaterials != null)
            {
                foreach (var material in originalMaterials)
                {
                    if (material != null)
                    {
                        Destroy(material);
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        if (selectedMaterial == null)
        {
            Debug.LogWarning("Please assign a Selected Material in the inspector");
        }

        if (selectionIndicator == null)
        {
            Debug.LogWarning("Please assign a Selection Indicator in the inspector");
        }
    }
}