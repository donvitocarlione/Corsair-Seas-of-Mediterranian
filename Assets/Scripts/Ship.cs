using UnityEngine;

public class Ship : SeaEntityBase
{
    public string shipName;
    private bool isSelected;
    private Pirate ownerPirate;  // Renamed the field to avoid naming conflict

    public bool IsSelected => isSelected;
    public Pirate owner => ownerPirate;  // Property now returns the private field

    protected virtual void Awake()
    {
        // Initialize any required components or variables
        if (string.IsNullOrEmpty(shipName))
        {
            shipName = $"Ship_{Random.Range(1000, 9999)}";
        }
    }

    public void Initialize(FactionType faction, string name)
    {
        shipName = name;
        SetFaction(faction);
        Debug.Log($"Initialized ship {shipName} with faction {faction}");
    }

    public void Select()
    {
        isSelected = true;
        // Show selection indicator
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.ShowSelectionAt(transform);
            Debug.Log($"Selected ship {shipName}");
        }
    }

    public void Deselect()
    {
        isSelected = false;
        // Hide selection indicator
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.HideSelection();
            Debug.Log($"Deselected ship {shipName}");
        }
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
            Debug.Log($"Set owner of {shipName} to {ownerPirate.GetType().Name}");
        }
        else
        {
            Debug.Log($"Cleared owner of {shipName}");
        }
    }

    protected override void OnFactionChanged()
    {
        base.OnFactionChanged();
        Debug.Log($"Ship {shipName} faction changed to {Faction}");
    }
}
