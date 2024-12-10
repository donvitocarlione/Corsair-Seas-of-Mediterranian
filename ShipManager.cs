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
        Debug.Log("[ShipManager] Awake start");
        ValidatePlayerFactionConfig();
        
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

    private void ValidatePlayerFactionConfig()
    {
        int playerFactionCount = 0;
        foreach (var data in factionShipData)
        {
            if (data.isPlayerFaction)
            {
                playerFactionCount++;
            }
        }

        if (playerFactionCount > 1)
        {
            Debug.LogError("[ShipManager] Multiple player factions detected! This will cause issues with ship ownership.");
            foreach (var data in factionShipData)
            {
                Debug.Log($"Faction: {data.faction}, IsPlayerFaction: {data.isPlayerFaction}");
            }
        }
        else if (playerFactionCount == 0)
        {
            Debug.LogError("[ShipManager] No player faction configured!");
        }
    }

    private void InitializeManager()
    {
        Debug.Log("[ShipManager] Initializing manager");
        CreateContainers();
        isInitialized = ValidateConfiguration();
        
        if (!isInitialized)
        {
            Debug.LogError("[ShipManager] Initialization failed");
            enabled = false;
        }
    }

    private void CreateContainers()
    {
        shipsParent = new GameObject("Ships").transform;
        shipsParent.parent = transform;
        
        piratesParent = new GameObject("Pirates").transform;
        piratesParent.parent = transform;
        Debug.Log("[ShipManager] Created containers");
    }

    private bool ValidateConfiguration()
    {
        Debug.Log("[ShipManager] Validating configuration");
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

        // Log faction configurations
        foreach (var data in factionShipData)
        {
            Debug.Log($"[ShipManager] Faction config - Type: {data.faction}, IsPlayerFaction: {data.isPlayerFaction}, Ships: {data.initialShipCount}, Pirates: {data.initialPirateCount}");
        }

        return factionShipData.TrueForAll(data => data.Validate());
    }

    void Start()
    {
        Debug.Log("[ShipManager] Start");
        if (Instance == this && isInitialized)
        {
            InitializePlayerFaction();
        }
    }

    private void InitializePlayerFaction()
    {
        Debug.Log("[ShipManager] Initializing player faction");
        var playerData = factionShipData.Find(data => data.isPlayerFaction);
        if (playerData == null)
        {
            Debug.LogError("[ShipManager] No player faction configured!");
            return;
        }

        playerFaction = playerData.faction;
        Debug.Log($"[ShipManager] Player faction set to: {playerFaction}");
        
        playerInstance = FindAnyObjectByType<Player>();
        if (playerInstance == null)
        {
            Debug.LogError("[ShipManager] No Player component found!");
            return;
        }

        playerInstance.SetFaction(playerFaction);
        Debug.Log($"[ShipManager] Player instance found and faction set to {playerFaction}");
        
        InitializeAllFactions();
    }

    private void InitializeAllFactions()
    {
        Debug.Log("[ShipManager] Initializing all factions");
        foreach (var data in factionShipData)
        {
            Debug.Log($"[ShipManager] Processing faction: {data.faction} (IsPlayerFaction: {data.isPlayerFaction}, ShouldBePlayerOwned: {data.faction == playerFaction})");
            
            bool isRealPlayerFaction = data.isPlayerFaction && data.faction == playerFaction;
            Debug.Log($"[ShipManager] IsRealPlayerFaction: {isRealPlayerFaction}");
            
            if (isRealPlayerFaction)
            {
                InitializePlayerShips(data);
            }
            else
            {
                InitializePiratesForFaction(data);
            }
        }
    }

    // Rest of the class remains the same...
}