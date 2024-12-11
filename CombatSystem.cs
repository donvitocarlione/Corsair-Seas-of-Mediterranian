using UnityEngine;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{
    private static CombatSystem instance;
    public static CombatSystem Instance => instance;

    [Header("Combat Settings")]
    public float maxCombatRange = 100f;
    public float minCombatRange = 10f;
    public float damageMultiplier = 1f;
    
    private Dictionary<Ship, List<Weapon>> shipWeapons = new Dictionary<Ship, List<Weapon>>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterShip(Ship ship, List<Weapon> weapons)
    {
        if (!shipWeapons.ContainsKey(ship))
            shipWeapons.Add(ship, weapons);
    }

    public void InitiateCombat(Ship attacker, Ship target)
    {
        if (!CanEngage(attacker, target))
            return;

        var availableWeapons = GetAvailableWeapons(attacker, target);
        foreach (var weapon in availableWeapons)
        {
            weapon.Fire(target);
        }
    }

    private bool CanEngage(Ship attacker, Ship target)
    {
        float distance = Vector3.Distance(attacker.transform.position, target.transform.position);
        return distance <= maxCombatRange && distance >= minCombatRange;
    }

    private List<Weapon> GetAvailableWeapons(Ship ship, Ship target)
    {
        List<Weapon> available = new List<Weapon>();
        if (!shipWeapons.ContainsKey(ship))
            return available;

        Vector3 directionToTarget = (target.transform.position - ship.transform.position).normalized;
        float angleToTarget = Vector3.Angle(ship.transform.right, directionToTarget);

        foreach (var weapon in shipWeapons[ship])
        {
            if (weapon.CanFireAtAngle(angleToTarget))
                available.Add(weapon);
        }

        return available;
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}