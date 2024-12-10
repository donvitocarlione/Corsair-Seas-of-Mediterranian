using UnityEngine;
using System.Collections;
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

    protected float currentHealth;
    protected bool isSelected;
    protected bool isSinking;
    protected IShipOwner owner;

    protected Rigidbody shipRigidbody;
    protected Buoyancy buoyancyComponent;
    protected ShipMovement movementComponent;
    protected ShipSelectionHandler selectionHandler;

    public event UnityAction OnShipDestroyed;

    public float Health => currentHealth;
    public bool IsSelected => isSelected;
    public bool IsSinking => isSinking;
    public IShipOwner ShipOwner => owner;
    public string ShipName => Name;
    public string Name => shipName;
    public FactionType Faction => faction;

    protected virtual void Awake()
    {
        Debug.Log($"[Ship] Awake called on {gameObject.name}");
        shipRigidbody = GetComponent<Rigidbody>();
        buoyancyComponent = GetComponent<Buoyancy>();
        movementComponent = GetComponent<ShipMovement>();
        selectionHandler = GetComponent<ShipSelectionHandler>();
        currentHealth = maxHealth;

        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError($"[Ship] No Collider found on {gameObject.name}");
        }

        if (selectionHandler == null)
        {
            Debug.LogError($"[Ship] No ShipSelectionHandler found on {gameObject.name}");
        }

        Debug.Log($"[Ship] Components check for {gameObject.name}:\n" +
                  $"- Rigidbody: {shipRigidbody != null}\n" +
                  $"- Buoyancy: {buoyancyComponent != null}\n" +
                  $"- Movement: {movementComponent != null}\n" +
                  $"- Collider: {collider != null}\n" +
                  $"- SelectionHandler: {selectionHandler != null}");
    }

    protected virtual void Start()
    {
        Debug.Log($"[Ship] Start called on {gameObject.name}");
        // Ensure selection indicator starts hidden
        if (selectionHandler != null)
        {
            selectionHandler.Deselect();
        }
    }

    public virtual void Initialize(FactionType newFaction, string newName)
    {
        Debug.Log($"[Ship] Initializing {gameObject.name} with faction {newFaction} and name {newName}");
        SetFaction(newFaction);
        SetName(newName);
    }

    public void SetName(string newName)
    {
        shipName = newName;
    }

    public void SetFaction(FactionType newFaction)
    {
        // Only allow faction changes if the ship has no owner
        if (owner != null)
        {
            Debug.LogWarning($"[Ship] Attempted to change faction of {shipName} while owned by {owner.GetType().Name}. Faction changes must be done through the owner.");
            return;
        }

        Debug.Log($"[Ship] Setting faction for {shipName} from {faction} to {newFaction}");
        faction = newFaction;
    }

    public virtual void SetOwner(IShipOwner newOwner)
    {
        Debug.Log($"[Ship] Setting owner for {gameObject.name} to {(newOwner != null ? newOwner.GetType().Name : "null")}");
        
        if (newOwner != null)
        {
            // Validate faction matches owner's faction
            if (newOwner.Faction != faction)
            {
                Debug.Log($"[Ship] Updating faction from {faction} to match new owner's faction {newOwner.Faction}");
                faction = newOwner.Faction;
            }
        }

        if (owner != null && !ReferenceEquals(owner, newOwner))
        {
            owner.RemoveShip(this);
        }

        owner = newOwner;
    }

    public virtual void ClearOwner()
    {
        Debug.Log($"[Ship] Clearing owner for {gameObject.name}");
        owner = null;
    }

    public virtual void Select()
    {
        Debug.Log($"[Ship] Selecting {gameObject.name}");
        isSelected = true;
        if (selectionHandler != null)
        {
            selectionHandler.Select();
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

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= sinkingThreshold && !isSinking)
        {
            StartSinking();
        }
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
    }
}