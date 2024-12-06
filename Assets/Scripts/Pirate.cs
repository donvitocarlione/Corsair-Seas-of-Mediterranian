using System;

public class Pirate
{
    public FactionType faction;
    public string name;
    public int health;
    public float attackSpeed;

    public Pirate(FactionType faction, string name, int health, float attackSpeed)
    {
        this.faction = faction;
        this.name = name;
        this.health = health;
        this.attackSpeed = attackSpeed;
    }

    public void Initialize(Pirate pirate)
    {
        // Copy properties from the passed pirate object
        this.faction = pirate.faction;
        this.name = pirate.name;
        this.health = pirate.health;
        this.attackSpeed = pirate.attackSpeed;
    }

    public void Attack(Ship target)
    {
        // Implement attack logic here
    }
}
