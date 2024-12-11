using UnityEngine;

public class ShipSelectionDebug : MonoBehaviour
{
    public LayerMask shipLayer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Debug.Log("[ShipSelectionDebug] Started - Click on ships to test selection");
    }

    void Update()
    {
        // On left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, shipLayer))
            {
                Debug.Log($"[ShipSelectionDebug] Hit object: {hit.collider.gameObject.name}");
                Ship ship = hit.collider.GetComponent<Ship>();
                if (ship != null)
                {
                    Debug.Log($"[ShipSelectionDebug] Ship found: {ship.ShipName}, Faction: {ship.Faction}, Owner: {(ship.ShipOwner != null ? ship.ShipOwner.GetType().Name : "None")}");
                }
            }
            else
            {
                Debug.Log("[ShipSelectionDebug] No ship hit - Check ship layer settings");
            }
        }
    }
}