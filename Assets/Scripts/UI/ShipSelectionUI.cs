using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipSelectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shipNameText;
    [SerializeField] private TextMeshProUGUI shipTypeText;
    [SerializeField] private TextMeshProUGUI shipHealthText;
    [SerializeField] private Image shipIcon;
    [SerializeField] private Button selectButton;
    
    private Ship currentShip;
    private ShipSelectionHandler selectionHandler;
    
    private void Start()
    {
        selectionHandler = FindObjectOfType<ShipSelectionHandler>();
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
        }
    }
    
    public void UpdateUI(Ship ship)
    {
        if (ship == null)
        {
            Debug.LogError("Attempting to update UI with null ship!");
            return;
        }
        
        currentShip = ship;
        
        if (shipNameText != null) shipNameText.text = ship.Name;
        if (shipTypeText != null) shipTypeText.text = ship.GetType().Name;
        if (shipHealthText != null) shipHealthText.text = $"Health: {ship.CurrentHealth}/{ship.MaxHealth}";
        if (shipIcon != null && ship.Icon != null) shipIcon.sprite = ship.Icon;
        
        gameObject.SetActive(true);
    }
    
    private void OnSelectButtonClicked()
    {
        if (currentShip != null && selectionHandler != null)
        {
            selectionHandler.HandleShipSelected(currentShip);
        }
    }
    
    public void Clear()
    {
        currentShip = null;
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveListener(OnSelectButtonClicked);
        }
    }
}
