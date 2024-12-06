using UnityEngine;

public class PlayerPirate : MonoBehaviour
{
    public static PlayerPirate Instance { get; private set; }

    [Header("Player Info")]
    public string pirateUsername = "Captain";
    public FactionType faction = FactionType.Pirates;
    public int level = 1;
    public float experience = 0f;
    public float experienceToNextLevel = 100f;

    [Header("Stats")]
    public int reputation = 0;
    public int gold = 0;
    public float attackBonus = 0f;
    public float defenseBonus = 0f;

    [Header("Ship References")]
    public Ship currentShip;
    public GameObject shipPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"PlayerPirate initialized with faction: {faction}");
            InitializePlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Double check ship initialization
        if (currentShip == null && shipPrefab != null)
        {
            SpawnPlayerShip();
        }
    }

    private void InitializePlayer()
    {
        if (string.IsNullOrEmpty(pirateUsername))
        {
            pirateUsername = "Captain " + Random.Range(1000, 9999);
        }

        if (currentShip == null && shipPrefab != null)
        {
            SpawnPlayerShip();
        }
        else if (shipPrefab == null)
        {
            Debug.LogError("Ship prefab is not assigned to PlayerPirate!");
        }

        Debug.Log($"Player initialized: {pirateUsername}, Faction: {faction}");
    }

    public void SpawnPlayerShip()
    {
        if (shipPrefab != null)
        {
            // Spawn ship slightly above water level to prevent physics issues
            Vector3 spawnPosition = new Vector3(0, 1f, 0);
            GameObject shipObj = Instantiate(shipPrefab, spawnPosition, Quaternion.identity);
            currentShip = shipObj.GetComponent<Ship>();
            
            if (currentShip != null)
            {
                currentShip.Initialize(faction, pirateUsername + "'s Ship", 150f, 15f);
                Debug.Log($"Player ship spawned and initialized with faction: {faction}");
                
                // Set up damage zones
                var damageSystem = currentShip.GetComponent<DamageSystem>();
                if (damageSystem)
                {
                    damageSystem.damageZones = new DamageSystem.DamageZone[]
                    {
                        new DamageSystem.DamageZone
                        {
                            zoneName = "Hull",
                            damageMultiplier = 1f,
                            isCriticalZone = true,
                            maxHealth = 150f,
                            currentHealth = 150f
                        },
                        new DamageSystem.DamageZone
                        {
                            zoneName = "Sails",
                            damageMultiplier = 1.5f,
                            isCriticalZone = false,
                            maxHealth = 100f,
                            currentHealth = 100f
                        },
                        new DamageSystem.DamageZone
                        {
                            zoneName = "Deck",
                            damageMultiplier = 0.8f,
                            isCriticalZone = false,
                            maxHealth = 120f,
                            currentHealth = 120f
                        }
                    };
                    Debug.Log("Ship damage zones initialized");
                }
                else
                {
                    Debug.LogError("DamageSystem component missing on ship prefab!");
                }
            }
            else
            {
                Debug.LogError("Ship component missing on ship prefab!");
            }
        }
        else
        {
            Debug.LogError("Cannot spawn player ship - shipPrefab is null!");
        }
    }

    public void GainExperience(float amount)
    {
        experience += amount;
        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel *= 1.5f;
        
        // Level up bonuses
        attackBonus += 0.1f;
        defenseBonus += 0.1f;
        
        if (currentShip != null)
        {
            currentShip.maxHealth += 10f;
            currentShip.currentHealth = currentShip.maxHealth;
            Debug.Log($"Player leveled up to {level}, ship health increased");
        }
    }

    public void ChangeReputationWithFaction(FactionType faction, int amount)
    {
        reputation += amount;
        Debug.Log($"Reputation changed by {amount} with {faction}");
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Added {amount} gold, new total: {gold}");
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log($"Spent {amount} gold, remaining: {gold}");
            return true;
        }
        Debug.Log($"Cannot spend {amount} gold - insufficient funds ({gold} available)");
        return false;
    }
}