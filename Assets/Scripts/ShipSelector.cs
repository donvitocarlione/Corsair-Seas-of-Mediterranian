using UnityEngine;
using System.Collections.Generic;

public class ShipSelector : MonoBehaviour
{
    public static ShipSelector Instance { get; private set; }
    public Material selectedMaterial;
    public FactionType playerFaction;
    
    private List<Ship> selectedShips = new List<Ship>();
    private Camera mainCamera;
    private LayerMask waterLayer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        mainCamera = Camera.main;
        waterLayer = LayerMask.GetMask("Water");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) HandleSelection();
        if (Input.GetMouseButtonDown(1)) HandleMovement();
    }

    void HandleSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Ship ship = hit.collider.GetComponent<Ship>();
            if (ship != null && ship.faction == playerFaction)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    ToggleSelection(ship);
                else
                {
                    DeselectAll();
                    SelectShip(ship);
                }
            }
            else DeselectAll();
        }
    }

    void HandleMovement()
    {
        if (selectedShips.Count == 0) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, waterLayer))
        {
            foreach (Ship ship in selectedShips)
                ship.GetComponent<ShipMovement>()?.SetTargetPosition(hit.point);
        }
    }

    void SelectShip(Ship ship)
    {
        if (!selectedShips.Contains(ship))
        {
            selectedShips.Add(ship);
            ship.GetComponent<ShipSelection>()?.SetSelected(true, selectedMaterial);
        }
    }

    void DeselectShip(Ship ship)
    {
        if (selectedShips.Contains(ship))
        {
            selectedShips.Remove(ship);
            ship.GetComponent<ShipSelection>()?.SetSelected(false, selectedMaterial);
        }
    }

    void ToggleSelection(Ship ship)
    {
        if (selectedShips.Contains(ship)) DeselectShip(ship);
        else SelectShip(ship);
    }

    void DeselectAll()
    {
        foreach (Ship ship in selectedShips)
            ship.GetComponent<ShipSelection>()?.SetSelected(false, selectedMaterial);
        selectedShips.Clear();
    }
}