using UnityEngine;

public class Player : Pirate
{
    private Ship selectedShip;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ShipSelectionUI shipSelectionUI;
    
    public event System.Action<Ship> OnShipSelected;
    public event System.Action<Ship> OnShipDeselected;

    protected override void Start()
    {
        // Don't call base.Start() to prevent early faction registration
        if (inputManager == null)
        {
            inputManager = GameObject.FindFirstObjectByType<InputManager>();
            Debug.LogWarning("InputManager not assigned in inspector, trying to find in scene.");
        }
        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene!");
        }
    }

    public override void SelectShip(Ship ship)
    {
        if (!ownedShips.Contains(ship))
        {
            Debug.LogWarning("Attempting to select a ship not owned by the player");
            return;
        }

        Ship previousShip = selectedShip;
        
        if (selectedShip != null)
        {
            selectedShip.Deselect();
            OnShipDeselected?.Invoke(selectedShip);
        }

        selectedShip = ship;
        ship.Select();
        OnShipSelected?.Invoke(ship);

        // Update UI
        if (shipSelectionUI != null)
        {
            shipSelectionUI.UpdateSelection(ship);
        }

        // Notify input manager
        if (inputManager != null)
        {
            inputManager.OnShipSelected(ship);
        }
    }

    public Ship GetSelectedShip()
    {
        return selectedShip;
    }

    public override void AddShip(Ship ship)
    {
        base.AddShip(ship);
        // Additional player-specific logic can be added here
        if (shipSelectionUI != null)
        {
            shipSelectionUI.UpdateShipList(GetOwnedShips());
        }
    }

    public override void RemoveShip(Ship ship)
    {
        if (ship == selectedShip)
        {
            selectedShip = null;
            OnShipDeselected?.Invoke(ship);
            if (inputManager != null)
            {
                inputManager.OnShipSelected(null);
            }
        }
        base.RemoveShip(ship);
        
        if (shipSelectionUI != null)
        {
            shipSelectionUI.UpdateShipList(GetOwnedShips());
        }
    }

    public void SelectNextShip()
    {
        if (ownedShips.Count == 0) return;
        
        int currentIndex = selectedShip != null ? ownedShips.IndexOf(selectedShip) : -1;
        int nextIndex = (currentIndex + 1) % ownedShips.Count;
        
        SelectShip(ownedShips[nextIndex]);
    }
    
    public void MoveShipsInFormation(Vector3 targetPosition)
    {
        if (ownedShips.Count == 0) return;
        
        float spacing = 5f; // Distance between ships
        Vector3 centerPosition = targetPosition;
        
        for (int i = 0; i < ownedShips.Count; i++)
        {
            Vector3 offset = new Vector3(
                (i % 3) * spacing - spacing,  // Horizontal offset
                0,
                (i / 3) * spacing            // Vertical offset
            );
            
            ShipMovement movement = ownedShips[i].GetComponent<ShipMovement>();
            if (movement != null)
            {
                movement.SetTargetPosition(centerPosition + offset);
            }
        }
    }
}
