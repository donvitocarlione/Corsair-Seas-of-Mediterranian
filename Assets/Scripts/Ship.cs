using UnityEngine;

public class Ship : SeaEntityBase
{
    public string shipName;
    private bool isSelected;
    private Pirate ownerPirate;  // Renamed the field to avoid naming conflict

    public bool IsSelected => isSelected;
    public Pirate owner => ownerPirate;  // Property now returns the private field

    public void Select()
    {
        isSelected = true;
        // Show selection indicator
        SelectionManager.Instance.ShowSelectionAt(transform);
    }

    public void Deselect()
    {
        isSelected = false;
        // Hide selection indicator
        SelectionManager.Instance.HideSelection();
    }

    public void SetOwner(Pirate newOwner)
    {
        if (ownerPirate != null)
        {
            ownerPirate.RemoveShip(this);
        }

        ownerPirate = newOwner;

        if (ownerPirate != null)
        {
            SetFaction(ownerPirate.Faction);
        }
    }
}
