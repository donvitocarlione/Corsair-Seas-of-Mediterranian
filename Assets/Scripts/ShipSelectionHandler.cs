using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShipSelectionHandler : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer[] targetRenderers;
    [SerializeField]
    private GameObject selectionIndicator;
    [SerializeField]
    private LayerMask selectableLayerMask = Physics.DefaultRaycastLayers;
    
    private Material[] originalMaterials;
    private Material selectedMaterial;
    private Ship shipReference;
    private ShipMovement movementComponent;
    
    private void OnEnable()
    {
        Debug.Log($"[ShipSelectionHandler] {gameObject.name} enabled:");
        Debug.Log($"- Layer: {gameObject.layer} (Ship layer is {LayerMask.NameToLayer("Ship")})");
        Debug.Log($"- Has Collider: {GetComponent<Collider>() != null}");
        Debug.Log($"- Has Ship: {GetComponent<Ship>() != null}");
        Debug.Log($"- Is on Ship layer: {gameObject.layer == LayerMask.NameToLayer("Ship")}");
        Debug.Log($"- Selection Indicator assigned: {selectionIndicator != null}");
        Debug.Log($"- Target Renderers count: {(targetRenderers != null ? targetRenderers.Length : 0)}");
    }
    
    private void Awake()
    {
        Debug.Log($"[ShipSelectionHandler] Initializing {gameObject.name}");
        
        shipReference = GetComponent<Ship>();
        movementComponent = GetComponent<ShipMovement>();
        
        if (shipReference == null)
        {
            Debug.LogError($"[ShipSelectionHandler] No Ship component found on {gameObject.name}");
            return;
        }

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Debug.Log("[ShipSelectionHandler] No target renderers assigned, auto-finding renderers");
            targetRenderers = GetComponentsInChildren<MeshRenderer>();
            Debug.Log($"[ShipSelectionHandler] Found {targetRenderers.Length} renderers");
        }

        originalMaterials = new Material[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
            {
                originalMaterials[i] = targetRenderers[i].material;
            }
        }

        // Ensure this object is on the correct layer
        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            Debug.LogWarning($"[ShipSelectionHandler] Ship {gameObject.name} is not on the Ship layer. Setting layer now.");
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        
        obj.layer = newLayer;
        Debug.Log($"[ShipSelectionHandler] Setting layer for {obj.name} to {newLayer}");
        
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void SetSelectedMaterial(Material material)
    {
        selectedMaterial = material;
        Debug.Log($"[ShipSelectionHandler] Set selected material: {(material != null ? material.name : "null")}");
    }

    public bool Select()
    {
        Debug.Log($"[ShipSelectionHandler] Attempting to select {gameObject.name}");
        
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
            Debug.Log($"[ShipSelectionHandler] Applied selection material to {targetRenderers.Length} renderers");
        }
        else
        {
            Debug.LogWarning("[ShipSelectionHandler] No selected material assigned");
        }
        
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(true);
            Debug.Log("[ShipSelectionHandler] Activated selection indicator");
        }
        else
        {
            Debug.LogWarning("[ShipSelectionHandler] No selection indicator assigned");
        }

        Debug.Log($"[ShipSelectionHandler] Successfully selected {gameObject.name}");
        return true;
    }

    public void Deselect()
    {
        Debug.Log($"[ShipSelectionHandler] Deselecting {gameObject.name}");
        
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
            Debug.Log("[ShipSelectionHandler] Deactivated selection indicator");
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"[ShipSelectionHandler] OnMouseDown triggered on {gameObject.name}");
        Debug.Log($"- Mouse Position: {Input.mousePosition}");
        Debug.Log($"- Has Main Camera: {Camera.main != null}");
        
        // Verify the raycast hit against the correct layer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayerMask))
        {
            Debug.Log($"[ShipSelectionHandler] Raycast hit: {hit.collider.gameObject.name}");
            
            if (hit.collider.gameObject != gameObject)
            {
                Debug.Log("[ShipSelectionHandler] Raycast hit different object");
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
        else
        {
            Debug.Log("[ShipSelectionHandler] Raycast did not hit anything");
        }
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
}