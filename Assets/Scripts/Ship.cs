using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Ship Info")]
    public string shipName;
    public string shipType;
    public FactionType faction;

    [Header("Crew & Cargo")]
    public int maxCrew = 10;
    public int currentCrew;
    public float maxCargo = 100f;
    public float currentCargo;

    [Header("Status")]
    public float health = 100f;
    public bool isSelected;

    [Header("References")]
    public Pirate owner;
    public GameObject selectionIndicator;  // Assign a visual indicator object

    public void Initialize(FactionType faction, string shipName)
    {
        this.faction = faction;
        this.shipName = shipName;
        this.health = 100f;
        this.currentCrew = maxCrew;
        this.currentCargo = 0f;
        isSelected = false;
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    private void Start()
    {
        isSelected = false;
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    public bool Select()
    {
        isSelected = true;
        ShowSelectionIndicator(true);
        Debug.Log($"{shipName} selected");
        return true;
    }

    public void Deselect()
    {
        isSelected = false;
        ShowSelectionIndicator(false);
    }

    private void ShowSelectionIndicator(bool show)
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(show);
    }

    public void SetOwner(Pirate newOwner)
    {
        owner = newOwner;
        if (owner != null && owner is Player)
            faction = FactionType.Player;
    }

    private void OnMouseDown()
    {
        if (owner != null && owner is Player)
        {
            owner.SelectShip(this);
        }
    }
}
