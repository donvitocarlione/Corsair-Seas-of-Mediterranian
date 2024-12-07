using UnityEngine;

public class InputManager : MonoBehaviour
{
    private ShipSelector currentlySelectedShip;
    private ShipManager shipManager;
    private Camera mainCamera;

    [Header("Selection Settings")]
    public LayerMask selectionMask = -1; // Default to everything
    public float maxSelectionDistance = 1000f;

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
            HandleSelection();
        }

        // Right click for movement/action
        if (Input.GetMouseButtonDown(1) && currentlySelectedShip != null)
        {
            HandleAction();
        }
    }

    void HandleSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * maxSelectionDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out hit, maxSelectionDistance, selectionMask))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}");

            ShipSelector newSelection = hit.collider.GetComponent<ShipSelector>();
            if (newSelection != null)
            {
                if (newSelection.IsSelectable())
                {
                    // Deselect previous ship
                    if (currentlySelectedShip != null)
                    {
                        currentlySelectedShip.Deselect();
                    }

                    // Select new ship
                    currentlySelectedShip = newSelection;
                    currentlySelectedShip.Select();
                    Debug.Log($"Selected ship: {hit.collider.gameObject.name}");
                }
                else
                {
                    Debug.Log($"Ship {hit.collider.gameObject.name} is not selectable");
                }
            }
            else
            {
                // Clicked something else, deselect current ship
                if (currentlySelectedShip != null)
                {
                    currentlySelectedShip.Deselect();
                    currentlySelectedShip = null;
                    Debug.Log("Deselected current ship");
                }
            }
        }
        else
        {
            Debug.Log("No object hit");
            // Clicked nothing, deselect current ship
            if (currentlySelectedShip != null)
            {
                currentlySelectedShip.Deselect();
                currentlySelectedShip = null;
                Debug.Log("Deselected current ship");
            }
        }
    }

    void HandleAction()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxSelectionDistance, selectionMask))
        {
            // Check if clicking on another ship (for potential combat/interaction)
            Ship targetShip = hit.collider.GetComponent<Ship>();
            if (targetShip != null)
            {
                // Handle ship-to-ship interaction here
                Vector3 directionToTarget = (targetShip.transform.position - currentlySelectedShip.transform.position).normalized;
                Vector3 stopPosition = targetShip.transform.position - directionToTarget * 5f; // Stop 5 units away
                MoveSelectedShip(stopPosition);
                Debug.Log($"Moving to interact with ship: {targetShip.gameObject.name}");
            }
            else
            {
                // Move to clicked position
                MoveSelectedShip(hit.point);
                Debug.Log($"Moving to position: {hit.point}");
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
            else
            {
                Debug.LogWarning("Selected ship has no ShipMovement component");
            }
        }
    }
}
