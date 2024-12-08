using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShipSelectionUI : MonoBehaviour
{
    [SerializeField]
    private Transform shipListContainer;
    [SerializeField]
    private Button shipButtonPrefab;
    [SerializeField]
    private Player player;

    private List<Button> shipButtons = new List<Button>();

    private void Awake()
    {
        // Try to find player if not assigned
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player == null)
            {
                Debug.LogError("No Player found in scene! Ship selection will not work.");
            }
        }
    }

    public void UpdateShipList(List<Ship> ships)
    {
        ClearShipButtons();

        foreach (var ship in ships)
        {
            CreateShipButton(ship);
        }
    }

    private void CreateShipButton(Ship ship)
    {
        var button = Instantiate(shipButtonPrefab, shipListContainer);
        var text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = ship.Name;
        }

        button.onClick.AddListener(() => OnShipButtonClicked(ship));
        shipButtons.Add(button);
    }

    private void ClearShipButtons()
    {
        foreach (var button in shipButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }
        }
        shipButtons.Clear();
    }

    private void OnShipButtonClicked(Ship ship)
    {
        if (player != null)
        {
            player.SelectShip(ship);
        }
        else
        {
            Debug.LogWarning("Cannot select ship: Player reference is missing!");
        }
    }

    public void UpdateSelection(Ship selectedShip)
    {
        foreach (var button in shipButtons)
        {
            var text = button.GetComponentInChildren<Text>();
            if (text != null && text.text == selectedShip.Name)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
    }

    private void OnDestroy()
    {
        ClearShipButtons();
    }
}