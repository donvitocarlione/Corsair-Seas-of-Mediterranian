using UnityEngine;
using UnityEngine.UI;

public class ShipSelector : MonoBehaviour
{
    public GameObject[] ships;
    public Button selectButton;
    private int selectedIndex = 0;

    void Start()
    {
        // Hide all ships initially
        foreach (GameObject ship in ships)
            ship.SetActive(false);

        // Show the first ship and enable selection
        ships[selectedIndex].SetActive(true);
        selectButton.onClick.AddListener(SelectNextShip);
    }

    void SelectNextShip()
    {
        // Deactivate the currently selected ship
        ships[selectedIndex].SetActive(false);

        // Move to the next ship in the array
        selectedIndex = (selectedIndex + 1) % ships.Length;
        ships[selectedIndex].SetActive(true);
    }
}