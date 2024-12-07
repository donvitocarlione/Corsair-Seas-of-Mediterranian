using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FactionShipData
{
    public FactionType faction;
    public List<GameObject> shipPrefabs;
    public Vector3 spawnArea;
    public float spawnRadius = 100f;
    public int initialShipCount = 3;
    public bool isPlayerFaction = false;
    public int initialPirateCount = 2;
}

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [Header("References")]
    public FactionManager factionManager;
    public GameObject piratePrefab;

    [Header("Faction Ship Settings")]
    public List<FactionShipData> factionShipData;
    public GameObject playerShipPrefab;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 50f;
    public float safeSpawnAttempts = 10;

    private Dictionary<FactionType, List<Ship>> activeShips = new Dictionary<FactionType, List<Ship>>();
    private Dictionary<FactionType, List<Pirate>> activePirates = new Dictionary<FactionType, List<Pirate>>();
    private List<Vector3> occupiedPositions = new List<Vector3>();
    private Transform shipsParent;
    private Transform piratesParent;

    public FactionType PlayerFaction { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            shipsParent = new GameObject("SpawnedShips").transform;
            shipsParent.parent = transform;
            
            piratesParent = new GameObject("SpawnedPirates").transform;
            piratesParent.parent = transform;

            var playerFactionData = factionShipData.Find(data => data.isPlayerFaction);
            if (playerFactionData != null)
            {
                PlayerFaction = playerFactionData.faction;
            }
            else
            {
                Debug.LogWarning("No player faction marked in ShipManager!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeShipsAndPirates();
    }

    public void InitializeShipsAndPirates()
    {
        foreach (var data in factionShipData)
        {
            if (!activeShips.ContainsKey(data.faction))
            {
                activeShips[data.faction] = new List<Ship>();
            }
            if (!activePirates.ContainsKey(data.faction))
            {
                activePirates[data.faction] = new List<Pirate>();
            }

            List<Pirate> factionPirates = new List<Pirate>();
            for (int i = 0; i < data.initialPirateCount; i++)
            {
                Pirate newPirate = SpawnPirate(data.faction);
                if (newPirate != null)
                {
                    factionPirates.Add(newPirate);
                }
            }

            List<Ship> ships = new List<Ship>();
            for (int i = 0; i < data.initialShipCount; i++)
            {
                Ship newShip = SpawnShipForFaction(data.faction);
                if (newShip != null)
                {
                    ships.Add(newShip);
                }
            }

            AssignShipsToPirates(ships, factionPirates);
        }
    }

    private Pirate SpawnPirate(FactionType faction)
    {
        if (piratePrefab == null)
        {
            Debug.LogError("Pirate prefab is missing!");
            return null;
        }

        GameObject pirateObj = Instantiate(piratePrefab, Vector3.zero, Quaternion.identity, piratesParent);
        Pirate pirate = pirateObj.GetComponent<Pirate>();
        if (pirate == null)
        {
            pirate = pirateObj.AddComponent<Pirate>();
        }

        pirateObj.name = $"Pirate_{faction}_{activePirates[faction].Count + 1}";
        activePirates[faction].Add(pirate);
        
        return pirate;
    }

    private void AssignShipsToPirates(List<Ship> ships, List<Pirate> pirates)
    {
        int pirateIndex = 0;
        foreach (Ship ship in ships)
        {
            if (pirates.Count > 0)
            {
                Pirate pirate = pirates[pirateIndex];
                pirate.AddShip(ship);
                
                // Move to next pirate in round-robin fashion
                pirateIndex = (pirateIndex + 1) % pirates.Count;
            }
            else
            {
                Debug.LogWarning($"Ship {ship.shipName} remained unassigned - no pirates available");
            }
        }
    }

    public Ship SpawnShipForFaction(FactionType faction)
    {
        FactionShipData data = GetFactionShipData(faction);
        if (data == null || data.shipPrefabs == null || data.shipPrefabs.Count == 0)
        {
            return null;
        }

        GameObject shipPrefab = data.shipPrefabs[Random.Range(0, data.shipPrefabs.Count)];
        Vector3 spawnPos = GetSafeSpawnPosition(data.spawnArea, data.spawnRadius);
        
        GameObject shipObj = Instantiate(shipPrefab, spawnPos, Quaternion.identity, shipsParent);
        Ship ship = shipObj.GetComponent<Ship>();
        
        if (ship != null)
        {
            string shipName = $"{faction} Ship {activeShips[faction].Count + 1}";
            ship.Initialize(faction, shipName);
            
            if (!data.isPlayerFaction)
            {
                AIShipController aiController = shipObj.AddComponent<AIShipController>();
                aiController.Initialize(ship);
            }

            RegisterShip(faction, ship);
            occupiedPositions.Add(spawnPos);
            return ship;
        }
        
        Destroy(shipObj);
        return null;
    }

    private void RegisterShip(FactionType faction, Ship ship)
    {
        activeShips[faction].Add(ship);
        factionManager.RegisterShip(faction, ship);
    }

    private Vector3 GetSafeSpawnPosition(Vector3 centerPoint, float radius)
    {
        for (int i = 0; i < safeSpawnAttempts; i++)
        {
            Vector3 randomPos = centerPoint + Random.insideUnitSphere * radius;
            randomPos.y = 0;

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
        return activeShips.TryGetValue(faction, out List<Ship> ships) ? ships : new List<Ship>();
    }

    public List<Pirate> GetFactionPirates(FactionType faction)
    {
        return activePirates.TryGetValue(faction, out List<Pirate> pirates) ? pirates : new List<Pirate>();
    }

    public void RemoveShip(FactionType faction, Ship ship)
    {
        if (activeShips.ContainsKey(faction))
        {
            activeShips[faction].Remove(ship);
            occupiedPositions.Remove(ship.transform.position);
            factionManager.UnregisterShip(faction, ship);
        }
    }

    public void RemovePirate(FactionType faction, Pirate pirate)
    {
        if (activePirates.ContainsKey(faction))
        {
            activePirates[faction].Remove(pirate);
        }
    }

    public void OnShipDestroyed(Ship ship)
    {
        RemoveShip(ship.faction, ship);
    }
}
