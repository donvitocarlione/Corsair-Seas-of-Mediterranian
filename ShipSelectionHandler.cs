using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ShipSelectionHandler : MonoBehaviour
{
    [Header("Selection Visual Settings")]
    [SerializeField]
    private Material selectedMaterial;
    [SerializeField]
    private GameObject selectionIndicator;

    private List<MeshRenderer> targetRenderers = new List<MeshRenderer>();
    private Dictionary<MeshRenderer, Material> originalMaterials = new Dictionary<MeshRenderer, Material>();
    private Ship shipReference;
    private bool isInitialized = false;

    private void Reset()
    {
        // Called when component is first added or reset in editor
        Debug.Log($"[ShipSelectionHandler] Reset called on {gameObject.name}");
        InitializeHandler();
    }

    private void Awake()
    {
        Debug.Log($"[ShipSelectionHandler] Awake called on {gameObject.name}");
        InitializeHandler();
    }

    private void OnEnable()
    {
        Debug.Log($"[ShipSelectionHandler] OnEnable called on {gameObject.name}");
        if (!isInitialized)
        {
            InitializeHandler();
        }
    }

    private void InitializeHandler()
    {
        Debug.Log($"[ShipSelectionHandler] Initializing handler for {gameObject.name}");

        // Get ship reference
        if (shipReference == null)
        {
            shipReference = GetComponent<Ship>();
            if (shipReference == null)
            {
                Debug.LogError($"[ShipSelectionHandler] No Ship component found on {gameObject.name}");
                return;
            }
        }

        // Find renderers if not already found
        if (targetRenderers.Count == 0)
        {
            FindAndStoreRenderers();
        }

        // Ensure on correct layer
        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
            Debug.Log($"[ShipSelectionHandler] Set layer to Ship for {gameObject.name}");
        }

        // Setup selection indicator
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }

        isInitialized = true;
        Debug.Log($"[ShipSelectionHandler] Initialization complete for {gameObject.name}");
    }

    private void FindAndStoreRenderers()
    {
        Debug.Log($"[ShipSelectionHandler] Finding renderers for {gameObject.name}");
        targetRenderers.Clear();
        originalMaterials.Clear();

        // Get all mesh renderers in children
        MeshRenderer[] foundRenderers = GetComponentsInChildren<MeshRenderer>(true);
        
        foreach (MeshRenderer renderer in foundRenderers)
        {
            if (renderer != null && renderer.sharedMaterial != null)
            {
                targetRenderers.Add(renderer);
                originalMaterials[renderer] = renderer.sharedMaterial;
                Debug.Log($"[ShipSelectionHandler] Found renderer: {renderer.name} with material: {renderer.sharedMaterial.name}");
            }
        }

        Debug.Log($"[ShipSelectionHandler] Found {targetRenderers.Count} valid renderers in {gameObject.name}");
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
        if (!isInitialized || shipReference == null)
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
        if (selectedMaterial == null)
        {
            Debug.LogError($"[ShipSelectionHandler] Selected material is not assigned on {gameObject.name}");
            return;
        }

        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                renderer.material = selectedMaterial;
                Debug.Log($"[ShipSelectionHandler] Applied highlight material to {renderer.name}");
            }
        }
    }

    private void RestoreOriginalMaterials()
    {
        foreach (var renderer in targetRenderers)
        {
            if (renderer != null && originalMaterials.ContainsKey(renderer))
            {
                renderer.material = originalMaterials[renderer];
                Debug.Log($"[ShipSelectionHandler] Restored original material for {renderer.name}");
            }
        }
    }

    private void OnDisable()
    {
        if (isInitialized)
        {
            RestoreOriginalMaterials();
        }
    }

    private void OnDestroy()
    {
        targetRenderers.Clear();
        originalMaterials.Clear();
    }
}