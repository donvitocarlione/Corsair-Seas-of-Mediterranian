using UnityEngine;

public class Port : MonoBehaviour
{
    public string portName;
    public Faction controllingFaction;
    
    public void SetControllingFaction(Faction faction)
    {
        controllingFaction = faction;
    }
}
