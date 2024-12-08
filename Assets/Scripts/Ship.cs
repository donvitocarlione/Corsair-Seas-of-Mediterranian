using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Ship : SeaEntityBase
{
    [Header("Ship Properties")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float sinkingThreshold = 20f;
    [SerializeField] protected GameObject waterSplashPrefab;
    [SerializeField] protected float waterFloodRate = 0.1f;

    protected float currentHealth;
    protected bool isSelected;
    protected bool isSinking;
    protected IShipOwner owner;

    protected Rigidbody shipRigidbody;
    protected Buoyancy buoyancyComponent;
    protected ShipMovement movementComponent;

    public event UnityAction OnShipDestroyed;

    public float Health => currentHealth;
    public bool IsSelected => isSelected;
    public bool IsSinking => isSinking;
    public IShipOwner Owner => owner;

    protected virtual void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody>();
        buoyancyComponent = GetComponent<Buoyancy>();
        movementComponent = GetComponent<ShipMovement>();
        currentHealth = maxHealth;
    }

    protected override void Start()
    {
        base.Start();
    }

    public virtual void Initialize(FactionType newFaction, string newName)
    {
        SetFaction(newFaction);
        SetName(newName);
    }

    public virtual void SetOwner(IShipOwner newOwner)
    {
        if (owner != null && owner != newOwner)
        {
            owner.RemoveShip(this);
        }

        owner = newOwner;
    }

    public virtual void ClearOwner()
    {
        owner = null;
    }

    public virtual void Select()
    {
        isSelected = true;
    }

    public virtual void Deselect()
    {
        isSelected = false;
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
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
