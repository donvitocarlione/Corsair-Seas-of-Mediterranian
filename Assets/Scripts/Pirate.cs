using System;
using UnityEngine;

public class Pirate
{
    public FactionType faction;
    public string name;
    public int health;
    public float attackSpeed;
    public int reputation;         // Reputation affects trading and interaction options
    public int crewSize;          // Size of the pirate's crew
    public float combatSkill;     // Affects combat effectiveness
    public string specialization; // E.g., "Boarding", "Cannon warfare", "Navigation"

    public Pirate(FactionType faction, string name, int health, float attackSpeed)
    {
        this.faction = faction;
        this.name = name;
        this.health = health;
        this.attackSpeed = attackSpeed;
        this.reputation = 0;
        this.crewSize = 50;  // Default crew size
        this.combatSkill = 1.0f;
        this.specialization = "None";
    }

    public void Initialize(Pirate pirate)
    {
        this.faction = pirate.faction;
        this.name = pirate.name;
        this.health = pirate.health;
        this.attackSpeed = pirate.attackSpeed;
        this.reputation = pirate.reputation;
        this.crewSize = pirate.crewSize;
        this.combatSkill = pirate.combatSkill;
        this.specialization = pirate.specialization;
    }

    public void Attack(Ship target)
    {
        // Calculate damage based on combat skill and crew size
        float damageMultiplier = combatSkill * (crewSize / 50f);
        // Implementation will be connected to the Ship's damage system
    }

    public void UpdateReputation(int change)
    {
        reputation += change;
        // Clamp reputation between -100 and 100
        reputation = Mathf.Clamp(reputation, -100, 100);
    }
}