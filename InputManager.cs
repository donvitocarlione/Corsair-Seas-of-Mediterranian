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
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        // Setup default layer masks if not set
        if (shipLayerMask == 0)
        {
            shipLayerMask = LayerMask.GetMask("Ship");
            Debug.LogWarning("Ship layer mask not set. Using 'Ship' layer.");
        }

        if (groundLayerMask == 0)
        {
            groundLayerMask = LayerMask.GetMask("Water", "Terrain"); // Updated to include Water and Terrain
            Debug.LogWarning("Ground layer mask not set. Using 'Water' and 'Terrain' layers.");
        }
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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, shipLayerMask))
        {
            Ship ship = hit.collider.GetComponent<Ship>();
            if (ship != null)
            {
                OnShipSelected(ship);
            }
        }
        else
        {
            // Clicked nothing - deselect
            OnShipSelected(null);
        }
    }

    private void HandleMovement()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            var movement = selectedShip.GetComponent<ShipMovement>();
            if (movement != null)
            {
                movement.SetTargetPosition(hit.point);
                Debug.Log($"[InputManager] Moving {selectedShip.ShipName} to position: {hit.point}");
            }
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