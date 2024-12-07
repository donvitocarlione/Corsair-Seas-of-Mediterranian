using UnityEngine;

public abstract class SeaEntityBase : MonoBehaviour
{
    public FactionType Faction { get; protected set; }
    protected string entityName;
    
    public virtual void SetFaction(FactionType faction)
    {
        this.Faction = faction;
        OnFactionChanged();
    }
    
    protected virtual void OnFactionChanged() {}
}