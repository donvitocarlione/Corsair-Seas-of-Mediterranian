using UnityEngine;

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
            Debug.LogWarning("[InputManager] Ship layer mask not set. Using 'Ship' layer. Value: " + shipLayerMask.value);
        }

        if (groundLayerMask == 0)
        {
            groundLayerMask = LayerMask.GetMask("Water", "Terrain"); // Updated to include Water and Terrain
            Debug.LogWarning("[InputManager] Ground layer mask not set. Using 'Water' and 'Terrain' layers. Value: " + groundLayerMask.value);
        }
        
        Debug.Log("[InputManager] Initialization complete");
    }

    public void OnShipSelected(Ship ship)
    {
        if (ship == null)
        {
            Debug.Log("[InputManager] Deselecting current ship");
            if (selectedShip != null)
            {
                selectedShip.Deselect();
                selectedShip = null;
            }
            return;
        }

        // Validate ship can be selected
        if (!(ship.ShipOwner is Player))
        {
            Debug.LogWarning("[InputManager] Cannot select ship - not owned by player");
            return;
        }

        // Deselect previous ship if any
        if (selectedShip != null && selectedShip != ship)
        {
            selectedShip.Deselect();
        }
        
        selectedShip = ship;
        ship.Select();
        Debug.Log($"[InputManager] Selected ship: {ship.ShipName}");
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
            Debug.Log("[InputManager] Left click detected");
            HandleSelection();
        }
        // Right click for movement
        else if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("[InputManager] Right click detected");
            if (selectedShip != null)
            {
                HandleMovement();
            }
            else
            {
                Debug.LogWarning("[InputManager] No ship selected for movement");
            }
        }
    }

    private void HandleSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log($"[InputManager] Attempting selection raycast with layer mask: {shipLayerMask.value}");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, shipLayerMask))
        {
            Debug.Log($"[InputManager] Hit object: {hit.collider.gameObject.name}");
            Ship ship = hit.collider.GetComponent<Ship>();
            if (ship != null)
            {
                OnShipSelected(ship);
            }
            else
            {
                Debug.LogWarning("[InputManager] Hit object does not have Ship component");
            }
        }
        else
        {
            Debug.Log("[InputManager] No ship hit - deselecting");
            OnShipSelected(null);
        }
    }

    private void HandleMovement()
    {
        Debug.Log("[InputManager] Attempting to move selected ship");
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log($"[InputManager] Attempting movement raycast with layer mask: {groundLayerMask.value}");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Debug.Log($"[InputManager] Found movement target at {hit.point}");
            var movement = selectedShip.GetComponent<ShipMovement>();
            if (movement != null)
            {
                Debug.Log($"[InputManager] Moving {selectedShip.ShipName} to {hit.point}");
                movement.SetTargetPosition(hit.point);
            }
            else
            {
                Debug.LogError($"[InputManager] Selected ship {selectedShip.ShipName} has no ShipMovement component!");
            }
        }
        else
        {
            Debug.LogWarning($"[InputManager] No valid movement target found. Check if Water/Terrain layers are set up correctly.");
        }
    }

    public Ship GetSelectedShip()
    {
        return selectedShip;
    }

    private void OnValidate()
    {
        // Help ensure proper layer masks are set in the inspector
        if (shipLayerMask == 0)
        {
            Debug.LogWarning("Ship layer mask not set in InputManager. Please set it in the inspector.");
        }
        if (groundLayerMask == 0)
        {
            Debug.LogWarning("Ground layer mask not set in InputManager. Please set it in the inspector.");
        }
    }
}