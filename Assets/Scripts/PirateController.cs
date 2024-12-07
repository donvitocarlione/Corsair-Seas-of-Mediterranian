using UnityEngine;

[RequireComponent(typeof(Pirate))]
public class PirateController : MonoBehaviour
{
    private Pirate pirate;

    private void Start()
    {
        pirate = GetComponent<Pirate>();
        
        if (pirate.selectedShip == null && pirate.ships.Count > 0)
        {
            pirate.SelectShip(pirate.ships[0]);
        }
    }
}
