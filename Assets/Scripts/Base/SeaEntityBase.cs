using UnityEngine;

/// <summary>
/// Base class for all entities in the game that belong to factions
/// </summary>
[RequireComponent(typeof(Transform))]  // All sea entities need transforms
public abstract class SeaEntityBase : MonoBehaviour, IFactionMember, INameable
{
    [SerializeField, Tooltip("The faction this entity belongs to")]
    protected FactionType faction;
    
    [SerializeField, Tooltip("The name of this entity")]
    protected string entityName;

    public event System.Action<FactionType> OnFactionChanged;

    public FactionType Faction => faction;
    public string Name => entityName;
    public Color FactionColor => FactionManager.Instance?.GetFactionColor(faction) ?? Color.gray;

    protected virtual void Start()
    {
        ValidateEntity();
    }

    protected virtual void ValidateEntity()
    {
        // Ensure we have a valid name
        if (string.IsNullOrEmpty(entityName))
        {
            entityName = $"{GetType().Name}_{GetInstanceID()}";
            Debug.LogWarning($"Entity missing name, assigned default: {entityName}");
        }

        // Register with FactionManager if needed
        if (FactionManager.Instance != null)
        {
            var factionData = FactionManager.Instance.GetFactionData(faction);
            if (factionData == null)
            {
                Debug.LogWarning($"Entity {entityName} belongs to undefined faction {faction}, defaulting to Neutral");
                faction = FactionType.Neutral;
            }
        }
    }

    public virtual bool SetFaction(FactionType newFaction)
    {
        if (faction == newFaction) return false;

        faction = newFaction;
        OnFactionChanged?.Invoke(newFaction);
        return true;
    }

    public virtual bool SetName(string newName)
    {
        if (string.IsNullOrEmpty(newName)) return false;
        if (entityName == newName) return false;

        entityName = newName;
        return true;
    }

    protected virtual void OnDestroy()
    {
        // Clean up event subscribers
        OnFactionChanged = null;
    }
}
