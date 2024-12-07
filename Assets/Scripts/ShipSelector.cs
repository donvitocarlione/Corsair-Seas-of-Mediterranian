using UnityEngine;
using System.Collections;

public class ShipSelector : MonoBehaviour
{
    public static ShipSelector Instance { get; private set; }
    
    [SerializeField] private GameObject[] shipPrefabs;
    private GameObject currentShip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SpawnSelectedShip();
    }

    public void SpawnSelectedShip()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedShipIndex", 0);
        
        if (currentShip != null)
            Destroy(currentShip);

        if (selectedIndex < shipPrefabs.Length)
        {
            Vector3 spawnPosition = new Vector3(0, 0, 0); // Adjust as needed
            currentShip = Instantiate(shipPrefabs[selectedIndex], spawnPosition, Quaternion.identity);
        }
    }

    public GameObject GetCurrentShip()
    {
        return currentShip;
    }
}