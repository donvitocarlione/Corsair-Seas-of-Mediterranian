using UnityEngine;

[RequireComponent(typeof(ShipSelectionHandler))]
public class Ship : SeaEntityBase, ISelectable
{
    public string shipName;
    public string shipType;
    public bool isCombatShip;
    public float shipSize = 0.5f;

    public int maxCrew = 10;
    public int currentCrew;
    public float maxCargo = 100f;
    public float currentCargo;

    public float baseAttack = 10f;
    public float baseDefense = 10f;
    public float currentAttack;
    public float currentDefense;

    public float health = 100f;
    private bool isSelected;
    public bool IsSelected => isSelected;

    public Pirate owner;
    private ShipSelectionHandler selectionHandler;

    private void Awake()
    {
        selectionHandler = GetComponent<ShipSelectionHandler>();
        ResetStats();
    }

    public void Initialize(FactionType faction, string shipName)
    {
        SetFaction(faction);
        this.shipName = shipName;
        ResetStats();
        isSelected = false;

        if (string.IsNullOrEmpty(shipType))
        {
            RandomizeShipProperties();
        }
    }

    private void RandomizeShipProperties()
    {
        string[] types = { "Sloop", "Brigantine", "Frigate", "Galleon" };
        shipType = types[Random.Range(0, types.Length)];
        
        switch (shipType)
        {
            case "Sloop":
                shipSize = Random.Range(0.1f, 0.3f);
                break;
            case "Brigantine":
                shipSize = Random.Range(0.3f, 0.5f);
                break;
            case "Frigate":
                shipSize = Random.Range(0.5f, 0.7f);
                break;
            case "Galleon":
                shipSize = Random.Range(0.7f, 0.9f);
                break;
        }

        maxCrew = Mathf.RoundToInt(maxCrew * (1f + shipSize));
        maxCargo = maxCargo * (1f + shipSize * 2f);
        baseAttack = baseAttack * (1f + shipSize);
        baseDefense = baseDefense * (1f + shipSize);
        
        isCombatShip = Random.value > 0.4f;
        if (isCombatShip)
        {
            baseAttack *= 1.5f;
            baseDefense *= 1.2f;
            maxCargo *= 0.7f;
        }
    }

    public void ResetStats()
    {
        health = 100f;
        currentCrew = maxCrew;
        currentCargo = 0f;
        currentAttack = baseAttack;
        currentDefense = baseDefense;
        isSelected = false;
    }

    public bool Select()
    {
        if (selectionHandler.Select())
        {
            isSelected = true;
            return true;
        }
        return false;
    }

    public void Deselect()
    {
        isSelected = false;
        selectionHandler.Deselect();
    }

    public void SetOwner(Pirate newOwner)
    {
        if (owner != null)
        {
            owner.RemoveShip(this);
        }

        owner = newOwner;
        
        if (owner != null && owner is Player)
        {
            SetFaction(FactionType.Player);
        }
    }
}
