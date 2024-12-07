using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FactionShipData
{
    public FactionType faction;
    public List<GameObject> shipPrefabs;  // Different ship types for this faction
    public Vector3 spawnArea;             // Center point of spawn area for this faction
    public float spawnRadius = 100f;      // Radius around spawn point where ships can appear
    public int initialShipCount = 3;      // How many ships this faction starts with
}

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [Header("References")]
    public FactionManager factionManager;

    [Header("Faction Ship Settings")]
    public List<FactionShipData> factionShipData;
    public GameObject playerShipPrefab;   // Default player ship

    [Header("Spawn Settings")]
    public float minSpawnDistance = 50f;  // Minimum distance between spawned ships
    public float safeSpawnAttempts = 10;  // How many times to try finding a safe spawn point

    private Dictionary<FactionType, List<Ship>> activeShips = new Dictionary<FactionType, List<Ship>>();
    private List<Vector3> occupiedPositions = new List<Vector3>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeShipsForAllFactions();
    }

    public void InitializeShipsForAllFactions()
    {
        Debug.Log("Initializing ships for all factions");
        
        foreach (var data in factionShipData)
        {
            if (!activeShips.ContainsKey(data.faction))
            {
                activeShips[data.faction] = new List<Ship>();
            }

            for (int i = 0; i < data.initialShipCount; i++)
            {
                SpawnShipForFaction(data.faction);
            }
        }
    }

    public Ship SpawnPlayerShip(FactionType playerFaction, Vector3 spawnPosition)
    {
        if (playerShipPrefab == null)
        {
            Debug.LogError("Player ship prefab not assigned!");
            return null;
        }

        GameObject shipObj = Instantiate(playerShipPrefab, spawnPosition, Quaternion.identity);
        Ship ship = shipObj.GetComponent<Ship>();
        
        if (ship != null)
        {
            ship.Initialize(playerFaction, "Player Ship");
            RegisterShip(playerFaction, ship);
            return ship;
        }
        
        Debug.LogError("Ship component not found on player ship prefab!");
        return null;
    }

    public Ship SpawnShipForFaction(FactionType faction)
    {
        FactionShipData data = GetFactionShipData(faction);
        if (data == null || data.shipPrefabs == null || data.shipPrefabs.Count == 0)
        {
            Debug.LogError($"No ship prefabs found for faction {faction}");
            return null;
        }

        // Get a random ship prefab for this faction
        GameObject shipPrefab = data.shipPrefabs[Random.Range(0, data.shipPrefabs.Count)];
        
        // Find a safe spawn position
        Vector3 spawnPos = GetSafeSpawnPosition(data.spawnArea, data.spawnRadius);
        
        // Spawn the ship
        GameObject shipObj = Instantiate(shipPrefab, spawnPos, Quaternion.identity);
        Ship ship = shipObj.GetComponent<Ship>();
        
        if (ship != null)
        {
            ship.Initialize(faction, $"{faction} Ship {activeShips[faction].Count + 1}");
            RegisterShip(faction, ship);
            occupiedPositions.Add(spawnPos);
            return ship;
        }
        
        Debug.LogError("Ship component not found on prefab!");
        Destroy(shipObj);
        return null;
    }

    private void RegisterShip(FactionType faction, Ship ship)
    {
        if (!activeShips.ContainsKey(faction))
        {
            activeShips[faction] = new List<Ship>();
        }
        
        activeShips[faction].Add(ship);
        factionManager.RegisterShip(faction, ship);
    }

    private Vector3 GetSafeSpawnPosition(Vector3 centerPoint, float radius)
    {
        for (int i = 0; i < safeSpawnAttempts; i++)
        {
            Vector3 randomPos = centerPoint + Random.insideUnitSphere * radius;
            randomPos.y = 0; // Keep ships at water level

            bool positionIsSafe = true;
            foreach (Vector3 occupied in occupiedPositions)
            {
                if (Vector3.Distance(randomPos, occupied) < minSpawnDistance)
                {
                    positionIsSafe = false;
                    break;
                }
            }

            if (positionIsSafe)
            {
                return randomPos;
            }
        }

        // If we couldn't find a safe position, just return a random one
        Vector3 fallbackPos = centerPoint + Random.insideUnitSphere * radius;
        fallbackPos.y = 0;
        return fallbackPos;
    }

    private FactionShipData GetFactionShipData(FactionType faction)
    {
        return factionShipData.Find(data => data.faction == faction);
    }

    public List<Ship> GetFactionShips(FactionType faction)
    {
        if (activeShips.TryGetValue(faction, out List<Ship> ships))
        {
            return ships;
        }
        return new List<Ship>();
    }

    public void RemoveShip(FactionType faction, Ship ship)
    {
        if (activeShips.ContainsKey(faction))
        {
            activeShips[faction].Remove(ship);
            occupiedPositions.Remove(ship.transform.position);
        }
    }

    // Call this when a ship is destroyed
    public void OnShipDestroyed(Ship ship)
    {
        RemoveShip(ship.faction, ship);
        // You might want to spawn a replacement ship after some delay
        // StartCoroutine(SpawnReplacementShipAfterDelay(ship.faction));
    }
}