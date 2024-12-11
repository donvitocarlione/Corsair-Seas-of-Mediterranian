using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Game/Player")]
public class Player : Pirate
{
    private static Player instance;
    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<Player>();
                if (instance == null)
                {
                    Debug.LogError("No Player instance found in scene!");
                }
            }
            return instance;
        }
    }

    private Ship selectedShip;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private ShipSelectionUI shipSelectionUI;
    
    public event System.Action<Ship> OnShipSelected;
    public event System.Action<Ship> OnShipDeselected;

    public Ship SelectedShip => selectedShip;

    protected override void Awake()
    {
        Debug.Log("[Player] Awake called");
        if (instance != null && instance != this)
        {
            Debug.LogError("Multiple Player instances found! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        base.Awake();
    }

    protected override void Start()
    {
        Debug.Log("[Player] Start called");
        base.Start();
        InitializeServices();

        // Set faction for existing ships
        foreach (var ship in GetOwnedShips())
        {
            if (ship != null)
            {
                ship.SetOwner(this);
            }
        }
    }

    private void InitializeServices()
    {
        Debug.Log("[Player] Initializing services");
        InitializeInputManager();
        InitializeUI();
    }

    private void InitializeInputManager()
    {
        if (inputManager != null) return;

        inputManager = FindAnyObjectByType<InputManager>();
        if (inputManager == null)
        {
            Debug.LogError("[Player] InputManager not found! Player controls will be disabled.");
        }
        else
        {
            Debug.Log("[Player] InputManager found and initialized");
        }
    }

    private void InitializeUI()
    {
        if (shipSelectionUI == null)
        {
            Debug.LogWarning("[Player] ShipSelectionUI reference not set.");
        }
        else
        {
            Debug.Log("[Player] ShipSelectionUI initialized");
        }
    }

    public override void SelectShip(Ship ship)
    {
        if (ship == null)
        {
            Debug.LogError("[Player] Attempting to select a null ship!");
            return;
        }

        if (!ownedShips.Contains(ship))
        {
            Debug.LogWarning($"[Player] Cannot select ship '{ship.ShipName}' - not owned by player");
            return;
        }

        // Deselect previous ship
        if (selectedShip != null)
        {
            selectedShip.Deselect();
            OnShipDeselected?.Invoke(selectedShip);
        }

        // Select new ship
        selectedShip = ship;
        ship.Select();
        OnShipSelected?.Invoke(ship);

        // Update UI and input manager
        shipSelectionUI?.UpdateSelection(ship);
        inputManager?.OnShipSelected(ship);

        Debug.Log($"[Player] Selected ship: {ship.ShipName}");
    }

    public override void AddShip(Ship ship)
    {
        if (ship == null)
        {
            Debug.LogError("[Player] Attempting to add a null ship!");
            return;
        }

        // Set ownership before adding to list
        ship.SetOwner(this);
        base.AddShip(ship);
        
        Debug.Log($"[Player] Added ship to fleet: {ship.ShipName}");
        shipSelectionUI?.UpdateShipList(GetOwnedShips());

        // Auto-select if this is our only ship
        if (selectedShip == null && ownedShips.Count == 1)
        {
            SelectShip(ship);
        }
    }

    public override void RemoveShip(Ship ship)
    {
        if (ship == null)
        {
            Debug.LogError("[Player] Attempting to remove a null ship!");
            return;
        }

        if (ship == selectedShip)
        {
            selectedShip = null;
            OnShipDeselected?.Invoke(ship);
            inputManager?.OnShipSelected(null);
        }

        base.RemoveShip(ship);
        Debug.Log($"[Player] Removed ship from fleet: {ship.ShipName}");
        
        shipSelectionUI?.UpdateShipList(GetOwnedShips());

        // Auto-select another ship if available
        if (selectedShip == null && ownedShips.Count > 0)
        {
            SelectShip(ownedShips[0]);
        }
    }

    protected override void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        OnShipSelected = null;
        OnShipDeselected = null;

        base.OnDestroy();
    }
}