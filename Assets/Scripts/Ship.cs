using UnityEngine;
using System.Collections;

public class Ship : SeaEntityBase
{
    [Header("Ship Properties")]
    public string shipName;
    public float maxHealth = 100f;
    public float sinkingThreshold = 20f;  // Health threshold when ship starts sinking
    public float sinkingDuration = 30f;   // How long it takes to fully sink
    
    [Header("Sinking Effects")]
    public float maxTiltAngle = 45f;      // Maximum tilt when sinking
    public float waterFloodRate = 0.1f;   // How fast the ship takes on water
    public GameObject waterSplashPrefab;   // Optional water splash effect
    
    [Header("Debug Info")]
    public float currentHealth;
    public bool isSinking;
    public float sinkProgress;
    
    private bool isSelected;
    private Pirate ownerPirate;
    private Buoyancy buoyancyComponent;
    private Rigidbody shipRigidbody;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float originalBuoyancyForce;
    
    public bool IsSelected => isSelected;
    public Pirate owner => ownerPirate;
    
    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(shipName))
        {
            shipName = $"Ship_{Random.Range(1000, 9999)}";
        }
        
        // Cache components
        buoyancyComponent = GetComponent<Buoyancy>();
        shipRigidbody = GetComponent<Rigidbody>();
        
        if (buoyancyComponent == null)
        {
            Debug.LogError($"Ship {shipName} is missing Buoyancy component!");
            enabled = false;
            return;
        }
        
        // Store original values
        currentHealth = maxHealth;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalBuoyancyForce = buoyancyComponent.buoyancyForce;
    }
    
    public void Initialize(FactionType faction, string name)
    {
        shipName = name;
        SetFaction(faction);
        Debug.Log($"Initialized ship {shipName} with faction {faction}");
    }
    
    public void TakeDamage(float amount)
    {
        if (isSinking) return; // Already sinking
        
        currentHealth = Mathf.Max(0, currentHealth - amount);
        
        // Check if should start sinking
        if (currentHealth <= sinkingThreshold && !isSinking)
        {
            StartSinking();
        }
    }
    
    private void StartSinking()
    {
        isSinking = true;
        sinkProgress = 0f;
        
        // Disable movement and player control
        var movement = GetComponent<ShipMovement>();
        if (movement != null) movement.enabled = false;
        
        // Start sinking coroutine
        StartCoroutine(SinkingCoroutine());
        
        Debug.Log($"Ship {shipName} started sinking!");
    }
    
    private IEnumerator SinkingCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 targetRotation = new Vector3(
            Random.Range(-maxTiltAngle, maxTiltAngle),
            transform.eulerAngles.y,
            Random.Range(-maxTiltAngle, maxTiltAngle)
        );
        
        while (elapsedTime < sinkingDuration)
        {
            elapsedTime += Time.deltaTime;
            sinkProgress = elapsedTime / sinkingDuration;
            
            // Gradually reduce buoyancy
            buoyancyComponent.buoyancyForce = Mathf.Lerp(
                originalBuoyancyForce,
                0f,
                sinkProgress
            );
            
            // Tilt the ship
            transform.rotation = Quaternion.Lerp(
                originalRotation,
                Quaternion.Euler(targetRotation),
                sinkProgress
            );
            
            // Add water splashes
            if (waterSplashPrefab != null && Random.value < waterFloodRate * Time.deltaTime)
            {
                Vector3 splashPosition = transform.position + Random.insideUnitSphere * 2f;
                splashPosition.y = buoyancyComponent.waterLevelY;
                Instantiate(waterSplashPrefab, splashPosition, Quaternion.identity);
            }
            
            yield return null;
        }
        
        // Ship has fully sunk
        OnShipDestroyed();
    }
    
    private void OnShipDestroyed()
    {
        // Notify relevant systems
        if (ownerPirate != null)
        {
            ownerPirate.RemoveShip(this);
        }
        
        // Disable physics and rendering
        if (shipRigidbody != null) shipRigidbody.isKinematic = true;
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        
        // Hide selection if selected
        if (isSelected)
        {
            Deselect();
        }
        
        Debug.Log($"Ship {shipName} has been destroyed!");
        
        // Optional: Could add delayed destruction
        // Destroy(gameObject, 5f);
    }
    
    public void Select()
    {
        isSelected = true;
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.ShowSelectionAt(transform);
            Debug.Log($"Selected ship {shipName}");
        }
    }
    
    public void Deselect()
    {
        isSelected = false;
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.HideSelection();
            Debug.Log($"Deselected ship {shipName}");
        }
    }
    
    public void SetOwner(Pirate newOwner)
    {
        if (ownerPirate != null)
        {
            ownerPirate.RemoveShip(this);
        }
        
        ownerPirate = newOwner;
        
        if (ownerPirate != null)
        {
            SetFaction(ownerPirate.Faction);
            Debug.Log($"Set owner of {shipName} to {ownerPirate.GetType().Name}");
        }
        else
        {
            Debug.Log($"Cleared owner of {shipName}");
        }
    }
    
    protected override void OnFactionChanged()
    {
        base.OnFactionChanged();
        Debug.Log($"Ship {shipName} faction changed to {Faction}");
    }
    
    void OnValidate()
    {
        // Ensure sinking threshold is less than max health
        if (sinkingThreshold > maxHealth)
        {
            sinkingThreshold = maxHealth * 0.2f; // Default to 20% of max health
        }
    }
}
