using UnityEngine;

public class Player : Pirate
{
    private Ship selectedShip;
    [SerializeField] private InputManager inputManager;

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

        if (selectedShip != null)
        {
            selectedShip.Deselect();
        }

        selectedShip = ship;
        ship.Select();

        // Notify input manager of selection change
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
    }

    public override void RemoveShip(Ship ship)
    {
        if (ship == selectedShip)
        {
            selectedShip = null;
            if (inputManager != null)
            {
                inputManager.OnShipSelected(null);
            }
        }
        base.RemoveShip(ship);
    }
}
