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

        // Ensure proper component setup
        ValidateComponents();
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

        // Ensure ship is on correct layer
        if (gameObject.layer != LayerMask.NameToLayer("Ship"))
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ship"));
            Debug.Log($"[Ship] Set layer to Ship for {gameObject.name}");
        }

        Debug.Log($"[Ship] Components check for {gameObject.name}:\n" +
                  $"- Rigidbody: {shipRigidbody != null}\n" +
                  $"- Buoyancy: {buoyancyComponent != null}\n" +
                  $"- Movement: {movementComponent != null}\n" +
                  $"- Collider: {collider != null}\n" +
                  $"- SelectionHandler: {selectionHandler != null}");
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
        
        // First update the faction to match the new owner
        if (newOwner != null)
        {
            faction = newOwner.Faction;
            Debug.Log($"[Ship] Updated faction to {faction} to match new owner");
        }

        // Then handle owner reassignment
        if (owner != null && !ReferenceEquals(owner, newOwner))
        {
            owner.RemoveShip(this);
        }

        owner = newOwner;

        // Validate selection state after ownership change
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
                splashPosition.y = buoyancyComponent.WaterLevel; // Updated to use the new property
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