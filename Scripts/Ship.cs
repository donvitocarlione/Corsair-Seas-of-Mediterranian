using UnityEngine;

public class Ship : MonoBehaviour
{
    public string shipName;
    public string shipType;
    public int maxCrew;
    public int currentCrew;
    public float maxCargo;
    public float currentCargo;
    public float health = 100f;
    public Pirate owner;
    public bool isSelected;

    private void Start()
    {
        isSelected = false;
    }

    public bool Select()
    {
        isSelected = true;
        // Add visual indication of selection
        ShowSelectionIndicator(true);
        return true;
    }

    public void Deselect()
    {
        isSelected = false;
        // Remove visual indication of selection
        ShowSelectionIndicator(false);
    }

    private void ShowSelectionIndicator(bool show)
    {
        // Implement visual feedback for selection
        // For example, enable/disable a highlight effect or selection ring
    }

    public void SetOwner(Pirate newOwner)
    {
        owner = newOwner;
    }

    public void TakeDamage(float amount)
    {
        health = Mathf.Max(0, health - amount);
        if (health <= 0)
        {
            // Handle ship destruction
            DestroyShip();
        }
    }

    public void Repair(float amount)
    {
        health = Mathf.Min(100, health + amount);
    }

    private void DestroyShip()
    {
        // Implement ship destruction logic
        if (owner != null)
        {
            owner.RemoveShip(this);
        }
        // Add destruction effects, etc.
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        // Handle ship selection on mouse click
        if (owner != null && owner is Player)
        {
            ((Player)owner).SelectShip(this);
        }
    }
}
