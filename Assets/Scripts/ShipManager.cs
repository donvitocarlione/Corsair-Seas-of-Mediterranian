using UnityEngine;
using System.Collections.Generic;

public class ShipManager : MonoBehaviour
{
    public FactionManager factionManager;
    public List<Ship> ships = new List<Ship>();
    public string pirateNamePrefix = "Pirate ";
    public List<GameObject> shipPrefabs;
    public List<FactionType> factions;
    public List<Vector3> spawnPositions;
    private List<Pirate> pirates = new List<Pirate>();

    [Header("Ship Damage Setup")]
    public GameObject defaultHitEffect;
    public GameObject defaultFireEffect;
    public AudioClip defaultHitSound;

    void Start()
    {
        InitializeShips();
        AssignFactionsAndPirates();
        SetupDamageSystems();
    }

    private void InitializeShips()
    {
        if (shipPrefabs == null || factions == null || spawnPositions == null)
        {
            Debug.LogError("Required components not assigned in ShipManager!");
            return;
        }

        for (int i = 0; i < shipPrefabs.Count; i++)
        {
            if (i < spawnPositions.Count && i < factions.Count)
            {
                GameObject shipObject = Instantiate(shipPrefabs[i], spawnPositions[i], Quaternion.identity);
                Ship ship = shipObject.GetComponent<Ship>();
                if (ship != null)
                {
                    ship.Initialize(factions[i % factions.Count], "Ship " + (i + 1));
                    ships.Add(ship);
                }
                else
                {
                    Debug.LogError("Ship component not found on ship prefab!");
                    Destroy(shipObject);
                }
            }
        }
    }

    private void SetupDamageSystems()
    {
        foreach (Ship ship in ships)
        {
            DamageSystem damageSystem = ship.GetComponent<DamageSystem>();
            if (damageSystem != null)
            {
                // Setup default damage zones if none exist
                if (damageSystem.damageZones == null || damageSystem.damageZones.Length == 0)
                {
                    damageSystem.damageZones = new DamageSystem.DamageZone[]
                    {
                        new DamageSystem.DamageZone
                        {
                            zoneName = "Hull",
                            damageMultiplier = 1f,
                            isCriticalZone = true,
                            isRepairable = true,
                            maxHealth = 100f,
                            currentHealth = 100f,
                            visualDamageEffect = defaultHitEffect
                        },
                        new DamageSystem.DamageZone
                        {
                            zoneName = "Sails",
                            damageMultiplier = 1.5f,
                            isCriticalZone = false,
                            isRepairable = true,
                            maxHealth = 75f,
                            currentHealth = 75f,
                            visualDamageEffect = defaultHitEffect
                        },
                        new DamageSystem.DamageZone
                        {
                            zoneName = "Deck",
                            damageMultiplier = 0.8f,
                            isCriticalZone = false,
                            isRepairable = true,
                            maxHealth = 150f,
                            currentHealth = 150f,
                            visualDamageEffect = defaultHitEffect
                        }
                    };
                }

                // Set default effects if not already set
                if (damageSystem.fireEffect == null)
                    damageSystem.fireEffect = defaultFireEffect;
                if (damageSystem.hitSound == null)
                    damageSystem.hitSound = defaultHitSound;
            }
        }
    }

    private void AssignFactionsAndPirates()
    {
        if (factionManager == null || ships.Count == 0) return;

        List<FactionType> factionsWithShips = new List<FactionType>();

        foreach (var ship in ships)
        {
            if (ship == null) continue;

            FactionType factionToAssign = AssignFaction(factionsWithShips);
            ship.faction = factionToAssign;
            try
            {
                factionManager.RegisterShip(factionToAssign, ship);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error registering ship: {e.Message}");
            }
            factionsWithShips.Add(factionToAssign);
            CreatePirate(factionToAssign, ship.transform.position);
        }
    }

    private FactionType AssignFaction(List<FactionType> factionsWithShips)
    {
        List<FactionType> availableFactions = new List<FactionType>();
        foreach (FactionType ft in System.Enum.GetValues(typeof(FactionType)))
        {
            if (!factionsWithShips.Contains(ft))
            {
                availableFactions.Add(ft);
            }
        }

        if (availableFactions.Count > 0)
            return availableFactions[Random.Range(0, availableFactions.Count)];
        else
            return (FactionType)Random.Range(0, System.Enum.GetValues(typeof(FactionType)).Length);
    }

    private void CreatePirate(FactionType faction, Vector3 position)
    {
        string pirateName = pirateNamePrefix + Random.Range(1, 1000);
        Pirate pirate = new Pirate(faction, pirateName, 100, 1.5f);
        
        GameObject pirateObject = new GameObject(pirateName);
        pirateObject.transform.position = position;
        PirateController pirateController = pirateObject.AddComponent<PirateController>();
        pirateController.Initialize(pirate);
        pirates.Add(pirate);
    }

    public void RepairAllShipsInFaction(FactionType faction, float repairAmount)
    {
        foreach (var ship in ships)
        {
            if (ship != null && ship.faction == faction)
            {
                var damageSystem = ship.GetComponent<DamageSystem>();
                if (damageSystem != null)
                {
                    damageSystem.RepairZone("Hull", repairAmount);
                    damageSystem.RepairZone("Sails", repairAmount);
                    damageSystem.RepairZone("Deck", repairAmount);
                }
            }
        }
    }
}