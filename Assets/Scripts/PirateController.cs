using UnityEngine;

[RequireComponent(typeof(Pirate))]
public class PirateController : MonoBehaviour
{
    private Pirate pirate;

    private void Start()
    {
        pirate = GetComponent<Pirate>();

        if (pirate == null)
        {
            Debug.LogError("PirateController requires a Pirate component!");
            enabled = false;
            return;
        }

        // If the pirate has ships but none selected, select the first one
        if (pirate.selectedShip == null && pirate.ships.Count > 0)
        {
            pirate.SelectShip(pirate.ships[0]);
        }
    }
}
