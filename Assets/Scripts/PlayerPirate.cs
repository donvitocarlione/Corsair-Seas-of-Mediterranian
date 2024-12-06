using UnityEngine;

public class PlayerPirate : MonoBehaviour
{
    public static PlayerPirate Instance { get; private set; }

    [Header("Player Info")]
    public string pirateUsername;
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
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayer();
        }
        else
        {
            Destroy(gameObject);
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
    }

    public void SpawnPlayerShip()
    {
        if (shipPrefab != null)
        {
            GameObject shipObj = Instantiate(shipPrefab, Vector3.zero, Quaternion.identity);
            currentShip = shipObj.GetComponent<Ship>();
            if (currentShip != null)
            {
                currentShip.Initialize(faction, pirateUsername + "'s Ship", 150f, 15f);
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
                }
            }
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
        }
    }

    public void ChangeReputationWithFaction(FactionType faction, int amount)
    {
        reputation += amount;
        // You can add specific faction reputation tracking here
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }
}
