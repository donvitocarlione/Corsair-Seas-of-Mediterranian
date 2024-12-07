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
    public GameObject piratePrefab;

    [Header("Faction Ship Settings")]
    public List<FactionShipData> factionShipData;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 50f;
    public float safeSpawnAttempts = 10;

    private List<Vector3> occupiedPositions = new List<Vector3>();
    private Transform shipsParent;
    private Transform piratesParent;

    public FactionType PlayerFaction { get; private set; }
    private Player playerInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateContainers();
            SetupPlayerFaction();
        }
        else
        {
            Destroy(gameObject);
        }
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
            Debug.Log($"Player faction set to: {PlayerFaction}");

            // Find player instance after a short delay to ensure all components are initialized
            StartCoroutine(SetupPlayerWithDelay());
        }
        else
        {
            Debug.LogError("No player faction marked in ShipManager!");
            PlayerFaction = FactionType.None;
        }
    }

    private System.Collections.IEnumerator SetupPlayerWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // Short delay to ensure everything is initialized

        playerInstance = FindObjectOfType<Player>();
        if (playerInstance != null)
        {
            Debug.Log("Found player instance, setting faction");
            playerInstance.SetFaction(PlayerFaction);
        }
        else
        {
            Debug.LogError("No Player component found in the scene!");
        }

        // Initialize ships after player setup
        InitializeShipsAndPirates();
    }

    public void InitializeShipsAndPirates()
    {
        foreach (var data in factionShipData)
        {
            if (data.isPlayerFaction)
            {
                if (playerInstance != null)
                {
                    Debug.Log($"Initializing player ships for faction: {data.faction}");
                    
                    for (int i = 0; i < data.initialShipCount; i++)
                    {
                        Ship newShip = SpawnShipForFaction(data.faction);
                        if (newShip != null)
                        {
                            playerInstance.AddShip(newShip);
                            Debug.Log($"Added ship {newShip.shipName} to player");
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < data.initialPirateCount; i++)
                {
                    Pirate newPirate = SpawnPirate(data.faction);
                    if (newPirate != null)
                    {
                        for (int j = 0; j < data.initialShipCount / data.initialPirateCount; j++)
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
        if (data == null || data.shipPrefabs == null || data.shipPrefabs.Count == 0)
        {
            Debug.LogError($"No ship prefabs found for faction {faction}!");
            return null;
        }

        GameObject shipPrefab = data.shipPrefabs[Random.Range(0, data.shipPrefabs.Count)];
        Vector3 spawnPos = GetSafeSpawnPosition(data.spawnArea, data.spawnRadius);
        
        GameObject shipObj = Instantiate(shipPrefab, spawnPos, Quaternion.identity, shipsParent);
        Ship ship = shipObj.GetComponent<Ship>();
        
        if (ship != null)
        {
            string shipName = $"{faction}_Ship_{Random.Range(1000, 9999)}";
            ship.Initialize(faction, shipName);
            
            if (!data.isPlayerFaction)
            {
                AIShipController aiController = shipObj.AddComponent<AIShipController>();
                aiController.Initialize(ship);
            }

            occupiedPositions.Add(spawnPos);
            Debug.Log($"Spawned ship {shipName} for faction {faction}");
            return ship;
        }
        
        Debug.LogError($"Ship prefab {shipPrefab.name} doesn't have a Ship component!");
        Destroy(shipObj);
        return null;
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

    public void OnShipDestroyed(Ship ship)
    {
        occupiedPositions.Remove(ship.transform.position);
    }
}
