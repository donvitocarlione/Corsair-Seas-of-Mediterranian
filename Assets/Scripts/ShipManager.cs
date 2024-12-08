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
    private Player playerInstance;

    public FactionType PlayerFaction => playerFaction;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManager()
    {
        CreateContainers();
        isInitialized = ValidateConfiguration();
        
        if (!isInitialized)
        {
            Debug.LogError("ShipManager initialization failed");
            enabled = false;
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
            Debug.LogError("Missing pirate prefab!");
            return false;
        }

        if (factionShipData == null || factionShipData.Count == 0)
        {
            Debug.LogError("No faction data configured!");
            return false;
        }

        return factionShipData.TrueForAll(data => data.Validate());
    }

    void Start()
    {
        if (Instance == this && isInitialized)
        {
            InitializePlayerFaction();
        }
    }

    private void InitializePlayerFaction()
    {
        var playerData = factionShipData.Find(data => data.isPlayerFaction);
        if (playerData == null)
        {
            Debug.LogError("No player faction configured!");
            return;
        }

        playerFaction = playerData.faction;
        playerInstance = FindAnyObjectByType<Player>();
        
        if (playerInstance == null)
        {
            Debug.LogError("No Player component found!");
            return;
        }

        playerInstance.SetFaction(playerFaction);
        InitializeAllFactions();
    }

    private void InitializeAllFactions()
    {
        foreach (var data in factionShipData)
        {
            if (data.isPlayerFaction)
            {
                InitializePlayerShips(data);
            }
            else
            {
                InitializePiratesForFaction(data);
            }
        }
    }

    private void InitializePlayerShips(FactionShipData data)
    {
        if (playerInstance == null) return;

        for (int i = 0; i < data.initialShipCount; i++)
        {
            if (SpawnShipForFaction(data.faction) is Ship ship)
            {
                playerInstance.AddShip(ship);
            }
        }
    }

    private void InitializePiratesForFaction(FactionShipData data)
    {
        for (int i = 0; i < data.initialPirateCount; i++)
        {
            if (SpawnPirate(data.faction) is Pirate pirate)
            {
                int shipsPerPirate = data.initialShipCount / data.initialPirateCount;
                for (int j = 0; j < shipsPerPirate; j++)
                {
                    if (SpawnShipForFaction(data.faction) is Ship ship)
                    {
                        pirate.AddShip(ship);
                    }
                }
            }
        }
    }

    public Ship SpawnShipForFaction(FactionType faction)
    {
        var data = GetFactionShipData(faction);
        if (data == null) return null;

        var prefab = data.shipPrefabs[Random.Range(0, data.shipPrefabs.Count)];
        var spawnPos = GetSafeSpawnPosition(data.spawnArea, data.spawnRadius);
        
        var shipObj = Instantiate(prefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0), shipsParent);
        var ship = shipObj.GetComponent<Ship>();
        
        if (ship != null)
        {
            string shipName = $"{faction}_Ship_{Random.Range(1000, 9999)}";
            ship.Initialize(faction, shipName);
            
            if (!data.isPlayerFaction && shipObj.GetComponent<AIShipController>() == null)
            {
                shipObj.AddComponent<AIShipController>().Initialize(ship);
            }

            occupiedPositions.Add(spawnPos);
            return ship;
        }
        
        Destroy(shipObj);
        return null;
    }

    private Pirate SpawnPirate(FactionType faction)
    {
        var pirateObj = Instantiate(piratePrefab, Vector3.zero, Quaternion.identity, piratesParent);
        var pirate = pirateObj.GetComponent<Pirate>();
        
        if (pirate == null)
        {
            Debug.LogError("Pirate prefab missing Pirate component!");
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