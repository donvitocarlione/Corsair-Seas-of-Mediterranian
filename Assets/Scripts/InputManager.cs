using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Ship selectedShip;
    private Camera mainCamera;

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

            if (Physics.Raycast(ray, out hit))
            {
                // Get ship movement component and set target position
                var movement = selectedShip.GetComponent<ShipMovement>();
                if (movement != null)
                {
                    movement.SetTargetPosition(hit.point);
                }
            }
        }
    }

    public Ship GetSelectedShip()
    {
        return selectedShip;
    }
}
