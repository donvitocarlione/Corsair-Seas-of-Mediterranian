using UnityEngine;
using System.Collections.Generic;

public class ShipManager : MonoBehaviour
{
    public FactionManager factionManager;
    public List<Ship> ships = new List<Ship>();
    public string pirateNamePrefix = "Pirate ";
    public List<GameObject> shipPrefabs; // List of ship prefabs to instantiate
    public List<FactionType> factions; // List of factions to assign to ships
    public List<Vector3> spawnPositions; // List of spawn positions
    private List<Pirate> pirates = new List<Pirate>(); // List to manage pirates

    void Start()
    {
        Debug.Log("Starting ShipManager"); // Added log
        Debug.Log("shipPrefabs: " + (shipPrefabs != null ? shipPrefabs.Count : "null")); // Added log
        Debug.Log("factions: " + (factions != null ? factions.Count : "null")); // Added log
        Debug.Log("spawnPositions: " + (spawnPositions != null ? spawnPositions.Count : "null")); // Added log
        InitializeShips();
        AssignFactionsAndPirates();
    }

    private void InitializeShips()
    {
        Debug.Log("Initializing Ships"); // Added log
        if (shipPrefabs == null || factions == null || spawnPositions == null)
        {
            Debug.LogError("shipPrefabs, factions, or spawnPositions are not assigned in ShipManager!");
            return;
        }

        Debug.Log($"Number of ship prefabs: {shipPrefabs.Count}"); // Added log
        Debug.Log($"Number of factions: {factions.Count}"); // Added log
        Debug.Log($"Number of spawn positions: {spawnPositions.Count}"); // Added log

        for (int i = 0; i < shipPrefabs.Count; i++)
        {
            if (i < spawnPositions.Count && i < factions.Count) // Added check for factions.Count
            {
                Debug.Log($"Spawning ship {i + 1} at position: {spawnPositions[i]}"); // Added log
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
            else
            {
                Debug.LogWarning($"Not enough spawn positions or factions provided for ship prefab at index {i}");
            }
        }
    }

    private void AssignFactionsAndPirates()
    {
        Debug.Log("Assigning Factions and Pirates"); // Added log
        Debug.Log("factionManager: " + factionManager); // Check if factionManager is null
        if (factionManager == null) {
            Debug.LogError("factionManager is null in AssignFactionsAndPirates!");
            return; // Exit early to prevent further errors
        }

        Debug.Log("Number of ships: " + ships.Count); // Check ship count

        if (ships.Count == 0)
        {
            Debug.LogError("ships list is empty in AssignFactionsAndPirates!");
            return; // Exit early
        }

        List<FactionType> factionsWithShips = new List<FactionType>();

        foreach (var ship in ships)
        {
            if (ship == null) {
                Debug.LogError("Null ship encountered in ships list!");
                continue; // Skip to the next ship if there are nulls in the list, avoiding the exception
            }

            if (ship.faction == null) {
                Debug.LogError("ship.faction is null for ship: " + ship.name);
                continue; // Skip if faction is null
            }

            if (ship.transform == null) {
                Debug.LogError("ship.transform is null for ship: " + ship.name);
                continue; // Skip if transform is null
            }

            FactionType factionToAssign = AssignFaction(factionsWithShips);
            ship.faction = factionToAssign;
            try {
                factionManager.RegisterShip(factionToAssign, ship); // Handle potential exceptions here
            } catch (System.Exception e) {
                Debug.LogError("Error during factionManager.RegisterShip: " + e.Message);
                // Consider what should happen if this fails. Should the ship be removed from the list or reassigned?
            }
            factionsWithShips.Add(factionToAssign);
            CreatePirate(factionToAssign, ship.transform.position);
        }
    }

    // Helper function
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
        else // If we run out of factions, assign to a random one
            return (FactionType)Random.Range(0, System.Enum.GetValues(typeof(FactionType)).Length);
    }

    private void CreatePirate(FactionType faction, Vector3 position)
    {
        string pirateName = pirateNamePrefix + Random.Range(1, 1000);
        Pirate pirate = new Pirate(faction, pirateName, 100, 1.5f);
        Debug.Log($"Created pirate: {pirate.name} for faction {pirate.faction}");

        // Instantiate a pirate GameObject
        GameObject pirateObject = new GameObject(pirateName);
        pirateObject.transform.position = position;
        PirateController pirateController = pirateObject.AddComponent<PirateController>();
        pirateController.Initialize(pirate);
        pirates.Add(pirate); // Add pirate to the list
    }
}
