using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShipSelectionHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] targetRenderers;
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private LayerMask selectableLayerMask = Physics.DefaultRaycastLayers;
    
    private Material[] originalMaterials;
    private Material selectedMaterial;
    private Ship shipReference;
    private ShipMovement movementComponent;
    
    private void Awake()
    {
        // Ensure this object is on the correct layer
        if (gameObject.layer == 0) // Default layer
        {
            Debug.LogWarning($"[ShipSelectionHandler] {gameObject.name} is on Default layer. Consider using a dedicated Ship layer.");
        }

        shipReference = GetComponent<Ship>();
        movementComponent = GetComponent<ShipMovement>();
        
        if (shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] No Ship component found on {gameObject.name}");
            return;
        }

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = GetComponentsInChildren<MeshRenderer>();
            Debug.Log($"[ShipSelectionHandler] Found {targetRenderers.Length} renderers on {gameObject.name}");
        }

        originalMaterials = new Material[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
            {
                originalMaterials[i] = targetRenderers[i].material;
            }
        }

        // Verify collider setup
        var collider = GetComponent<Collider>();
        if (collider.isTrigger)
        {
            Debug.LogWarning($"[ShipSelectionHandler] Collider on {gameObject.name} is set as trigger. This may interfere with selection.");
        }
    }

    public void SetSelectedMaterial(Material material)
    {
        selectedMaterial = material;
    }

    public bool Select()
    {
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
        
        if (selectedMaterial != null)
        {
            foreach (var renderer in targetRenderers)
            {
                if (renderer != null)
                {
                    renderer.material = selectedMaterial;
                }
            }
        }
        
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(true);
        }

        Debug.Log($"[ShipSelectionHandler] Successfully selected {gameObject.name}");
        return true;
    }

    public void Deselect()
    {
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null && originalMaterials[i] != null)
            {
                targetRenderers[i].material = originalMaterials[i];
            }
        }

        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
        Debug.Log($"[ShipSelectionHandler] Deselected {gameObject.name}");
    }

    private void OnMouseDown()
    {
        Debug.Log($"[ShipSelectionHandler] OnMouseDown triggered on {gameObject.name}");
        
        // Verify the click is valid using the layer mask
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayerMask))
        {
            Debug.LogWarning($"[ShipSelectionHandler] Raycast failed - check layer masks on {gameObject.name}");
            return;
        }

        if (hit.collider.gameObject != gameObject)
        {
            Debug.LogWarning($"[ShipSelectionHandler] Raycast hit different object: {hit.collider.gameObject.name}");
            return;
        }

        if (shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] OnMouseDown - shipReference is null on {gameObject.name}");
            return;
        }

        if (shipReference.ShipOwner == null)
        {
            Debug.LogError($"[ShipSelectionHandler] OnMouseDown - ship has no owner on {gameObject.name}");
            return;
        }

        if (!(shipReference.ShipOwner is Player player))
        {
            Debug.LogWarning($"[ShipSelectionHandler] OnMouseDown - ship's owner is not a Player on {gameObject.name}");
            return;
        }

        Debug.Log($"[ShipSelectionHandler] Selecting ship {shipReference.ShipName}");
        player.SelectShip(shipReference);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
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

    private void OnValidate()
    {
        // Ensure the layer mask is set to something by default
        if (selectableLayerMask.value == 0)
        {
            selectableLayerMask = Physics.DefaultRaycastLayers;
        }
    }
}