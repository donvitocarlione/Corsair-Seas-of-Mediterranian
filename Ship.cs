using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Ship : MonoBehaviour
{
    [Header("Ship Properties")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float sinkingThreshold = 20f;
    [SerializeField] protected GameObject waterSplashPrefab;
    [SerializeField] protected float waterFloodRate = 0.1f;
    [SerializeField] protected string shipName;
    [SerializeField] protected FactionType faction;

    [Header("Combat Properties")]
    [SerializeField] protected Transform[] portWeaponMounts;
    [SerializeField] protected Transform[] starboardWeaponMounts;
    [SerializeField] protected GameObject weaponPrefab;
    [SerializeField] protected float combatRange = 50f;
    [SerializeField] protected float armor = 1f;
    [SerializeField] protected float criticalHitMultiplier = 2f;

    protected float currentHealth;
    protected bool isSelected;
    protected bool isSinking;
    protected IShipOwner owner;
    protected List<Weapon> weapons = new List<Weapon>();

    protected Rigidbody shipRigidbody;
    protected Buoyancy buoyancyComponent;
    protected ShipMovement movementComponent;
    protected ShipSelectionHandler selectionHandler;

    public event UnityAction OnShipDestroyed;
    public event UnityAction<float> OnDamageTaken;
    public event UnityAction<Ship> OnTargetAcquired;

    public float Health => currentHealth;
    public bool IsSelected => isSelected;
    public bool IsSinking => isSinking;
    public IShipOwner ShipOwner => owner;
    public string ShipName => Name;
    public string Name => shipName;
    public FactionType Faction => faction;
    public float CombatRange => combatRange;
    public List<Weapon> Weapons => weapons;

    protected virtual void Awake()
    {
        Debug.Log($"[Ship] Awake called on {gameObject.name}");
        shipRigidbody = GetComponent<Rigidbody>();
        buoyancyComponent = GetComponent<Buoyancy>();
        movementComponent = GetComponent<ShipMovement>();
        selectionHandler = GetComponent<ShipSelectionHandler>();
        currentHealth = maxHealth;

        ValidateComponents();
        InitializeWeapons();
    }

    protected virtual void Start()
    {
        Debug.Log($"[Ship] Start called on {gameObject.name}");
        if (selectionHandler != null)
        {
            selectionHandler.Deselect();
        }
    }

    private void InitializeWeapons()
    {
        if (weaponPrefab == null)
        {
            Debug.LogWarning($"[Ship] No weapon prefab assigned to {gameObject.name}");
            return;
        }

        InitializeWeaponSide(portWeaponMounts, "Port");
        InitializeWeaponSide(starboardWeaponMounts, "Starboard");

        if (weapons.Count > 0)
        {
            CombatSystem.Instance?.RegisterShip(this, weapons);
        }
    }

    private void InitializeWeaponSide(Transform[] mounts, string side)
    {
        if (mounts == null) return;

        foreach (var mount in mounts)
        {
            if (mount == null) continue;

            GameObject weaponObj = Instantiate(weaponPrefab, mount.position, mount.rotation, mount);
            weaponObj.name = $"{side}_Weapon_{weapons.Count + 1}";
            
            Weapon weapon = weaponObj.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapons.Add(weapon);
                Debug.Log($"[Ship] Added {side} weapon to {gameObject.name}");
            }
        }
    }

    public virtual void FireAtTarget(Ship target)
    {
        if (target == null || target.Faction == this.Faction)
            return;

        OnTargetAcquired?.Invoke(target);
        CombatSystem.Instance?.InitiateCombat(this, target);
    }

    public virtual bool CanEngageTarget(Ship target)
    {
        if (target == null || target.Faction == this.Faction)
            return false;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= combatRange && !IsSinking && currentHealth > 0;
    }

    private void ValidateComponents()
    {
        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError($"[Ship] No Collider found on {gameObject.name}");
            gameObject.AddComponent<BoxCollider>();
            Debug.Log($"[Ship] Added BoxCollider to {gameObject.name}");
        }

        if (selectionHandler == null)
        {
            Debug.LogError($"[Ship] No ShipSelectionHandler found on {gameObject.name}");
            selectionHandler = gameObject.AddComponent<ShipSelectionHandler>();
            Debug.Log($"[Ship] Added ShipSelectionHandler to {gameObject.name}");
        }

        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
            Debug.Log($"[Ship] Set layer to Ship for {gameObject.name}");
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (null == obj) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    public virtual void Initialize(FactionType newFaction, string newName)
    {
        Debug.Log($"[Ship] Initializing {gameObject.name} with faction {newFaction} and name {newName}");
        faction = newFaction;
        shipName = newName;
    }

    public void SetName(string newName)
    {
        shipName = newName;
    }

    public virtual void SetOwner(IShipOwner newOwner)
    {
        Debug.Log($"[Ship] Setting owner for {gameObject.name} to {(newOwner != null ? newOwner.GetType().Name : "null")}");
        
        if (newOwner != null)
        {
            faction = newOwner.Faction;
            Debug.Log($"[Ship] Updated faction to {faction} to match new owner");
        }

        if (owner != null && !ReferenceEquals(owner, newOwner))
        {
            owner.RemoveShip(this);
        }

        owner = newOwner;

        if (isSelected && !(owner is Player))
        {
            Deselect();
        }
    }

    public virtual void ClearOwner()
    {
        Debug.Log($"[Ship] Clearing owner for {gameObject.name}");
        if (isSelected)
        {
            Deselect();
        }
        owner = null;
    }

    public virtual void Select()
    {
        Debug.Log($"[Ship] Selecting {gameObject.name}");
        if (owner is Player)
        {
            isSelected = true;
            if (selectionHandler != null)
            {
                selectionHandler.Select();
            }
        }
        else
        {
            Debug.LogWarning($"[Ship] Cannot select {gameObject.name} - not owned by player");
        }
    }

    public virtual void Deselect()
    {
        Debug.Log($"[Ship] Deselecting {gameObject.name}");
        isSelected = false;
        if (selectionHandler != null)
        {
            selectionHandler.Deselect();
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isSinking) return;

        float finalDamage = CalculateDamage(damage);
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);

        OnDamageTaken?.Invoke(finalDamage);

        if (currentHealth <= sinkingThreshold && !isSinking)
        {
            StartSinking();
        }
    }

    protected virtual float CalculateDamage(float rawDamage)
    {
        // Apply armor reduction
        float damageAfterArmor = rawDamage / armor;

        // Random chance for critical hit
        if (Random.value < 0.1f) // 10% chance
        {
            damageAfterArmor *= criticalHitMultiplier;
            Debug.Log($"Critical hit on {shipName}!");
        }

        return damageAfterArmor;
    }

    protected virtual void StartSinking()
    {
        isSinking = true;
        StartCoroutine(SinkingRoutine());
    }

    protected virtual IEnumerator SinkingRoutine()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (currentHealth > 0)
        {
            currentHealth = Mathf.Max(0, currentHealth - Time.deltaTime);

            if (waterSplashPrefab != null && 
                Random.value < waterFloodRate * Time.deltaTime && 
                buoyancyComponent != null)
            {
                Vector3 splashPosition = transform.position + Random.insideUnitSphere * 2f;
                splashPosition.y = buoyancyComponent.WaterLevel;
                Instantiate(waterSplashPrefab, splashPosition, Quaternion.identity);
            }

            yield return waitForEndOfFrame;
        }

        HandleShipDestroyed();
    }

    protected virtual void HandleShipDestroyed()
    {
        if (shipRigidbody != null) shipRigidbody.isKinematic = true;
        
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        
        if (isSelected) Deselect();
        ClearOwner();
        
        OnShipDestroyed?.Invoke();
        Debug.Log($"Ship {Name} has been destroyed!");
        
        ShipManager.Instance?.OnShipDestroyed(this);
    }

    protected virtual void OnDestroy()
    {
        OnShipDestroyed = null;
        OnDamageTaken = null;
        OnTargetAcquired = null;
    }

    protected virtual void OnValidate()
    {
        if (sinkingThreshold > maxHealth)
        {
            sinkingThreshold = maxHealth * 0.2f;
            Debug.LogWarning($"Adjusted sinking threshold to {sinkingThreshold}");
        }

        if (maxHealth <= 0)
        {
            maxHealth = 100f;
            Debug.LogWarning("Adjusted max health to default value (100)");
        }

        if (armor <= 0)
        {
            armor = 1f;
            Debug.LogWarning("Adjusted armor to default value (1)");
        }

        if (combatRange <= 0)
        {
            combatRange = 50f;
            Debug.LogWarning("Adjusted combat range to default value (50)");
        }
    }
}