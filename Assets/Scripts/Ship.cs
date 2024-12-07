using UnityEngine;

public class Ship : SeaEntityBase
{
    public string shipName;
    private bool isSelected;
    private Pirate owner;

    public bool IsSelected => isSelected;

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
        if (owner != null)
        {
            owner.RemoveShip(this);
        }

        owner = newOwner;

        if (owner != null)
        {
            SetFaction(owner.Faction);
        }
    }

    // Properties
    public Pirate owner => owner;
}
