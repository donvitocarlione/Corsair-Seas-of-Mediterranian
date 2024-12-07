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
        selectedShip = ship;
        // Add any additional input handling for selected ship
    }

    private void Update()
    {
        HandleShipMovement();
    }

    private void HandleShipMovement()
    {
        if (selectedShip == null) return;

        // Example movement controls
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            // Get ship movement component and apply movement
            var movement = selectedShip.GetComponent<ShipMovement>();
            if (movement != null)
            {
                movement.SetMovementInput(horizontal, vertical);
            }
        }
    }

    public Ship GetSelectedShip()
    {
        return selectedShip;
    }
}
