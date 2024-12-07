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
    public bool isPlayerFaction = false;  // Whether this faction is controlled by the player
}

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [Header("References")]
    public FactionManager factionManager;
    public DiplomacySystem diplomacySystem;

    [Header("Faction Ship Settings")]
    public List<FactionShipData> factionShipData;
    public GameObject playerShipPrefab;   // Default player ship

    [Header("Spawn Settings")]
    public float minSpawnDistance = 50f;  // Minimum distance between spawned ships
    public float safeSpawnAttempts = 10;  // How many times to try finding a safe spawn point

    private Dictionary<FactionType, List<Ship>> activeShips = new Dictionary<FactionType, List<Ship>>();
    private List<Vector3> occupiedPositions = new List<Vector3>();
    private Transform shipsParent;

    public FactionType PlayerFaction { get; private set; }

    void Awake()
    {
        Debug.Log("ShipManager Awake");
        if (Instance == null)
        {
            Instance = this;
            // Don't use DontDestroyOnLoad for now
            Debug.Log("ShipManager instance created");
            
            // Create a parent object for all ships
            shipsParent = new GameObject("SpawnedShips").transform;
            shipsParent.parent = transform; // Parent it to ShipManager

            // Find player faction
            var playerFactionData = factionShipData.Find(data => data.isPlayerFaction);
            if (playerFactionData != null)
            {
                PlayerFaction = playerFactionData.faction;
                Debug.Log($"Player faction set to: {PlayerFaction}");
            }
            else
            {
                Debug.LogWarning("No player faction marked in ShipManager!");
            }
        }
        else
        {
            Debug.Log("Duplicate ShipManager found, destroying");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("ShipManager Start");
        if (factionManager == null)
        {
            Debug.LogError("FactionManager reference is missing!");
            return;
        }

        if (diplomacySystem == null)
        {
            Debug.LogError("DiplomacySystem reference is missing!");
            return;
        }

        if (factionShipData == null || factionShipData.Count == 0)
        {
            Debug.LogError("No faction ship data configured!");
            return;
        }

        Debug.Log($"Found {factionShipData.Count} faction configurations");
        foreach (var data in factionShipData)
        {
            Debug.Log($"Faction: {data.faction}, Ship Prefabs: {(data.shipPrefabs != null ? data.shipPrefabs.Count : 0)}, Initial Count: {data.initialShipCount}");
        }

        InitializeShipsForAllFactions();
    }

    public void InitializeShipsForAllFactions()
    {
        Debug.Log("Starting InitializeShipsForAllFactions");
        
        foreach (var data in factionShipData)
        {
            Debug.Log($"Processing faction: {data.faction}");
            if (!activeShips.ContainsKey(data.faction))
            {
                activeShips[data.faction] = new List<Ship>();
            }

            for (int i = 0; i < data.initialShipCount; i++)
            {
                Debug.Log($"Attempting to spawn ship {i + 1} for {data.faction}");
                Ship newShip = SpawnShipForFaction(data.faction);
                if (newShip != null)
                {
                    Debug.Log($"Successfully spawned ship {i + 1} for {data.faction} at {newShip.transform.position}");
                }
                else
                {
                    Debug.LogError($"Failed to spawn ship {i + 1} for {data.faction}");
                }
            }
        }
    }

    public Ship SpawnShipForFaction(FactionType faction)
    {
        Debug.Log($"SpawnShipForFaction called for {faction}");
        FactionShipData data = GetFactionShipData(faction);
        if (data == null)
        {
            Debug.LogError($"No FactionShipData found for faction {faction}");
            return null;
        }

        if (data.shipPrefabs == null || data.shipPrefabs.Count == 0)
        {
            Debug.LogError($"No ship prefabs assigned for faction {faction}");
            return null;
        }

        // Get random prefab
        GameObject shipPrefab = data.shipPrefabs[Random.Range(0, data.shipPrefabs.Count)];
        if (shipPrefab == null)
        {
            Debug.LogError($"Ship prefab is null for faction {faction}");
            return null;
        }

        // Get spawn position
        Vector3 spawnPos = GetSafeSpawnPosition(data.spawnArea, data.spawnRadius);
        
        Debug.Log($"Spawning ship for {faction} at position {spawnPos}");
        GameObject shipObj = Instantiate(shipPrefab, spawnPos, Quaternion.identity, shipsParent);
        
        // Add required components if they don't exist
        if (shipObj.GetComponent<MeshRenderer>() == null)
        {
            Debug.LogWarning($"Adding MeshRenderer to ship {shipObj.name}");
            shipObj.AddComponent<MeshRenderer>();
        }

        if (shipObj.GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"Adding BoxCollider to ship {shipObj.name}");
            shipObj.AddComponent<BoxCollider>();
        }

        Ship ship = shipObj.GetComponent<Ship>();
        if (ship == null)
        {
            Debug.LogWarning($"Adding Ship component to {shipObj.name}");
            ship = shipObj.AddComponent<Ship>();
        }
        
        if (ship != null)
        {
            string shipName = $"{faction} Ship {activeShips[faction].Count + 1}";
            ship.Initialize(faction, shipName);
            
            // Add appropriate control components based on faction
            if (data.isPlayerFaction)
            {
                // Add player control components
                ShipSelector selector = shipObj.AddComponent<ShipSelector>();
                selector.PlayerFaction = faction;
            }
            else
            {
                // Add AI control components
                AIShipController aiController = shipObj.AddComponent<AIShipController>();
                aiController.Initialize(ship, diplomacySystem);
            }

            RegisterShip(faction, ship);
            occupiedPositions.Add(spawnPos);

            // Ensure the ship is visible and active
            shipObj.SetActive(true);
            
            return ship;
        }
        
        Debug.LogError($"Failed to setup ship for faction {faction}");
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
        Debug.Log($"Registered ship for faction {faction}. Total ships: {activeShips[faction].Count}");
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
            Debug.Log($"Removed ship from faction {faction}");
        }
    }

    public void OnShipDestroyed(Ship ship)
    {
        RemoveShip(ship.faction, ship);
        Debug.Log($"Ship destroyed for faction {ship.faction}");
    }
}
