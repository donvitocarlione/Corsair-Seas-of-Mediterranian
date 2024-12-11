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
        InitializeHandler();
    }
    
    private void Awake()
    {
        InitializeHandler();
    }

    private void InitializeHandler()
    {
        // Always try to initialize
        Debug.Log($"[ShipSelectionHandler] Initializing handler for {gameObject.name}");
        
        // Get ship reference
        shipReference = GetComponent<Ship>();
        if (shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] No Ship component found on {gameObject.name}");
            return;
        }

        // Find all renderers in the ship hierarchy if not already found
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            FindTargetRenderers();
            StoreOriginalMaterials();
        }

        // Ensure this object is on the correct layer
        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
        }

        // Hide selection indicator at start
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }

        isInitialized = true;
        Debug.Log($"[ShipSelectionHandler] Initialization complete for {gameObject.name}");
    }

    private void FindTargetRenderers()
    {
        // Get all mesh renderers in children
        targetRenderers = GetComponentsInChildren<MeshRenderer>(true); // Include inactive objects
        
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Debug.LogError($"[ShipSelectionHandler] No mesh renderers found in {gameObject.name}");
            return;
        }

        Debug.Log($"[ShipSelectionHandler] Found {targetRenderers.Length} renderers in {gameObject.name}");
    }

    private void StoreOriginalMaterials()
    {
        if (targetRenderers == null) return;

        originalMaterials = new Material[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
            {
                // Store a reference to the original shared material
                originalMaterials[i] = targetRenderers[i].sharedMaterial;
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
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(true);
        }
        ApplySelectedMaterial();
        return true;
    }

    public void Deselect()
    {
        Debug.Log($"[ShipSelectionHandler] Deselecting ship {gameObject.name}");
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
        RestoreOriginalMaterials();
    }

    private bool CanBeSelected()
    {
        if (!isInitialized)
        {
            InitializeHandler(); // Try to initialize if not done
        }

        if (shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Cannot select - shipReference is null on {gameObject.name}");
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

        return true;
    }

    private void ApplySelectedMaterial()
    {
        if (selectedMaterial == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Selected material is not assigned on {gameObject.name}");
            return;
        }

        if (targetRenderers == null)
        {
            FindTargetRenderers();
            if (targetRenderers == null) return;
        }

        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                renderer.material = selectedMaterial;
            }
        }
    }

    private void RestoreOriginalMaterials()
    {
        if (targetRenderers == null || originalMaterials == null) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null && originalMaterials[i] != null)
            {
                targetRenderers[i].material = originalMaterials[i];
            }
        }
    }
}
