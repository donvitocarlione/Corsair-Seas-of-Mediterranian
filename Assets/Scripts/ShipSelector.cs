using UnityEngine;
using System.Collections.Generic;

public class ShipSelector : MonoBehaviour
{
    public static ShipSelector Instance { get; private set; }
    public Material selectedMaterial;
    public LayerMask selectableLayer; // Layer mask for objects that can be selected
    
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
            selectableLayer = LayerMask.GetMask("Ship", "Default"); // Add layers that contain selectable objects
            
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
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f); // Visualize the ray
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayer))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name} on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            
            // First try to get Ship component from the hit object
            Ship ship = hit.collider.GetComponent<Ship>();
            
            // If not found, try to get it from the parent
            if (ship == null)
            {
                ship = hit.collider.GetComponentInParent<Ship>();
            }
            
            if (ship != null)
            {
                Debug.Log($"Found ship with faction: {ship.faction}, Player faction: {playerFaction}");
                
                if (ship.faction == playerFaction)
                {
                    Debug.Log("Faction match - processing selection");
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
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
                Debug.Log("No Ship component found on hit object or its parents");
                DeselectAll();
            }
        }
        else
        {
            Debug.Log("Ray didn't hit anything on selectable layers");
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
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.blue, 1f); // Visualize the movement ray
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, waterLayer))
        {
            Debug.Log($"Moving selected ships to position: {hit.point}");
            foreach (Ship ship in selectedShips)
            {
                if (ship != null) // Add null check
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
    }

    void SelectShip(Ship ship)
    {
        if (ship != null && !selectedShips.Contains(ship))
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
        if (ship != null && selectedShips.Contains(ship))
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
        foreach (Ship ship in new List<Ship>(selectedShips)) // Create a copy to avoid modification during iteration
        {
            if (ship != null)
            {
                var selection = ship.GetComponent<ShipSelection>();
                if (selection != null)
                {
                    selection.SetSelected(false, selectedMaterial);
                }
            }
        }
        selectedShips.Clear();
        Debug.Log("Deselected all ships");
    }
}
