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

    public bool Validate()
    {
        if (shipPrefabs == null || shipPrefabs.Count == 0)
        {
            Debug.LogError($"No ship prefabs assigned for faction {faction}");
            return false;
        }

        foreach (var prefab in shipPrefabs)
        {
            if (prefab == null)
            {
                Debug.LogError($"Null ship prefab found in faction {faction}");
                return false;
            }

            if (prefab.GetComponent<Ship>() == null)
            {
                Debug.LogError($"Ship prefab {prefab.name} for faction {faction} is missing Ship component");
                return false;
            }
        }

        if (spawnRadius <= 0)
        {
            Debug.LogError($"Invalid spawn radius for faction {faction}");
            return false;
        }

        return true;
    }
}

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [Header("References")]
    public GameObject piratePrefab;

    [Header("Faction Ship Settings")]
    public List<FactionShipData> factionShipData;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 50f;
    public float safeSpawnAttempts = 10;

    private List<Vector3> occupiedPositions = new List<Vector3>();
    private Transform shipsParent;
    private Transform piratesParent;
    private bool isInitialized = false;

    public FactionType PlayerFaction { get; private set; }
    private Player playerInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateContainers();
            if (ValidateConfiguration())
            {
                isInitialized = true;
            }
            else
            {
                Debug.LogError("ShipManager initialization failed due to invalid configuration");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (Instance == this && isInitialized)
        {
            SetupPlayerFaction();
        }
    }

    private bool ValidateConfiguration()
    {
        if (piratePrefab == null)
        {
            Debug.LogError("Pirate prefab is not assigned in ShipManager!");
            return false;
        }

        if (factionShipData == null || factionShipData.Count == 0)
        {
            Debug.LogError("No faction ship data configured in ShipManager!");
            return false;
        }

        foreach (var data in factionShipData)
        {
            if (!data.Validate())
            {
                Debug.LogError($"Invalid configuration for faction {data.faction}");
                return false;
            }
        }

        return true;
    }

    private void CreateContainers()
    {
        shipsParent = new GameObject("Ships").transform;
        shipsParent.parent = transform;
        
        piratesParent = new GameObject("Pirates").transform;
        piratesParent.parent = transform;
    }

    private void SetupPlayerFaction()
    {
        var playerFactionData = factionShipData.Find(data => data.isPlayerFaction);
        if (playerFactionData != null)
        {
            PlayerFaction = playerFactionData.faction;
            playerInstance = FindObjectOfType<Player>();
            
            if (playerInstance != null)
            {
                playerInstance.SetFaction(PlayerFaction);
                InitializeShipsAndPirates();
            }
            else
            {
                Debug.LogError("No Player component found in the scene!");
                PlayerFaction = FactionType.None;
            }
        }
        else
        {
            Debug.LogError("No player faction marked in ShipManager!");
            PlayerFaction = FactionType.None;
        }
    }

    private void InitializeShipsAndPirates()
    {
        foreach (var data in factionShipData)
        {
            if (!data.Validate()) continue;

            if (data.isPlayerFaction)
            {
                InitializePlayerShips(data);
            }
            else
            {
                InitializePiratesAndShips(data);
            }
        }
    }

    private void InitializePlayerShips(FactionShipData data)
    {
        if (playerInstance == null) return;

        for (int i = 0; i < data.initialShipCount; i++)
        {
            Ship newShip = SpawnShipForFaction(data.faction);
            if (newShip != null)
            {
                playerInstance.AddShip(newShip);
            }
        }
    }

    private void InitializePiratesAndShips(FactionShipData data)
    {
        for (int i = 0; i < data.initialPirateCount; i++)
        {
            Pirate newPirate = SpawnPirate(data.faction);
            if (newPirate != null)
            {
                int shipsPerPirate = data.initialShipCount / data.initialPirateCount;
                for (int j = 0; j < shipsPerPirate; j++)
                {
                    Ship newShip = SpawnShipForFaction(data.faction);
                    if (newShip != null)
                    {
                        newPirate.AddShip(newShip);
                    }
                }
            }
        }
    }

    private Pirate SpawnPirate(FactionType faction)
    {
        if (piratePrefab == null) return null;

        GameObject pirateObj = Instantiate(piratePrefab, Vector3.zero, Quaternion.identity, piratesParent);
        Pirate pirate = pirateObj.GetComponent<Pirate>();
        
        if (pirate == null)
        {
            Debug.LogError("Pirate prefab must have a Pirate component!");
            Destroy(pirateObj);
            return null;
        }

        pirateObj.name = $"Pirate_{faction}_{Random.Range(1000, 9999)}";
        pirate.SetFaction(faction);
        return pirate;
    }

    public Ship SpawnShipForFaction(FactionType faction)
    {
        FactionShipData data = GetFactionShipData(faction);
        if (data == null || !data.Validate()) return null;

        GameObject shipPrefab = data.shipPrefabs[Random.Range(0, data.shipPrefabs.Count)];
        Vector3 spawnPos = GetSafeSpawnPosition(data.spawnArea, data.spawnRadius);
        
        GameObject shipObj = Instantiate(shipPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0), shipsParent);
        Ship ship = shipObj.GetComponent<Ship>();
        
        if (ship != null)
        {
            string shipName = $"{faction}_Ship_{Random.Range(1000, 9999)}";
            ship.Initialize(faction, shipName);
            
            if (!data.isPlayerFaction)
            {
                AIShipController aiController = shipObj.GetComponent<AIShipController>();
                if (aiController == null)
                {
                    aiController = shipObj.AddComponent<AIShipController>();
                }
                aiController.Initialize(ship);
            }

            occupiedPositions.Add(spawnPos);
            return ship;
        }
        
        Destroy(shipObj);
        return null;
    }

    private Vector3 GetSafeSpawnPosition(Vector3 centerPoint, float radius)
    {
        for (int i = 0; i < safeSpawnAttempts; i++)
        {
            Vector3 randomPos = centerPoint + Random.insideUnitSphere * radius;
            randomPos.y = 0;

            if (IsSafePosition(randomPos))
            {
                return randomPos;
            }
        }

        Vector3 fallbackPos = centerPoint + Random.insideUnitSphere * radius;
        fallbackPos.y = 0;
        Debug.LogWarning("Using fallback spawn position");
        return fallbackPos;
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
}
