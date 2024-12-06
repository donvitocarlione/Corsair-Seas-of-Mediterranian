using UnityEngine;

[RequireComponent(typeof(Ship))]
public class PlayerShip : MonoBehaviour
{
    private Ship ship;
    
    void Awake()
    {
        ship = GetComponent<Ship>();
        if (ship != null)
        {
            var playerPirate = FindObjectOfType<PlayerPirate>();
            if (playerPirate != null)
            {
                ship.faction = playerPirate.faction;
                Debug.Log($"PlayerShip initialized with faction: {ship.faction}");
            }
        }
    }
}