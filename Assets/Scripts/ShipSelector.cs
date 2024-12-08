using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    private Camera mainCamera;
    private LayerMask shipLayer;

    private void Awake()
    {
        mainCamera = Camera.main;
        shipLayer = LayerMask.GetMask("Ships");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }

    private void HandleSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, shipLayer))
        {
            var ship = hit.collider.GetComponent<Ship>();
            if (ship != null)
            {
                SelectShip(ship);
            }
        }
        else
        {
            DeselectCurrentShip();
        }
    }

    private void SelectShip(Ship ship)
    {
        if (ship.Owner is Player player)
        {
            player.SelectShip(ship);
        }
    }

    private void DeselectCurrentShip()
    {
        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.DeselectCurrentShip();
        }
    }
}
