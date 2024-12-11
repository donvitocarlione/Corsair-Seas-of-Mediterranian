using UnityEngine;
using UnityEngine.EventSystems; // Added for EventSystem

public class InputManager : MonoBehaviour
{
    private Ship selectedShip;
    private Camera mainCamera;
    
    [SerializeField]
    private LayerMask shipLayerMask;
    [SerializeField]
    private LayerMask groundLayerMask; // For right-click movement target detection

    private void Start()
    {
        Debug.Log("[InputManager] Starting initialization");
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[InputManager] Main camera not found! Ensure camera has MainCamera tag.");
            return;
        }
        Debug.Log("[InputManager] Main camera found successfully");

        // Setup default layer masks if not set
        if (shipLayerMask == 0)
        {
            shipLayerMask = LayerMask.GetMask("Ship");
            Debug.Log("[InputManager] Setting default Ship layer mask: " + shipLayerMask.value);
        }

        if (groundLayerMask == 0)
        {
            groundLayerMask = LayerMask.GetMask("Water", "Terrain");
            Debug.Log("[InputManager] Setting default Ground layer mask: " + groundLayerMask.value);
        }
        
        Debug.Log("[InputManager] Layer masks - Ship: " + shipLayerMask.value + ", Ground: " + groundLayerMask.value);
    }

    public void OnShipSelected(Ship ship)
    {
        if (ship == null)
        {
            if (selectedShip != null)
            {
                Debug.Log($"[InputManager] Deselecting ship: {selectedShip.ShipName}");
                selectedShip.Deselect();
                selectedShip = null;
            }
            return;
        }

        // Validate ship can be selected
        if (!(ship.ShipOwner is Player))
        {
            Debug.LogWarning($"[InputManager] Cannot select ship {ship.ShipName} - not owned by player");
            return;
        }

        // Only deselect if selecting a different ship
        if (selectedShip != null && selectedShip != ship)
        {
            Debug.Log($"[InputManager] Deselecting previous ship: {selectedShip.ShipName}");
            selectedShip.Deselect();
        }

        // Don't reselect if it's already selected
        if (selectedShip != ship)
        {
            selectedShip = ship;
            ship.Select();
            Debug.Log($"[InputManager] Selected new ship: {ship.ShipName}");
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // Left click for selection
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
        // Right click for movement
        else if (Input.GetMouseButtonDown(1) && selectedShip != null)
        {
            HandleMovement();
        }
    }

    private void HandleSelection()
    {
        // Skip if clicking on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[InputManager] Clicked on UI - skipping selection");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);
        Debug.Log($"[InputManager] Casting selection ray with mask {shipLayerMask.value}");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, shipLayerMask))
        {
            Debug.Log($"[InputManager] Hit object: {hit.collider.gameObject.name} on layer {hit.collider.gameObject.layer}");
            Ship ship = hit.collider.GetComponent<Ship>();
            if (ship != null)
            {
                OnShipSelected(ship);
            }
            else
            {
                Debug.LogWarning($"[InputManager] Hit object {hit.collider.gameObject.name} has no Ship component");
            }
        }
        else
        {
            Debug.Log("[InputManager] Selection raycast hit nothing");
            OnShipSelected(null);
        }
    }

    private void HandleMovement()
    {
        // Skip if clicking on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[InputManager] Clicked on UI - skipping movement");
            return;
        }

        if (selectedShip == null)
        {
            Debug.LogWarning("[InputManager] No ship selected for movement");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.blue, 1f);
        Debug.Log($"[InputManager] Casting movement ray with mask {groundLayerMask.value}");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Debug.Log($"[InputManager] Movement raycast hit {hit.collider.gameObject.name} at {hit.point}");
            var movement = selectedShip.GetComponent<ShipMovement>();
            if (movement != null)
            {
                movement.SetTargetPosition(hit.point);
                Debug.Log($"[InputManager] Set movement target for {selectedShip.ShipName} to {hit.point}");
            }
            else
            {
                Debug.LogError($"[InputManager] Ship {selectedShip.ShipName} has no ShipMovement component!");
            }
        }
        else
        {
            Debug.LogWarning("[InputManager] Movement raycast hit nothing");
        }
    }

    public Ship GetSelectedShip()
    {
        return selectedShip;
    }

    private void OnValidate()
    {
        if (shipLayerMask == 0)
        {
            shipLayerMask = LayerMask.GetMask("Ship");
        }
        if (groundLayerMask == 0)
        {
            groundLayerMask = LayerMask.GetMask("Water", "Terrain");
        }
    }
}
