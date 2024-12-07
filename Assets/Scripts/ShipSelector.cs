using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    public FactionType PlayerFaction { get; set; }
    private Ship ship;

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
        ship.Select();
    }

    public void Deselect()
    {
        if (ship != null)
        {
            ship.Deselect();
        }
    }
}
