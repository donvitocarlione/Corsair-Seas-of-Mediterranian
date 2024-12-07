using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipSelection : MonoBehaviour
{
    [System.Serializable]
    public class ShipData
    {
        public string shipName;
        public GameObject shipPrefab;
        public float speed;
        public float health;
        public int cannonCount;
        public float maneuverability;
    }

    public ShipData[] availableShips;
    public Button nextButton;
    public Button previousButton;
    public Button selectButton;
    public TextMeshProUGUI shipNameText;
    public TextMeshProUGUI statsText;

    private int currentIndex = 0;
    private GameObject currentShipInstance;

    void Start()
    {
        nextButton.onClick.AddListener(() => ChangeShip(1));
        previousButton.onClick.AddListener(() => ChangeShip(-1));
        selectButton.onClick.AddListener(SelectCurrentShip);
        
        ShowShip(currentIndex);
    }

    void ChangeShip(int direction)
    {
        currentIndex += direction;
        
        if (currentIndex >= availableShips.Length)
            currentIndex = 0;
        else if (currentIndex < 0)
            currentIndex = availableShips.Length - 1;

        ShowShip(currentIndex);
    }

    void ShowShip(int index)
    {
        if (currentShipInstance != null)
            Destroy(currentShipInstance);

        ShipData ship = availableShips[index];
        currentShipInstance = Instantiate(ship.shipPrefab, Vector3.zero, Quaternion.identity);
        
        UpdateUI(ship);
    }

    void UpdateUI(ShipData ship)
    {
        shipNameText.text = ship.shipName;
        statsText.text = $"Speed: {ship.speed}\n" +
                        $"Health: {ship.health}\n" +
                        $"Cannons: {ship.cannonCount}\n" +
                        $"Maneuverability: {ship.maneuverability}";
    }

    void SelectCurrentShip()
    {
        PlayerPrefs.SetInt("SelectedShipIndex", currentIndex);
        // Add scene transition here
    }
}