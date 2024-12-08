using UnityEngine;

[CreateAssetMenu(fileName = "New Faction", menuName = "Game/Faction Definition")]
public class FactionDefinition : ScriptableObject
{
    public FactionType factionType;
    public string factionName;
    public Color factionColor = Color.white;
    public Sprite factionFlag;
    
    [Header("Starting Resources")]
    public int startingGold = 1000;
    public int startingShips = 1;
    
    [Header("Relationships")]
    public float defaultRelationship = 0f;
    public float allyThreshold = 75f;
    public float friendlyThreshold = 50f;
    public float neutralThreshold = 25f;
    public float hostileThreshold = 0f;
    
    [Header("Gameplay Settings")]
    public bool canBePlayerFaction = true;
    public bool startsNeutralWithAll = true;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(factionName))
        {
            factionName = factionType.ToString();
        }
    }
}
