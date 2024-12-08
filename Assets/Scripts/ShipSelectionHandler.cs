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
        try
        {
            Debug.Log($"[ShipSelectionHandler] OnEnable start for {gameObject.name}");
            Debug.Log($"[ShipSelectionHandler] Camera.main exists: {Camera.main != null}");
            if (Camera.main != null)
            {
                Debug.Log($"[ShipSelectionHandler] Camera culling mask: {Camera.main.cullingMask}");
                Debug.Log($"[ShipSelectionHandler] Ship layer in camera: {(Camera.main.cullingMask & (1 << gameObject.layer)) != 0}");
            }
            
            // Check if we can raycast to this object
            Ray testRay = new Ray(transform.position + Vector3.up * 10f, Vector3.down);
            if (Physics.Raycast(testRay, out RaycastHit hit, 20f, selectableLayerMask))
            {
                Debug.Log($"[ShipSelectionHandler] Test raycast hit: {hit.collider.gameObject.name}");
                Debug.DrawRay(testRay.origin, testRay.direction * 20f, Color.green, 5f);
            }
            else
            {
                Debug.Log("[ShipSelectionHandler] Test raycast hit nothing");
                Debug.DrawRay(testRay.origin, testRay.direction * 20f, Color.red, 5f);
            }

            Debug.Log($"[ShipSelectionHandler] {gameObject.name} layer: {LayerMask.LayerToName(gameObject.layer)}");
            Debug.Log($"[ShipSelectionHandler] SelectableLayerMask: {selectableLayerMask.value}");
            Debug.Log($"[ShipSelectionHandler] OnEnable completed for {gameObject.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ShipSelectionHandler] Error in OnEnable for {gameObject.name}: {e}");
        }
    }
    
    private void Awake()
    {
        try
        {
            Debug.Log($"[ShipSelectionHandler] Awake start for {gameObject.name}");
            
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
            
            Debug.Log($"[ShipSelectionHandler] Awake completed for {gameObject.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ShipSelectionHandler] Error in Awake for {gameObject.name}: {e}");
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
        Debug.Log($"[ShipSelectionHandler] Select() called on {gameObject.name}");
        
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
                    Debug.Log($"[ShipSelectionHandler] Applied selection material to {renderer.gameObject.name}");
                }
            }
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

        return true;
    }

    public void Deselect()
    {
        Debug.Log($"[ShipSelectionHandler] Deselect() called on {gameObject.name}");
        
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
        try
        {
            Debug.Log($"[ShipSelectionHandler] OnMouseDown start on {gameObject.name}");
            Debug.Log($"- Mouse Position: {Input.mousePosition}");
            Debug.Log($"- Has Main Camera: {Camera.main != null}");
            
            // Verify the raycast hit against the correct layer
            if (Camera.main == null)
            {
                Debug.LogError("[ShipSelectionHandler] No main camera found!");
                return;
            }
            
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
        catch (System.Exception e)
        {
            Debug.LogError($"[ShipSelectionHandler] Error in OnMouseDown for {gameObject.name}: {e}");
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