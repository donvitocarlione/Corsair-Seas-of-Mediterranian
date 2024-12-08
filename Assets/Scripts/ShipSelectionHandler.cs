using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShipSelectionHandler : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer[] targetRenderers;
    [SerializeField]
    private GameObject selectionIndicator;
    
    private Material[] originalMaterials;
    private Material selectedMaterial;
    private Ship shipReference;
    private ShipMovement movementComponent;
    
    private void Awake()
    {
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
        }

        originalMaterials = new Material[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
            {
                originalMaterials[i] = targetRenderers[i].material;
            }
        }
    }

    public void SetSelectedMaterial(Material material)
    {
        selectedMaterial = material;
    }

    public bool Select()
    {
        if (shipReference == null || shipReference.ShipOwner == null || !(shipReference.ShipOwner is Player))
        {
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
    }

    private void OnMouseDown()
    {
        if (shipReference != null && shipReference.ShipOwner != null && shipReference.ShipOwner is Player player)
        {
            player.SelectShip(shipReference);
        }
    }

    private void OnMouseOver()
    {
        // Handle right-click for movement when selected
        if (Input.GetMouseButtonDown(1) && shipReference != null && shipReference.IsSelected && movementComponent != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                // Move to clicked position
                movementComponent.SetTargetPosition(hit.point);
            }
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