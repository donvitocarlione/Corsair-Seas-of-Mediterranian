using UnityEngine;
using System.Collections.Generic;

public class ShipSelector : MonoBehaviour
{
    public static ShipSelector Instance { get; private set; }
    public Material selectedMaterial;
    
    // Changed to private - will get from PlayerPirate
    private FactionType playerFaction;
    private List<Ship> selectedShips = new List<Ship>();
    private Camera mainCamera;
    private LayerMask waterLayer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mainCamera = Camera.main;
            waterLayer = LayerMask.GetMask("Water");
            
            // Get player faction from PlayerPirate
            var playerPirate = FindObjectOfType<PlayerPirate>();
            if (playerPirate != null)
            {
                playerFaction = playerPirate.faction;
                Debug.Log($"ShipSelector initialized with faction: {playerFaction}");
            }
            else
            {
                Debug.LogError("PlayerPirate not found in scene!");
            }
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (selectedMaterial == null)
        {
            Debug.LogError("Selected material is not assigned in ShipSelector!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) HandleSelection();
        if (Input.GetMouseButtonDown(1)) HandleMovement();
    }

    void HandleSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.Log("Casting ray for selection...");
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}");
            
            Ship ship = hit.collider.GetComponent<Ship>();
            if (ship != null)
            {
                Debug.Log($"Found ship with faction: {ship.faction}, Player faction: {playerFaction}");
                
                if (ship.faction == playerFaction)
                {
                    Debug.Log("Faction match - processing selection");
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        ToggleSelection(ship);
                    }
                    else
                    {
                        DeselectAll();
                        SelectShip(ship);
                    }
                }
                else
                {
                    Debug.Log($"Faction mismatch - Cannot select ship of faction {ship.faction}");
                    DeselectAll();
                }
            }
            else
            {
                Debug.Log("No Ship component found on hit object");
                DeselectAll();
            }
        }
        else
        {
            Debug.Log("Ray didn't hit anything");
        }
    }

    void HandleMovement()
    {
        if (selectedShips.Count == 0)
        {
            Debug.Log("No ships selected for movement");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, waterLayer))
        {
            Debug.Log($"Moving selected ships to position: {hit.point}");
            foreach (Ship ship in selectedShips)
            {
                var movement = ship.GetComponent<ShipMovement>();
                if (movement != null)
                {
                    movement.SetTargetPosition(hit.point);
                }
                else
                {
                    Debug.LogError($"ShipMovement component missing on ship: {ship.name}");
                }
            }
        }
    }

    void SelectShip(Ship ship)
    {
        if (!selectedShips.Contains(ship))
        {
            selectedShips.Add(ship);
            var selection = ship.GetComponent<ShipSelection>();
            if (selection != null)
            {
                selection.SetSelected(true, selectedMaterial);
                Debug.Log($"Selected ship: {ship.name}");
            }
            else
            {
                Debug.LogError($"ShipSelection component missing on ship: {ship.name}");
            }
        }
    }

    void DeselectShip(Ship ship)
    {
        if (selectedShips.Contains(ship))
        {
            selectedShips.Remove(ship);
            var selection = ship.GetComponent<ShipSelection>();
            if (selection != null)
            {
                selection.SetSelected(false, selectedMaterial);
                Debug.Log($"Deselected ship: {ship.name}");
            }
        }
    }

    void ToggleSelection(Ship ship)
    {
        if (selectedShips.Contains(ship))
        {
            DeselectShip(ship);
        }
        else
        {
            SelectShip(ship);
        }
    }

    void DeselectAll()
    {
        foreach (Ship ship in selectedShips)
        {
            var selection = ship.GetComponent<ShipSelection>();
            if (selection != null)
            {
                selection.SetSelected(false, selectedMaterial);
            }
        }
        selectedShips.Clear();
        Debug.Log("Deselected all ships");
    }
}