using UnityEngine;

public class InputManager : MonoBehaviour
{
    private ShipSelector currentlySelectedShip;
    private ShipManager shipManager;
    private Camera mainCamera;

    void Start()
    {
        shipManager = FindObjectOfType<ShipManager>();
        mainCamera = Camera.main;

        if (mainCamera == null)
            Debug.LogError("Main camera not found!");

        if (shipManager == null)
            Debug.LogError("ShipManager not found!");
    }

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        // Left click for selection
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ShipSelector newSelection = hit.collider.GetComponent<ShipSelector>();
                if (newSelection != null)
                {
                    // Only allow selection of player faction ships
                    Ship ship = newSelection.GetComponent<Ship>();
                    if (ship != null && ship.faction == shipManager.PlayerFaction)
                    {
                        // Deselect previous ship
                        if (currentlySelectedShip != null)
                            currentlySelectedShip.Deselect();

                        // Select new ship
                        currentlySelectedShip = newSelection;
                        currentlySelectedShip.Select();
                    }
                }
                else
                {
                    // Clicked something else, deselect current ship
                    if (currentlySelectedShip != null)
                    {
                        currentlySelectedShip.Deselect();
                        currentlySelectedShip = null;
                    }
                }
            }
        }

        // Right click for movement/action
        if (Input.GetMouseButtonDown(1) && currentlySelectedShip != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if clicking on another ship (for potential combat/interaction)
                Ship targetShip = hit.collider.GetComponent<Ship>();
                if (targetShip != null)
                {
                    // Handle ship-to-ship interaction here
                    // For now, just move near the target ship
                    Vector3 directionToTarget = (targetShip.transform.position - currentlySelectedShip.transform.position).normalized;
                    Vector3 stopPosition = targetShip.transform.position - directionToTarget * 5f; // Stop 5 units away
                    MoveSelectedShip(stopPosition);
                }
                else
                {
                    // Move to clicked position
                    MoveSelectedShip(hit.point);
                }
            }
        }
    }

    void MoveSelectedShip(Vector3 targetPosition)
    {
        if (currentlySelectedShip != null)
        {
            ShipMovement movement = currentlySelectedShip.GetComponent<ShipMovement>();
            if (movement != null)
            {
                movement.SetTargetPosition(targetPosition);
            }
        }
    }
}
