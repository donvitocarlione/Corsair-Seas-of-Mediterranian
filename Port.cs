using UnityEngine;

public class Port : MonoBehaviour
{
    [SerializeField]
    private FactionType owningFaction = FactionType.None;
    
    [SerializeField]
    private string portName;
    
    public FactionType OwningFaction => owningFaction;
    public string PortName => portName;

    public void SetFaction(FactionType newFaction)
    {
        if (owningFaction != newFaction)
        {
            var oldFaction = owningFaction;
            owningFaction = newFaction;
            
            // Notify FactionManager of the change
            if (FactionManager.Instance != null)
            {
                FactionManager.Instance.HandlePortCapture(newFaction, this);
            }
            
            Debug.Log($"Port {portName} changed ownership from {oldFaction} to {newFaction}");
        }
    }

    protected virtual void Start()
    {
        // Register with initial faction
        if (owningFaction != FactionType.None && FactionManager.Instance != null)
        {
            var factionData = FactionManager.Instance.GetFactionData(owningFaction);
            if (factionData != null)
            {
                factionData.AddPort(this);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        // Unregister from current faction
        if (owningFaction != FactionType.None && FactionManager.Instance != null)
        {
            var factionData = FactionManager.Instance.GetFactionData(owningFaction);
            if (factionData != null)
            {
                factionData.RemovePort(this);
            }
        }
    }
}