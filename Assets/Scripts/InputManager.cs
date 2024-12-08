using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Ship selectedShip;
    private Camera mainCamera;

    [SerializeField] private LayerMask selectableLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private LayerMask groundLayerMask = Physics.DefaultRaycastLayers;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }

    public void OnShipSelected(Ship ship)
    {
        // Deselect previous ship if any
        if (selectedShip != null)
        {
            selectedShip.Deselect();
        }
        
        selectedShip = ship;
        if (selectedShip != null)
        {
            selectedShip.Select();
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // Only handle right-click when we have a selected ship
        if (selectedShip == null) return;

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Use groundLayerMask for movement target
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
            {
                var movement = selectedShip.GetComponent<ShipMovement>();
                if (movement != null)
                {
                    movement.SetTargetPosition(hit.point);
                    Debug.Log($"Setting target position for {selectedShip.name} to {hit.point}");
                }
            }
        }
    }

    public Ship GetSelectedShip()
    {
        return selectedShip;
    }

    private void OnValidate()
    {
        // Ensure the layer masks are set to something by default
        if (selectableLayerMask.value == 0)
        {
            selectableLayerMask = Physics.DefaultRaycastLayers;
        }
        if (groundLayerMask.value == 0)
        {
            groundLayerMask = Physics.DefaultRaycastLayers;
        }
    }
}