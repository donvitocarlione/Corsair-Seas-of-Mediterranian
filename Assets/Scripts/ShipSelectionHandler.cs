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
    
    private void Awake()
    {
        shipReference = GetComponent<Ship>();
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
        Debug.Log($"[ShipSelectionHandler] OnMouseDown triggered on {gameObject.name}");
        
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
}