using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShipSelectionHandler : MonoBehaviour
{
    [Header("Selection Visual Settings")]
    [SerializeField]
    private Material selectedMaterial;
    [SerializeField]
    private GameObject selectionIndicator;
    
    [Header("Material Settings")]
    [SerializeField]
    private Color highlightColor = new Color(1f, 1f, 0f, 0.3f);
    [SerializeField]
    private float highlightIntensity = 0.5f;
    
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
        // Store original materials immediately after initialization
        if (isInitialized)
        {
            StoreOriginalMaterials();
        }
    }

    private void InitializeHandler()
    {
        if (isInitialized) return;

        Debug.Log($"[ShipSelectionHandler] Initializing handler for {gameObject.name}");
        
        shipReference = GetComponent<Ship>();
        if (shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] No Ship component found on {gameObject.name}");
            return;
        }

        FindTargetRenderers();

        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
        }

        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }

        isInitialized = true;
    }

    private void FindTargetRenderers()
    {
        targetRenderers = GetComponentsInChildren<MeshRenderer>(true);
        Debug.Log($"[ShipSelectionHandler] Found {targetRenderers.Length} renderers in {gameObject.name}");

        // Log all found renderers and their current materials
        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                Debug.Log($"Found renderer: {renderer.name} with material: {renderer.sharedMaterial.name}");
            }
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
                // Store a reference to the original shared material
                originalMaterials[i] = targetRenderers[i].sharedMaterial;
                Debug.Log($"Stored original material for {targetRenderers[i].name}: {originalMaterials[i].name}");
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
            InitializeHandler();
        }

        if (shipReference == null || shipReference.ShipOwner == null)
        {
            return false;
        }

        return shipReference.ShipOwner is Player;
    }

    private void ApplySelectedMaterial()
    {
        if (selectedMaterial == null || targetRenderers == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Missing materials or renderers on {gameObject.name}");
            return;
        }

        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                Debug.Log($"Applying highlight material to {renderer.name}");
                renderer.material = selectedMaterial;
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
                Debug.Log($"Restoring original material for {targetRenderers[i].name}: {originalMaterials[i].name}");
                targetRenderers[i].material = originalMaterials[i];
            }
        }
    }

    private void OnValidate()
    {
        highlightIntensity = Mathf.Clamp(highlightIntensity, 0f, 2f);
    }

    private void OnDisable()
    {
        // Ensure materials are restored when the component is disabled
        RestoreOriginalMaterials();
    }
}