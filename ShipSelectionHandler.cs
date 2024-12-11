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
    private Color highlightColor = new Color(1f, 1f, 0f, 0.3f); // Default to semi-transparent yellow
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

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            FindTargetRenderers();
            StoreOriginalMaterials();
        }

        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
        }

        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }

        // Initialize highlight material if needed
        if (selectedMaterial != null)
        {
            selectedMaterial.SetColor("_EmissionColor", highlightColor * highlightIntensity);
            selectedMaterial.SetFloat("_Metallic", 0.8f);
            selectedMaterial.SetFloat("_Smoothness", 0.7f);
        }

        isInitialized = true;
    }

    private void FindTargetRenderers()
    {
        // Get ALL mesh renderers in the ship
        targetRenderers = GetComponentsInChildren<MeshRenderer>(true);
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
                // Create instance of original material to avoid modifying the shared material
                originalMaterials[i] = new Material(targetRenderers[i].sharedMaterial);
                targetRenderers[i].material = originalMaterials[i];
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
        if (selectedMaterial == null || targetRenderers == null) return;

        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                Material highlightMat = new Material(selectedMaterial);
                highlightMat.SetColor("_EmissionColor", highlightColor * highlightIntensity);
                renderer.material = highlightMat;
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

    private void OnValidate()
    {
        // Clamp values to reasonable ranges
        highlightIntensity = Mathf.Clamp(highlightIntensity, 0f, 2f);
    }
}