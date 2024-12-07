using UnityEngine;

public class Ship : MonoBehaviour
{
    public string shipName;
    public string shipType;
    public FactionType faction;
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
    public bool isSelected;

    public Pirate owner;
    public GameObject selectionIndicator;

    private void Awake()
    {
        ResetStats();
    }

    public void Initialize(FactionType faction, string shipName)
    {
        this.faction = faction;
        this.shipName = shipName;
        ResetStats();
        isSelected = false;
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);

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
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    public bool Select()
    {
        isSelected = true;
        if (selectionIndicator != null)
            selectionIndicator.SetActive(true);
        return true;
    }

    public void Deselect()
    {
        isSelected = false;
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
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
            faction = FactionType.Player;
        }
    }

    private void OnMouseDown()
    {
        if (owner != null && owner is Player)
        {
            owner.SelectShip(this);
        }
    }
}
