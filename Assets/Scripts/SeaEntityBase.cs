using UnityEngine;

public abstract class SeaEntityBase : MonoBehaviour
{
    [SerializeField]
    protected string entityName;
    protected FactionType faction;

    public string Name => entityName;
    public FactionType Faction => faction;

    protected virtual void Start()
    {
        if (string.IsNullOrEmpty(entityName))
        {
            entityName = gameObject.name;
        }
    }

    public virtual void SetName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            entityName = newName;
            gameObject.name = newName;
        }
    }

    public virtual void SetFaction(FactionType newFaction)
    {
        faction = newFaction;
    }

    protected virtual void OnDestroy() { }
}
