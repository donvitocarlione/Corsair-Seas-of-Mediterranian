using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    public FactionType PlayerFaction { get; set; }
    private Ship ship;
    private bool isSelected = false;

    void Start()
    {
        ship = GetComponent<Ship>();
        if (ship == null)
        {
            Debug.LogError("Ship component not found on object with ShipSelector");
        }
    }

    public bool IsSelectable()
    {
        return ship != null && ship.faction == PlayerFaction;
    }

    public void Select()
    {
        if (!IsSelectable()) return;
        
        isSelected = true;
        ship.Select();
    }

    public void Deselect()
    {
        isSelected = false;
        if (ship != null)
        {
            ship.Deselect();
        }
    }
}
