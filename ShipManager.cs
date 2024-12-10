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
    public bool isPlayerFaction;
    public int initialPirateCount = 2;

    public bool Validate()
    {
        if (shipPrefabs == null || shipPrefabs.Count == 0)
        {
            Debug.LogError($"Missing ship prefabs for faction {faction}");
            return false;
        }

        foreach (var prefab in shipPrefabs)
        {
            if (prefab == null || prefab.GetComponent<Ship>() == null)
            {
                Debug.LogError($"Invalid ship prefab configuration for faction {faction}");
                return false;
            }
        }

        return spawnRadius > 0;
    }
}

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [Header("References")]
    public GameObject piratePrefab;

    [Header("Faction Settings")]
    public List<FactionShipData> factionShipData;
    public float minSpawnDistance = 50f;
    public int maxSpawnAttempts = 10;

    private Transform shipsParent;
    private Transform piratesParent;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private bool isInitialized;
    private FactionType playerFaction;

    public FactionType PlayerFaction => playerFaction;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
            Debug.Log("[ShipManager] Instance initialized");
        }
        else
        {
            Debug.Log("[ShipManager] Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }

    private void InitializeManager()
    {
        ValidatePlayerFactionConfig();
        CreateContainers();
        isInitialized = ValidateConfiguration();
        
        if (!isInitialized)
        {
            Debug.LogError("[ShipManager] Initialization failed");
            enabled = false;
        }
    }

    private void ValidatePlayerFactionConfig()
    {
        int playerFactionCount = 0;
        FactionShipData playerFactionData = null;

        foreach (var data in factionShipData)
        {
            if (data.isPlayerFaction)
            {
                playerFactionCount++;
                playerFactionData = data;
            }
        }

        if (playerFactionCount == 0)
        {
            Debug.LogError("[ShipManager] No player faction configured!");
        }
        else if (playerFactionCount > 1)
        {
            Debug.LogError("[ShipManager] Multiple player factions detected!");
        }
        else if (playerFactionData != null)
        {
            playerFaction = playerFactionData.faction;
            Debug.Log($"[ShipManager] Player faction validated: {playerFaction}");
        }
    }

    private void CreateContainers()
    {
        shipsParent = new GameObject("Ships").transform;
        shipsParent.parent = transform;
        
        piratesParent = new GameObject("Pirates").transform;
        piratesParent.parent = transform;
    }

    private bool ValidateConfiguration()
    {
        if (piratePrefab == null)
        {
            Debug.LogError("[ShipManager] Missing pirate prefab!");
            return false;
        }

        if (factionShipData == null || factionShipData.Count == 0)
        {
            Debug.LogError("[ShipManager] No faction data configured!");
            return false;
        }

        return factionShipData.TrueForAll(data => data.Validate());
    }

    void Start()
    {
        if (Instance == this && isInitialized)
        {
            InitializeAllFactions();
        }
    }

    private void InitializeAllFactions()
    {
        Player playerInstance = Player.Instance;
        if (playerInstance == null)
        {
            Debug.LogError("[ShipManager] Cannot initialize factions - Player instance not found!");
            return;
        }

        // Set player's faction first
        playerInstance.SetFaction(playerFaction);
        Debug.Log($"[ShipManager] Set player faction to: {playerFaction}");

        foreach (var data in factionShipData)
        {
            if (data.isPlayerFaction)
            {
                // Use player instance's faction instead of data.faction
                InitializePlayerShips(playerInstance, data.initialShipCount);
            }
            else
            {
                InitializePiratesForFaction(data);
            }
        }
    }

    private void InitializePlayerShips(Player player, int shipCount)
    {
        Debug.Log($"[ShipManager] Initializing {shipCount} ships for player faction {player.Faction}");
        for (int i = 0; i < shipCount; i++)
        {
            // Initialize ship with owner's faction
            Ship ship = SpawnShipForFaction(player.Faction, true);
            if (ship != null)
            {
                // The ship's faction will be validated and updated when added to the player
                player.AddShip(ship);
                Debug.Log($"[ShipManager] Added ship {ship.ShipName} to player's fleet");
            }
        }
    }

    private void InitializePiratesForFaction(FactionShipData data)
    {
        for (int i = 0; i < data.initialPirateCount; i++)
        {
            if (SpawnPirateShip(data.faction) is Pirate pirate)
            {
                int shipsPerPirate = data.initialShipCount / data.initialPirateCount;
                for (int j = 0; j < shipsPerPirate; j++)
                {
                    Ship ship = SpawnShipForFaction(pirate.Faction, false);
                    if (ship != null)
                    {
                        pirate.AddShip(ship);
                    }
                }
            }
        }
    }

    public Ship SpawnShipForFaction(FactionType faction, bool isPlayerShip)
    {
        var data = GetFactionShipData(faction);
        if (data == null)
        {
            Debug.LogError($"[ShipManager] No data found for faction {faction}");
            return null;
        }

        var prefab = data.shipPrefabs[Random.Range(0, data.shipPrefabs.Count)];
        var spawnPos = GetSafeSpawnPosition(data.spawnArea, data.spawnRadius);
        
        var shipObj = Instantiate(prefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0), shipsParent);
        var ship = shipObj.GetComponent<Ship>();
        
        if (ship != null)
        {
            string shipName = $"{faction}_Ship_{Random.Range(1000, 9999)}";
            ship.Initialize(faction, shipName);
            
            if (!isPlayerShip && shipObj.GetComponent<AIShipController>() == null)
            {
                shipObj.AddComponent<AIShipController>().Initialize(ship);
            }

            occupiedPositions.Add(spawnPos);
            return ship;
        }
        
        Debug.LogError($"[ShipManager] Failed to get Ship component from prefab for faction {faction}");
        Destroy(shipObj);
        return null;
    }

    private Pirate SpawnPirateShip(FactionType faction)
    {
        var pirateObj = Instantiate(piratePrefab, Vector3.zero, Quaternion.identity, piratesParent);
        var pirate = pirateObj.GetComponent<Pirate>();
        
        if (pirate == null)
        {
            Debug.LogError("[ShipManager] Pirate prefab missing Pirate component!");
            Destroy(pirateObj);
            return null;
        }

        pirateObj.name = $"Pirate_{faction}_{Random.Range(1000, 9999)}";
        pirate.SetFaction(faction);
        return pirate;
    }

    private Vector3 GetSafeSpawnPosition(Vector3 center, float radius)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            randomPos.y = 0;

            if (IsSafePosition(randomPos))
            {
                return randomPos;
            }
        }

        return center + Random.insideUnitSphere * radius * 0.5f;
    }

    private bool IsSafePosition(Vector3 position)
    {
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Vector3.Distance(position, occupied) < minSpawnDistance)
            {
                return false;
            }
        }
        return true;
    }

    private FactionShipData GetFactionShipData(FactionType faction)
    {
        return factionShipData.Find(data => data.faction == faction);
    }

    public void OnShipDestroyed(Ship ship)
    {
        if (ship != null)
        {
            occupiedPositions.Remove(ship.transform.position);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void OnValidate()
    {
        if (minSpawnDistance < 0) minSpawnDistance = 50f;
        if (maxSpawnAttempts < 1) maxSpawnAttempts = 10;
    }
}